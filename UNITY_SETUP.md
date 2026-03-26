# Unity Integration Guide

This guide explains how to integrate the GameCore into a Unity project.

## Setup Instructions

### 1. Project Structure

```
Assets/
├── Scripts/
│   ├── GameCore/          (Copy src/GameCore/*.cs here)
│   └── Unity/             (Copy src/Unity/*.cs here)
├── Resources/
│   ├── Meshes/            (game object prefabs)
│   ├── Audio/             (audio clips)
│   └── Materials/         (textures/materials)
├── Shaders/
│   └── RetroPixelate.shader
└── Scenes/
    └── GameScene.unity
```

### 2. Project Setup Steps

#### Step 1: Copy Source Files

1. Copy all files from `src/GameCore/` to `Assets/Scripts/GameCore/`
2. Copy all files from `src/Unity/` to `Assets/Scripts/Unity/`
3. Copy `RetroPixelate.shader` to `Assets/Shaders/`

#### Step 2: Create Folder Structure

In Project window, create folders:
- `Assets/Resources/Meshes/`
- `Assets/Resources/Audio/`
- `Assets/Resources/Materials/`

#### Step 3: Configure Input Manager

Open `Edit > Project Settings > Input Manager` and verify these axes exist:

- **Horizontal**: WASD / Arrow Keys / Gamepad Left Stick X
- **Vertical**: WASD / Arrow Keys / Gamepad Left Stick Y

Or add custom mappings:
```
Horizontal:
  - Positive: D / Right Arrow / Gamepad A Stick Right
  - Negative: A / Left Arrow / Gamepad A Stick Left

Vertical:
  - Positive: W / Up Arrow / Gamepad A Stick Forward
  - Negative: S / Down Arrow / Gamepad A Stick Back
```

#### Step 4: Create Game Scene

1. Create a new scene: `Assets/Scenes/GameScene.unity`
2. Add to scene:
   - **Main Camera** (or use existing)
   - **Empty GameObject** named "GameManager"
   - **Canvas** (for UI and screen flash effects)

3. Add **SonicLikeGameManager** script to GameManager:
   - Select GameManager
   - Drag `SonicLikeGameManager.cs` onto it
   - Check "Auto Initialize" in Inspector

#### Step 5: Create Asset Resources

**Create Mesh Prefabs:**

For each entity type, create a prefab in `Assets/Resources/Meshes/`:
- `player_mesh.prefab`
- `enemy_mesh.prefab`
- `rock_mesh.prefab`

Example player prefab:
```
GameObject: Player Model
├── MeshFilter (cube, sphere, or custom model)
├── MeshRenderer
└── Material (Albedo color set)
```

**Create Audio Clips:**

Place audio files in `Assets/Resources/Audio/`:
- `collect_ring.mp3`
- `collect_coin.mp3`
- `collect_healthpickup.mp3`
- `hazard_spike.mp3`
- `hazard_lava.mp3`
- `player_hit.mp3`

**Create Materials:**

Create materials in `Assets/Resources/Materials/`:
- `player_material.mat`
- `enemy_material.mat`
- `rock_material.mat`

### 3. Gameplay Instructions

**Controls:**
- **WASD** - Move
- **Space** - Jump (press again for double jump)
- **Shift + Direction** - Dash and throw rock
- **Mouse** - Look around (camera follows)
- **ESC** - Unlock mouse (pause menu)
- **E** - Interact with puzzles

**Game Mechanics:**
- Collect rings for health and points
- Avoid hazards (spikes, lava)
- Use platforms and trampolines for navigation
- Defeat enemies by dashing into them
- Interact with switches to solve puzzles

### 4. Customization

#### Change Player Speed

In `SonicLikeGameManager`:
```csharp
var playerController = player.GetComponent<PlayerController>();
playerController.Speed = 20f;           // Adjust movement speed
playerController.MaxSpeed = 30f;        // Max velocity
playerController.JumpForce = 20f;       // Jump height
```

#### Adjust Retro Graphics Preset

In `GameSetup.SetupSonicLikeGame()`:
```csharp
// Change retro graphics preset
retroEffect.AddComponent(RetroPresets.SegaMasterSystem);  // More pixelated
retroEffect.AddComponent(RetroPresets.SuperNintendo);     // 16-bit look
retroEffect.AddComponent(RetroPresets.GameBoy);           // Monochrome
```

#### Customize Collectibles

In level setup:
```csharp
// Create custom collectible
var collectible = game.CreateEntity();
collectible.AddComponent(new Collectible(
    Collectible.CollectibleType.Ring, 
    value: 10
));
```

#### Add More Hazards

```csharp
// Create custom hazard
var hazard = game.CreateEntity();
hazard.AddComponent(new Hazard(
    Hazard.HazardType.Lava, 
    damage: 25f
));
```

### 5. Building for Different Platforms

**PC/Mac:**
- Build Settings: Standalone
- Target resolution: 1920x1080 (or higher)
- Graphics quality: High

**WebGL:**
- Build Settings: WebGL
- Graphics quality: Medium
- Compression: Enabled

**Mobile:**
- Build Settings: iOS/Android
- Resolution: 1080x1920
- Touch input mapping (optional)

### 6. Performance Tips

1. **Cache Assets**: Preload meshes and audio in inspector
2. **Draw Call Batching**: Use static batching for platforms
3. **Physics**: Reduce collision checks if game lags
4. **Graphics**: Lower pixel size (4 → 8) for faster pixelation
5. **Build**: Use release build (not debug) for production

### 7. Debugging

**Enable Debug Output:**
```csharp
// In SonicLikeGameManager.cs
Debug.Log("Game initialized successfully!");
```

**Inspector Debugging:**
- Select GameManager
- View Game instance in Debug Inspector
- Inspect entity components in real-time

**Performance Profiling:**
- Window > Analysis > Profiler
- Check CPU usage, memory, draw calls

### 8. Common Issues

**Q: Audio not playing**
- A: Ensure audio files are in `Resources/Audio/` with correct naming
- Check AudioListener exists in scene

**Q: Meshes not displaying**
- A: Verify prefabs in `Resources/Meshes/` folder
- Check mesh IDs match in GameSetup

**Q: Input not working**
- A: Verify Input Manager has Horizontal/Vertical axes
- Check keyboard mappings in Input Settings

**Q: Pixelation not showing**
- A: Ensure RetroPixelate.shader is in `Assets/Shaders/`
- Check shader name: `Hidden/RetroPixelate`

### 9. Next Steps

1. **Add UI**: Create pause menu, HUD with health/score
2. **Level Design**: Create custom level layouts
3. **Sound Design**: Add background music and SFX
4. **Polish**: Add animations, particle effects, trails
5. **Optimization**: Profile and optimize for target platform

---

**Quick Start Checklist:**
- [ ] Copy src files to Assets/Scripts/
- [ ] Create Resources folders
- [ ] Create game scene
- [ ] Add SonicLikeGameManager to GameManager
- [ ] Create mesh prefabs
- [ ] Create audio clips
- [ ] Run scene in Play mode

Enjoy your Sonic-like game! 🎮
