# New_Game

## Overview

A **3D action-puzzle game core** (C#) inspired by **Sonic** with combat mechanics, **mouse-controlled camera**, and **multi-monitor damage feedback**. **Engine-agnostic** architecture with **complete Unity integration provided**.

**👉 [UNITY_SETUP.md](UNITY_SETUP.md) — Get started with Unity in 5 minutes!**

### Features

- **3D Physics**: gravity, velocity, drag, kinematic bodies
- **Collision & Damage**: sphere-based detection, knockback, health system
- **Player Control**: movement, jumping, double jump, dashing
- **� Projectile System**: fireballs, homing shots, with automatic despawn
- **💰 Collectibles**: rings, coins, health pickups, power-ups (speed boost, invincibility)
- **🎮 Level Obstacles**: spikes, lava, moving platforms, conveyor belts, trampolines
- **🖱️ Mouse-Controlled Camera**: third-person follow with mouse look
- **🖱️ Mouse Lock**: locked to screen during gameplay, unlock with ESC
- **💥 Damage Feedback**: red screen flash on all monitors when hit (customizable)
- **🎨 Retro Graphics**: pixelated, color-reduced rendering like classic Sonic (multiple presets)
- **Enemy AI**: patrol → chase → attack state machine
- **Puzzle Interactions**: switches, levers, doors with callbacks
- **🔊 Audio System**: sound effects, background music, volume control, preloading
- **🎯 UI System**: health bars, score display, damage popups, minimap, crosshair, interaction prompts
- **⌨️ Input System**: keyboard input management, menu navigation, dialogue choices, save/load bindings
- **💬 Dialogue System**: branching dialogue with voice acting support, JSON data-driven
- **📋 Menu System**: pause menus, main menus, settings, menu stacking
- **💾 Save/Load System**: 10 save slots, player/entity state persistence, playtime tracking
- **Component-based architecture**: ECS-style entity system

### Core files

| File | Purpose |
|------|---------|
| `Vector3.cs` | 3D math utilities (Lerp added) |
| `Components3D.cs` | Transform, Rigidbody, Health, Damage, Collision components |
| `CameraComponents.cs` | Camera, CameraController, ScreenFlashEffect, UILayer |
| `RetroGraphicsEffect.cs` | Pixelated/retro graphics component + presets (Genesis, Master System, Game Boy, etc.) |
| `GameplayComponents.cs` | Projectile, Collectible, PowerUp, Hazard, MovingPlatform, Trampoline, ConveyorBelt |
| `Physics3DSystem.cs` | Physics simulation + collision detection/response + damage flash + trampoline bounce |
| `CameraSystem.cs` | Camera follow logic + mouse input handling + mouse lock toggle |
| `ScreenFlashSystem.cs` | Damage feedback flash renderer |
| `RetroGraphicsSystem.cs` | Applies retro pixel effect post-processing |
| `GameplaySystem.cs` | ProjectileSystem, CollectibleSystem, HazardSystem, PlatformSystem, PowerUpSystem |
| `CharacterControllers.cs` | PlayerController (movement, jump, dash) + EnemyAI (patrol/chase/attack) |
| `GameSystems.cs` | PlayerInputSystem (now with firing), InteractionSystem, EnemyAISystem |
| `GameSetup.cs` | Complete game initialization: player, enemy, camera, collectibles, hazards, platforms |
| `Game.cs` | Main game loop & entity manager |
| `Entity.cs` | ECS entity with component storage |
| `IEngineRenderer.cs` | Engine bridge interfaces (mouse, flash, retro graphics, screen size, audio) |
| `EngineBridgeExamples.cs` | Unity/Godot implementation examples |
| `AudioComponents.cs` | AudioSource, AudioManager, SFXPlayer, BackgroundMusic components |
| `AudioSystem.cs` | Audio processing system with volume control & fade effects |
| `AudioExample.cs` | Complete audio usage examples |
| **Input System** | |
| `InputManager.cs` | Core input manager, key binding system, input state tracking |
| `InputHandlers.cs` | MenuInputHandler, DialogueInputHandler, SaveLoadInputHandler, PlayerInputHandler |
| `InputIntegrationExample.cs` | Complete integration examples for all input handlers |
| **Dialogue & Menu Systems** | |
| `DialogueComponents.cs` | DialogueLine, DialogueChoice, DialogueSequence, DialogueComponent, EncounterTrigger |
| `DialogueScriptParser.cs` | **NEW**: Dialogue script parser + builder (write dialogue without code!) |
| `DialogueSystem.cs` | Dialogue manager system with callbacks |
| `DialogueDataLoader.cs` | JSON dialogue data loader |
| `DIALOGUE_SCRIPTING_GUIDE.md` | **NEW**: Guide for writing dialogue scripts (non-code) |
| `example_dialogue.txt` | **NEW**: Example dialogue script file |
| `MenuComponents.cs` | MenuButton, MenuComponent, ConfirmDialog |
| `MenuSystem.cs` | Menu manager with stacking, pause integration |
| `MenuExamples.cs` | Pre-built menu templates |
| **Save/Load System** | |
| `SaveComponents.cs` | SaveSlot, GameState, PlayerState, EntityState, SaveableComponent |
| `SaveLoadSystem.cs` | Save/load manager with file I/O |
| `SaveLoadExamples.cs` | Save/load usage examples |
| **Documentation** | |
| `INPUT_SYSTEM_GUIDE.md` | Complete input system documentation |
| `DIALOGUE_GUIDE.md` | Dialogue system setup and usage |
| `MENUS_GUIDE.md` | Menu system guide |
| `SAVELOAD_GUIDE.md` | Save/load system guide |
| `UnityAudioBridge.cs` | Full Unity audio implementation (preload, cache, volume) |
| `UIComponents.cs` | HealthBar, ScoreDisplay, DamageNumber, Minimap, TextDisplay, etc. |
| `UISystem.cs` | UI element processing, positioning, animations & drawing |
| `UIExample.cs` | Complete UI usage examples |
| `UI_SYSTEM_GUIDE.md` | Comprehensive UI system documentation |

## Quick start

```csharp
// 1. Bridge to your engine
var inputBridge = new UnityInputBridge();           // Implement IEngineInput
var rendererBridge = new UnityRendererBridge();     // Implement IEngineRenderer
var audioBridge = new UnityAudioBridge();           // Implement IEngineAudio

// 2. Initialize game with audio support
var game = new GameCore.Game(inputBridge, audioBridge);
GameSetup.SetupSonicLikeGame(game, inputBridge, rendererBridge, audioBridge);

// 3. In your engine's update loop
void EngineUpdate(float deltaTime)
{
    game.Update(deltaTime);
}
```

## Testing

The project includes **comprehensive unit tests** for core gameplay systems:

### Running tests

```bash
cd GameTests
dotnet test
```

### Test coverage

- **54 total tests** (all passing ✅)
- **21 Vector3 math tests**: operations, magnitude, normalization, Lerp, distance, dot product
- **33 gameplay component tests**: health system, damage, collectibles, hazards, platforms

### Test categories

#### Health System (8 tests)
- Initialization and max health
- Damage application and clamping
- Healing and maximum capping
- Death state transitions
- Multiple damage events

#### Damage System (2 tests)
- DamageSource property storage
- Default values

#### Collectibles (5 tests)
- Ring, coin, health pickup creation
- All collectible types supported
- Collected state tracking

#### Hazards (5 tests)
- Spike, lava, and all hazard types
- Damage amount configuration
- Update method execution

#### Platforms (6 tests)
- Moving platform creation and wait times
- Trampoline bounce force (default and custom)
- Conveyor belt direction normalization

#### Vector3 Math (21 tests)
- Construction and statics (Zero, One, Up, Down, etc.)
- Arithmetic (addition, subtraction, multiplication, division)
- Magnitude, normalization, dot product
- Distance calculation, Lerp (with and without clamping)
- Equality, hashing, string representation

#### Integration Tests (4 tests)
- Player health with multiple damages
- Collectible health restoration
- Multiple hazards in sequence
- Vector3 combat calculations (distance, direction, knockback)

### Example test

```csharp
[Test]
public void Health_TakeDamage_ReducesCurrentHealth()
{
    // Arrange
    var health = new Health(100);

    // Act
    health.TakeDamage(25);

    // Assert
    Assert.That(health.CurrentHealth, Is.EqualTo(75));
    Assert.That(health.IsAlive, Is.True);
}
```

**Test project location**: `GameTests/GameplayComponentsTests.cs` and `GameTests/Vector3Tests.cs`

## Creating a player

```csharp
var player = game.CreateEntity();
player.AddComponent(new Tag("Player"));
player.AddComponent(new Transform3D { Position = Vector3.Zero });
player.AddComponent(new Rigidbody3D { UseGravity = true });
player.AddComponent(new CollisionSphere { Radius = 1.0f });
player.AddComponent(new Health(100));
player.AddComponent(new PlayerController 
{ 
    Speed = 15f, 
    JumpForce = 15f,
    MaxSpeed = 25f,
    CanDoubleJump = true 
});
player.AddComponent(new MeshRenderer { MeshId = "player", MaterialId = "mat_player" });
```

## Creating an enemy

```csharp
var enemy = game.CreateEntity();
enemy.AddComponent(new Tag("Enemy"));
enemy.AddComponent(new Transform3D { Position = new Vector3(10, 0, 0) });
enemy.AddComponent(new Rigidbody3D { UseGravity = true });
enemy.AddComponent(new CollisionSphere { Radius = 0.8f });
enemy.AddComponent(new Health(50));
enemy.AddComponent(new DamageSource { Damage = 10, SourceTag = "enemy" });
enemy.AddComponent(new EnemyAI 
{ 
    DetectionRange = 25f,
    ChaseSpeed = 12f 
});
```

## Creating a puzzle switch

```csharp
var puzzleSwitch = game.CreateEntity();
puzzleSwitch.AddComponent(new Transform3D { Position = new Vector3(0, 0, 10) });
puzzleSwitch.AddComponent(new CollisionSphere { Radius = 0.5f, IsTrigger = true });
puzzleSwitch.AddComponent(new Interactable 
{ 
    OnInteract = (entity) => 
    {
        // Trigger custom behavior (open door, spawn enemy, etc.)
    }
});
```

## Camera system

The **CameraSystem** automatically:
- Follows the player entity with adjustable distance and height
- Updates rotation based on **mouse movement**
- **Locks mouse to screen** on startup (ESC to unlock for pause menu)
- Supports pitch/yaw clamping for natural camera feel

```csharp
// Create camera entity
var camera = game.CreateEntity();
camera.AddComponent(new Tag("Camera"));
camera.AddComponent(new Transform3D { Position = Vector3.Zero });
camera.AddComponent(new Camera { FieldOfView = 60f });
camera.AddComponent(new CameraController
{
    FollowTarget = player,           // Follow this entity
    Distance = 6f,                   // Distance behind player
    Height = 2f,                     // Height above player
    MouseSensitivity = 2f,           // Mouse look speed
    Pitch = -20f,                    // Initial pitch (degrees)
    MaxPitch = 60f,                  // Up limit
    MinPitch = -80f,                 // Down limit
    IsLockedToScreen = true          // Lock mouse at start
});
```

## Screen flash (damage feedback)

When the player takes damage, a **red overlay flashes across all monitors**:

```csharp
// Automatically triggered when player takes damage
// In CollisionSystem.TriggerDamageFlash():
var flash = new ScreenFlashEffect(
    duration: 0.2f,      // Fade over 0.2 seconds
    intensity: 0.7f      // 70% opacity at peak
)
{
    Color = "red"        // red, white, yellow, etc.
};
entity.AddComponent(flash);
```

**Engine integration required:**

```csharp
public void DrawScreenFlash(float intensity, string color = "red")
{
    // Render a full-screen quad or overlay
    // with color and opacity = intensity
    // This affects ALL monitors in a multi-monitor setup
    
    // Example (pseudo-code):
    // var overlay = new Quad(0 -> ScreenWidth, 0 -> ScreenHeight);
    // overlay.Color = color with alpha = intensity;
    // renderer.Draw(overlay);
}
```

## Retro Graphics System

Pixelated, color-reduced rendering like classic Sonic games. Multiple **presets** available:

### Presets

| Preset | Pixel Size | Colors | Scanlines | Style |
|--------|------------|--------|-----------|-------|
| **Sega Genesis** | 2x2 | 32K (5-bit) | Yes | Classic Sonic 3 |
| **Sega Master System** | 3x3 | 4K (4-bit) | Yes | Sonic 1 (Master System) |
| **Super Nintendo** | 2x2 | 32K (5-bit) | Yes | Similar to Genesis |
| **Arcade** | 4x4 | 4K (4-bit) | Yes | High contrast cabinet style |
| **Game Boy** | 4x4 | 4 colors | No | Monochrome handheld |
| **Minimal Pixel** | 2x2 | Full color | No | Clean pixelated look |

### Create with preset

```csharp
// Apply Sega Genesis style (default in GameSetup)
var retroEffect = game.CreateEntity();
retroEffect.AddComponent(RetroPresets.SegaGenesis);

// Or Master System style
retroEffect.AddComponent(RetroPresets.SegaMasterSystem);

// Or Game Boy style
retroEffect.AddComponent(RetroPresets.GameBoy);
```

### Custom retro effect

```csharp
var customEffect = new RetroGraphicsEffect
{
    PixelSize = 3,                  // 3x3 pixel blocks
    ColorBitsPerChannel = 4,        // Reduce to 4-bit color (4096 colors)
    EnableScanlines = true,         // CRT scanline effect
    ScanlineOpacity = 0.4f,         // 40% scanline darkness
    EnableDithering = true,         // Smooth color transitions
    EnableAspectCorrection = true   // Fix aspect ratio
};

entity.AddComponent(customEffect);
```

### Engine integration

The renderer must implement `ApplyRetroGraphics()`:

```csharp
public void ApplyRetroGraphics(RetroGraphicsEffect effect)
{
    // Pseudocode (implementation varies by engine)
    
    // 1. Create downsample render target
    var pixelated = DownsampleToPixels(
        renderTarget, 
        effect.PixelSize
    );
    
    // 2. Reduce color palette
    if (effect.ColorBitsPerChannel > 0)
        pixelated = ReduceColorPalette(
            pixelated, 
            effect.ColorBitsPerChannel
        );
    
    // 3. Apply dithering for smoother gradients
    if (effect.EnableDithering)
        pixelated = ApplyDithering(pixelated);
    
    // 4. Upscale back to screen resolution
    var upscaled = UpscaleNearest(pixelated, GetScreenWidth(), GetScreenHeight());
    
    // 5. Apply scanlines
    if (effect.EnableScanlines)
        upscaled = AddScanlines(upscaled, effect.ScanlineOpacity);
    
    // 6. Display
    RenderToScreen(upscaled);
}
```

## Input handling

Updated **IEngineInput** interface includes mouse support:

```csharp
public interface IEngineInput
{
    bool IsKeyDown(string key);           // "Space", "Shift", "E", "Escape"
    float GetAxis(string axis);           // "Horizontal", "Vertical"
    float GetMouseX();                    // Screen X position
    float GetMouseY();                    // Screen Y position
    void LockMouse(bool locked);          // Lock/unlock cursor
    bool IsMouseLocked { get; }           // Check if locked
}
```

**ESC key** toggles mouse lock for pause menu:
- **Locked** (default): Mouse hidden, camera follows, no cursor movement
- **Unlocked**: Cursor visible, can interact with UI

## Creating a camera

```csharp
var camera = game.CreateEntity();
camera.AddComponent(new Tag("Camera"));
camera.AddComponent(new Transform3D { Position = new Vector3(0, 2, -5) });
camera.AddComponent(new Camera { FieldOfView = 60f });
camera.AddComponent(new CameraController
{
    FollowTarget = player,
    Distance = 6f,
    Height = 2f,
    MouseSensitivity = 2f
});
```

## Engine bridge example (Unity)

```csharp
public class UnityInputBridge : IEngineInput
{
    private bool _mouseLocked = true;

    public bool IsKeyDown(string key) => Input.GetKey(key);
    
    public float GetAxis(string axis) => Input.GetAxis(axis);
    
    public float GetMouseX() => Input.mousePosition.x;
    
    public float GetMouseY() => Input.mousePosition.y;
    
    public void LockMouse(bool locked)
    {
        _mouseLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    }
    
    public bool IsMouseLocked => _mouseLocked;
}

public class UnityRendererBridge : IEngineRenderer
{
    private Color _flashColor;
    private float _flashIntensity;

    public void DrawSprite(string spriteId, float x, float y, float rot, 
        float scaleX = 1, float scaleY = 1, float opacity = 1)
    {
        // Draw 3D mesh at position
    }
    
    public void DrawScreenFlash(float intensity, string color = "red")
    {
        // Render full-screen overlay on all monitors
        _flashIntensity = intensity;
        _flashColor = color == "red" ? Color.red : Color.white;
        // Draw overlay rect covering screen space
    }
```

## Projectile system

**Rocks are thrown when dashing** — Player automatically throws rocks while sliding forward:

```csharp
// When player dashes (Shift + direction):
// - Rock spawns in front of player
// - Travels at 25 units/second for 7 seconds
// - Deals 15 damage + 20 knockback on hit
// - Inherits player's momentum

// In PlayerController.Dash():
playerController.OnDash = (playerEntity, dashDirection) =>
{
    SpawnDashRock(game, playerEntity, dashDirection);
};
```

Rock properties:
- **Speed**: 25 units/sec (inherits dash velocity)
- **Lifetime**: 7 seconds (auto-despawn)
- **Damage**: 15 HP
- **Knockback**: 20 force units
- **Size**: 0.5 radius sphere
- **Auto-spawn**: One rock per dash

## Collectibles

Automatically picked up when player is within 2m:

```csharp
// Ring (health + points)
var ring = game.CreateEntity();
ring.AddComponent(new Collectible(Collectible.CollectibleType.Ring, value: 10));
ring.AddComponent(new Transform3D { Position = new Vector3(5, 1, 0) });
ring.AddComponent(new CollisionSphere { Radius = 0.3f, IsTrigger = true });

// Speed boost power-up (10 second duration)
var speedItem = game.CreateEntity();
speedItem.AddComponent(new Collectible(Collectible.CollectibleType.SpeedBoost, value: 10f));

// Health restoration
var healthPickup = game.CreateEntity();
healthPickup.AddComponent(new Collectible(Collectible.CollectibleType.HealthPickup, value: 25));
```

Types:
- **Coin**: Points only
- **Ring**: Points + partial healing
- **HealthPickup**: Restore health
- **SpeedBoost**: 1.5x speed for duration
- **Shield**: Invincibility for duration
- **Invincibility**: Damage immunity for duration

## Level obstacles & hazards

### Hazards

Damage player on contact:

```csharp
// Spikes (20 damage)
var spikes = game.CreateEntity();
spikes.AddComponent(new Hazard(Hazard.HazardType.Spike, damage: 20));
spikes.AddComponent(new Transform3D { Position = Vector3.Zero });
spikes.AddComponent(new CollisionSphere { Radius = 1.0f, IsTrigger = true });
```

Types: Spike, Lava, Pit, Electricity, Freeze

### Moving platforms

Elevators, sliding platforms:

```csharp
var platform = game.CreateEntity();
platform.AddComponent(new MovingPlatform(
    startPosition: new Vector3(0, 0, 0),
    endPosition: new Vector3(10, 0, 0),
    speed: 5f
));
platform.AddComponent(new Transform3D { Position = Vector3.Zero });
platform.AddComponent(new Rigidbody3D { IsKinematic = true });
platform.AddComponent(new CollisionSphere { Radius = 2.0f });
```

### Conveyor belts

Push entities along direction:

```csharp
var conveyor = game.CreateEntity();
conveyor.AddComponent(new ConveyorBelt(
    direction: Vector3.Right,
    speed: 8f
));
```

### Trampolines

Bounce entities upward:

```csharp
var trampoline = game.CreateEntity();
trampoline.AddComponent(new Trampoline(bounceForce: 40f));
trampoline.AddComponent(new CollisionSphere { Radius = 1.0f });
```

## Power-ups

Temporary ability modifications:

```csharp
var speedPowerUp = new PowerUpEffect(PowerUpEffect.PowerUpType.SpeedBoost, duration: 10f);
player.AddComponent(speedPowerUp);
```

Types:
- **SpeedBoost**: 1.5x speed multiplier
- **Invincibility**: Damage immunity
- **DoubleJump**: Extra jump ability
- **Slow**: Reduced speed (debuff)

## Audio system

Complete audio system with sound effects, background music, volume control, and preloading. **See [AUDIO_SYSTEM_GUIDE.md](AUDIO_SYSTEM_GUIDE.md) for full documentation.**

### Quick start

```csharp
// Play sound effects
audioSystem.PlaySoundOnEntity(entity, "jump_sound", volume: 0.8f);

// Play background music
audioSystem.PlayBackgroundMusic("level_1_music", volume: 0.5f, fadeInTime: 2.0f);

// Control volume
var audioManager = game.Entities.First(e => e.TryGetComponent<AudioManager>(out _));
audioManager.MasterVolume = 0.8f;
audioManager.SFXVolume = 0.9f;
audioManager.MusicVolume = 0.5f;

// Queue sounds to play in sequence
var sounds = new[]
{
    new SoundEffect("powerup_activate", volume: 0.7f),
    new SoundEffect("powerup_loop", volume: 0.6f, delay: 0.5f),
};
audioSystem.QueueSounds(entity, sounds);
```

### Features

- **Sound Effects**: Play one-shot sounds with caching
- **Background Music**: Looping music with fade in/out transitions
- **Volume Control**: Master + separate SFX/Music channels
- **Audio Queuing**: Play multiple sounds sequentially
- **Preloading**: Load audio assets ahead of time for performance
- **Muting**: Mute all audio globally
- **Spatial Audio**: 3D audio positioning (extensible)
- **Audio Types**: SFX, Music, Voice, Ambient categorization

### Components

- **AudioSource**: Play individual sounds on entities
- **AudioManager**: Global audio settings & state
- **BackgroundMusic**: Background music with fade effects
- **SFXPlayer**: Queue-based sequential sound effects
- **AudioSettings**: Advanced properties (pitch, spatial audio, type)

### Integration

Sounds automatically triggered by game systems:
- **Collectibles**: `collect_coin`, `collect_ring`, `collect_healthpickup`
- **Hazards**: `hazard_spike`, `hazard_lava`, `hazard_pit`, etc.
- **Damage**: `player_hit`, `enemy_hit`, etc.
- **Dialogue**: Voice playback from DialogueSystem

### Engine bridge

```csharp
public interface IEngineAudio
{
    void PlaySound(string soundId, float volume = 1.0f);
    void StopSound(string soundId);
    void PlayBackgroundMusic(string musicId, float volume = 0.5f);
    void StopBackgroundMusic(float fadeOutTime = 0f);
    void SetMasterVolume(float volume);
    float GetMasterVolume();
    void SetSFXVolume(float volume);
    void SetMusicVolume(float volume);
    bool IsSoundPlaying(string soundId);
    void PreloadSound(string soundId);
}
```

### File organization

Audio files in Unity `Resources` folder:
```
Assets/Resources/Audio/
├── SFX/
│   ├── player_jump.ogg
│   ├── collect_ring.ogg
│   └── ...
├── Music/
│   ├── level_1.ogg
│   ├── boss_battle.ogg
│   └── ...
└── Voice/
    ├── npc_greeting.ogg
    └── ...
```

## Controls summary

## UI System

Complete in-game HUD and UI elements. **See [UI_SYSTEM_GUIDE.md](UI_SYSTEM_GUIDE.md) for full documentation.**

### Quick start

```csharp
// Create a health bar above the player
var healthBar = new HealthBar
{
    TargetEntity = player,
    Width = 200f,
    Height = 20f,
    Anchor = UIElement.AnchorPosition.TopLeft,
    Offset = new Vector3(20, 20, 0),
    FillColor = "green",
    ShowHealthText = true
};

var healthBarEntity = game.CreateEntity();
healthBarEntity.AddComponent(healthBar);

// Show damage numbers on hits
uiSystem.ShowDamageNumber(hitPosition, 25, isCritical: false);

// Show score popups on collection
uiSystem.ShowScorePopup(coinPosition, 10);

// Create HUD elements
var crosshair = game.CreateEntity();
crosshair.AddComponent(new Crosshair
{
    Style = "cross",
    Size = 10f,
    Anchor = UIElement.AnchorPosition.MiddleCenter
});
```

### Components

- **HealthBar**: Entity health with animated fill bar
- **ScoreDisplay**: Score counter with optional popups
- **DamageNumber**: Floating damage integers (auto-fade)
- **ScorePopup**: Floating score animations
- **TextDisplay**: Generic text UI
- **Crosshair**: Aiming reticle
- **Minimap**: World overview radar
- **InteractionPrompt**: "Press E" prompts
- **StatusEffectDisplay**: Active buff/debuff icons
- **UIManager**: Global UI settings

### Anchor Positions

Position UI elements on screen:

```
TopLeft      TopCenter      TopRight
  □          □              □

MiddleLeft   MiddleCenter   MiddleRight
  □          □              □

BottomLeft   BottomCenter   BottomRight
  □          □              □
```

### Features

- **Health bars** animate smoothly to target health
- **Damage numbers** float upward and fade out
- **Score popups** show collected values
- **Dynamic colors**: Health bar changes by health %
- **Crosshair styles**: cross, dot, circle, target
- **Minimap** shows entities in world range
- **Interaction prompts** appear above interactables
- **Pause-aware** UI can update when paused
- **Opacity control** for all UI elements

## Controls summary

| Input | Action |
|-------|--------|
| **WASD** | Move |
| **Space** | Jump (hold = double jump) |
| **Shift + Direction** | **Dash forward & throw rock** |
| **Mouse** | Camera look |
| **ESC** | Toggle mouse lock (pause) |
| **E** | Interact with puzzles |

## Roadmap

- [x] 3D physics & collision
- [x] Camera system with mouse look
- [x] Mouse lock for pause menu
- [x] Screen flash on all monitors (damage feedback)
- [x] Retro pixelated graphics (classic Sonic style)
- [x] Projectile/weapon system
- [x] Collectibles & power-ups
- [x] Level obstacles (hazards, platforms, trampolines, conveyors)
- [x] Audio system integration (sound effects, music, volume control)
- [x] UI system (health bars, score, damage numbers, crosshair, minimap)
- [x] Pause/main menus (pause menu shown in UI examples)
- [x] Save/Load game state
- [ ] Networking (multiplayer)
- [x] Unit tests

