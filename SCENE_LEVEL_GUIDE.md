# Scene & Level Management System

Complete level loading, scene transitions, and checkpoint management system for your game.

## Overview

The scene/level management system provides:
- **Level Definition** - Data structures for levels with spawn points, checkpoints, and metadata
- **Scene Management** - Loading/unloading scenes with smooth transitions
- **Level Loader** - Procedural level generation and entity instantiation
- **Checkpoint System** - Player respawn points and progression tracking
- **Scene Transitions** - Fade in/out effects between scenes

## Architecture

### Core Components

**Level.cs** - Defines level data structures
- `Level` - Represents a complete game level
- `LevelCheckpoint` - Respawn point within a level

**SceneManager.cs** - ISystem that manages active scenes
- Controls scene loading/unloading
- Handles scene transitions with callbacks
- Tracks level progression

**LevelLoader.cs** - Loads level data and populates scenes
- Instantiates entities from level definitions
- Supports procedural level generation
- Includes `CreateTestLevel()` factory method

**CheckpointSystem.cs** - Monitors and manages checkpoints
- `CheckpointComponent` - Component for checkpoint entities
- `CheckpointSystem` - System that detects checkpoint activation
- `SceneTransitionEffect` - Visual effects during transitions
- `SceneTransitionSystem` - Handles fade animations

## Quick Start

### 1. Initialize Systems in Game Setup

```csharp
// In GameSetup.cs SetupSonicLikeGame()
var sceneManager = new SceneManager(game, audio);
var checkpointSystem = new CheckpointSystem();

game.AddSystem(sceneManager);
game.AddSystem(checkpointSystem);
game.AddSystem(new SceneTransitionSystem(renderer));

// Setup levels
SetupLevelSystem(game, sceneManager, checkpointSystem);
```

### 2. Create a Level

```csharp
var level = new Level("level_1", "Zone 1", 1)
{
    PlayerSpawnPoint = Vector3.Zero,
    CameraStartPosition = new Vector3(0, 5, -10),
    MusicTrackId = "main_gameplay",
    GraphicsPreset = RetroPresets.SegaGenesis,
    DifficultyMultiplier = 1.0f
};

// Add checkpoints
level.AddCheckpoint(new LevelCheckpoint(
    "cp_start",
    "Start",
    Vector3.Zero,
    0
));

level.AddCheckpoint(new LevelCheckpoint(
    "cp_mid",
    "Midpoint",
    new Vector3(50, 0, 0),
    1
));

// Define level population
level.OnLevelLoad = (lv, game) => PopulateLevel(game, lv);
```

### 3. Register and Load Levels

```csharp
// Register levels
sceneManager.RegisterLevel(level1);
sceneManager.RegisterLevel(level2);
sceneManager.RegisterLevel(level3);

// Load first level
sceneManager.LoadLevel(level1);
```

### 4. Create Checkpoints in Level

```csharp
var checkpointEntity = game.CreateEntity();
checkpointEntity.AddComponent(new Tag("CheckpointMarker"));
checkpointEntity.AddComponent(new Transform3D { Position = checkpoint.Position });
checkpointEntity.AddComponent(new CollisionSphere { Radius = 2.0f, IsTrigger = true });
checkpointEntity.AddComponent(new CheckpointComponent 
{ 
    Checkpoint = checkpoint,
    ActivationRadius = 2.0f
});
```

## API Reference

### Level Class

**Properties:**
- `string LevelId` - Unique identifier
- `string LevelName` - Display name
- `int LevelNumber` - For progression
- `float DifficultyMultiplier` - Scales enemies/hazards
- `Vector3 PlayerSpawnPoint` - Player start position
- `Vector3 CameraStartPosition` - Camera start position
- `string? MusicTrackId` - Background music ID
- `RetroGraphicsEffect? GraphicsPreset` - Visual style
- `List<LevelCheckpoint> Checkpoints` - All checkpoints
- `int EnemyCount` - Enemy spawn count
- `int CollectibleCount` - Collectible spawn count
- `Action<Level, Game>? OnLevelLoad` - Callback when loading
- `Action<Level>? OnLevelUnload` - Callback when unloading

**Methods:**
- `AddCheckpoint(LevelCheckpoint)` - Add a checkpoint
- `GetCheckpoint(string id)` - Get checkpoint by ID
- `GetNearestCheckpoint(Vector3 pos)` - Find nearest checkpoint

### SceneManager (ISystem)

**Properties:**
- `bool IsTransitioning` - Transition in progress
- `float TransitionDuration` - Fade duration (default 1.0s)
- `Level? CurrentLevel` - Active level

**Events:**
- `Action<Scene, Level>? OnSceneLoaded` - Scene loaded
- `Action<Scene, Level>? OnSceneUnloaded` - Scene unloaded
- `Action<Level?, Level>? OnTransitionStart` - Transition beginning
- `Action<Level>? OnTransitionComplete` - Transition finished

**Methods:**
- `LoadLevel(Level level)` - Load a level
- `LoadLevelById(string id)` - Load level by ID
- `RegisterLevel(Level level)` - Register a level
- `GetLevel(string id)` - Get level from registry
- `GetCurrentLevel()` - Get active level
- `GetNextLevel()` - Get next level in progression
- `GetActiveScene()` - Get current scene
- `UnloadScene()` - Unload current scene
- `ReloadCurrentLevel()` - Restart current level
- `AddProgressionPoints(int points)` - Track progress
- `GetProgressionPoints()` - Get total progress
- `GetLevelHistory()` - Get visited levels

### CheckpointSystem (ISystem)

**Events:**
- `Action<LevelCheckpoint>? OnCheckpointActivated` - Checkpoint reached

**Methods:**
- `GetLastCheckpoint()` - Get active checkpoint
- `RespawnAtCheckpoint(Entity player)` - Respawn player

### LevelLoader

**Static Methods:**
- `CreateTestLevel(int levelNumber)` - Generate test level

**Methods:**
- `LoadLevel(Level level)` - Load level into scene

## Examples

### Example: Load Multiple Levels with Progression

```csharp
// Create level sequence
var world = new Level("world_1", "The World", 1)
{
    PlayerSpawnPoint = Vector3.Zero,
    MusicTrackId = "world_theme"
};

world.OnLevelLoad = (level, game) =>
{
    // Create world entities...
    CreatePlayerSpawnArea(game);
    CreateEnemies(game, (int)(5 * level.DifficultyMultiplier));
    CreateCollectibles(game, 20);
};

// Register and load
sceneManager.RegisterLevel(world);
sceneManager.LoadLevel(world);

// Listen for transitions
sceneManager.OnTransitionComplete += (level) =>
{
    Console.WriteLine($"Now in {level.LevelName}");
};
```

### Example: Custom Checkpoint Behavior

```csharp
var checkpoint = new LevelCheckpoint("cp_boss", "Boss Arena", bossPos, 2);

checkpoint.OnCheckpointReached += (cp) =>
{
    // Trigger boss music, spawn boss, etc.
    Console.WriteLine($"Approaching: {cp.Name}");
};

level.AddCheckpoint(checkpoint);
```

### Example: Dynamic Level Difficulty

```csharp
// Increase difficulty based on player progress
int progress = sceneManager.GetProgressionPoints();
float difficulty = 1.0f + (progress * 0.1f);

var nextLevel = sceneManager.GetNextLevel();
if (nextLevel != null)
{
    nextLevel.DifficultyMultiplier = difficulty;
    sceneManager.LoadLevel(nextLevel);
}
```

## Testing

The system includes `LevelLoader.CreateTestLevel(number)` which generates complete test levels:
- Automatically creates player, enemies, collectibles, hazards
- Scales difficulty by level number
- Sets up 3 checkpoints per level

```csharp
// Auto-generate 3 test levels
var level1 = LevelLoader.CreateTestLevel(1); // Easy
var level2 = LevelLoader.CreateTestLevel(2); // Medium
var level3 = LevelLoader.CreateTestLevel(3); // Hard
```

## Best Practices

1. **Level Design** - Separate level data from game logic
2. **Checkpoints** - Place at major milestones, not too frequent
3. **Transitions** - Keep fade duration short (0.5-1.5s)
4. **Progression** - Track key achievements in level history
5. **Cleanup** - OnLevelUnload callbacks should clean up resources

## Integration Checklist

✅ Systems added to game update loop
✅ Levels created and registered
✅ Checkpoints placed in levels
✅ Transition callbacks configured
✅ Audio/graphics presets assigned
✅ Difficulty scaling implemented
✅ Progression tracking enabled

## See Also

- [GameSetup.cs](GameSetup.cs) - Complete setup integration
- [Scene.cs](Scene.cs) - Basic scene entity container
- [Game.cs](Game.cs) - Main game loop
- [SaveLoadSystem.cs](SaveLoadSystem.cs) - Level progress persistence
