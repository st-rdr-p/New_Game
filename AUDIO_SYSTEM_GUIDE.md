# Audio System Integration Guide

## Overview

The audio system provides a complete, engine-agnostic audio solution for the game. It handles sound effects, background music, volume control, and audio management through an extensible component-based architecture.

## Architecture

### Core Components

1. **IEngineAudio Interface** - Engine bridge for audio operations
   - Play/stop sounds
   - Background music management
   - Volume control
   - Preloading assets

2. **AudioSystem** - Main system that processes audio entities
   - Manages audio-related entity updates
   - Handles background music transitions
   - Applies volume levels
   - Processes sound queues

3. **UnityAudioBridge** - Unity implementation of IEngineAudio
   - Creates and manages AudioSources
   - Audio caching for performance
   - Volume multipliers

### Audio Components

- **AudioSource** - Entity component for playing individual sounds
- **BackgroundMusic** - Manages background music with fade effects
- **AudioManager** - Global audio settings (master volume, muting, etc.)
- **SFXPlayer** - Queue-based sound effect player
- **AudioListener** - Spatial audio positioning
- **AudioSettings** - Advanced audio properties (pitch, spatial audio, type)

## Quick Start

### 1. Initialize Game with Audio

```csharp
// Create bridges
var inputBridge = new UnityInputBridge();
var rendererBridge = new UnityRendererBridge();
var audioBridge = new UnityAudioBridge();

// Create game with audio support
var game = new GameCore.Game(inputBridge, audioBridge);

// Setup game
GameSetup.SetupSonicLikeGame(game, inputBridge, rendererBridge, audioBridge);
```

### 2. Play Sound Effects

```csharp
var audioSystem = game.Entities
    .First(e => e.TryGetComponent<AudioManager>(out _));

// Play a sound on an entity
audioSystem.PlaySoundOnEntity(entity, "jump_sound", volume: 0.8f);
```

### 3. Play Background Music

```csharp
// Play with fade-in
audioSystem.PlayBackgroundMusic("level_1_music", volume: 0.5f, fadeInTime: 2.0f);

// Stop with fade-out
audioSystem.StopBackgroundMusic(fadeOutTime: 1.0f);
```

### 4. Control Volume

```csharp
// Find audio manager
var audioManager = game.Entities
    .First(e => e.TryGetComponent<AudioManager>(out var mgr) && mgr.IsInitialized);

// Set volumes
audioManager.MasterVolume = 0.8f;  // 80% volume
audioManager.SFXVolume = 0.9f;     // Sound effects at 90%
audioManager.MusicVolume = 0.5f;   // Music at 50%

// Mute/unmute
audioManager.IsMuted = true;
```

## Usage Patterns

### Preloading Audio

For better performance, preload audio files before they're needed:

```csharp
// Preload individual sounds
audio.PreloadSound("jump_sound");
audio.PreloadSound("collect_ring");

// Preload multiple sounds
audio.PreloadSounds("jump_sound", "land_sound", "dash_sound");

// Preload a folder (if using UnityAudioBridge)
var bridge = (UnityAudioBridge)audio;
bridge.PreloadAudioFolder("Audio/Level_1");
```

### Queue Sound Effects

Play multiple sounds in sequence:

```csharp
var sfxEntity = game.CreateEntity();

var sounds = new[]
{
    new SoundEffect("powerup_activate", volume: 0.7f, delay: 0f),
    new SoundEffect("powerup_loop", volume: 0.6f, delay: 0.5f),
    new SoundEffect("powerup_end", volume: 0.5f, delay: 1.5f),
};

audioSystem.QueueSounds(sfxEntity, sounds);
```

### Dialog/Voice Audio

```csharp
var dialogueEntity = game.CreateEntity();
dialogueEntity.AddComponent(new AudioSettings
{
    Type = AudioSettings.AudioType.Voice,
    UseSpatialAudio = true,
    MaxDistance = 50f
});

audioSystem.PlaySoundOnEntity(dialogueEntity, "npc_greeting", volume: 0.8f);
```

### Sound Events with Callbacks

```csharp
var entity = game.CreateEntity();
var source = entity.AddComponent(new AudioSource
{
    ClipId = "explosion",
    Volume = 1.0f,
    IsPlaying = true
});

// Set callback for when sound finishes
source.OnSoundFinished = () =>
{
    Debug.Log("Explosion sound finished!");
    // Trigger post-sound effects
};
```

## Audio File Organization

Audio files should be organized in Unity's `Resources` folder:

```
Assets/Resources/
└── Audio/
    ├── SFX/
    │   ├── player_jump.ogg
    │   ├── player_land.ogg
    │   ├── collect_ring.ogg
    │   └── ...
    ├── Music/
    │   ├── level_1.ogg
    │   ├── level_2.ogg
    │   ├── boss_battle.ogg
    │   └── ...
    └── Voice/
        ├── npc_greeting.ogg
        ├── dialogue_line_1.ogg
        └── ...
```

Then reference them by ID:
```csharp
audio.PlaySound("SFX/player_jump");      // Play jump sound
audio.PlayBackgroundMusic("Music/level_1"); // Play background music
```

## Volume Management

### Master Volume
Controls all audio globally (0-1):
```csharp
audioManager.MasterVolume = 0.5f;  // 50% of all audio
```

### Channel Volumes
Separate control for different audio types:
```csharp
audioManager.SFXVolume = 0.9f;     // Effects at 90%
audioManager.MusicVolume = 0.5f;   // Music at 50%
```

### Muting
```csharp
audioManager.IsMuted = true;   // Mute all audio
audioManager.IsMuted = false;  // Unmute
```

## Audio System Updates

The AudioSystem processes entities each frame:

1. **Frame 0**: Initialize audio manager if needed
2. **Every Frame**:
   - Update volume levels
   - Update music fade effects
   - Process audio sources
   - Process background music
   - Process SFX queues

## Advanced Features

### Spatial Audio
Position audio in 3D space based on entity transform:

```csharp
var settings = entity.AddComponent(new AudioSettings
{
    UseSpatialAudio = true,
    MaxDistance = 50f  // Audible within 50 units
});
```

### Audio Pitch
Adjust playback speed/pitch:

```csharp
var settings = entity.AddComponent(new AudioSettings
{
    Pitch = 1.5f  // Play 50% faster
});
```

### Audio Type Classification
Organize audio by type:

```csharp
var settings = new AudioSettings
{
    Type = AudioSettings.AudioType.Voice  // or SFX, Music, Ambient
};
```

## Extending the System

### Custom Audio Bridge

Implement `IEngineAudio` for different engines:

```csharp
public class GodotAudioBridge : IEngineAudio
{
    public void PlaySound(string soundId, float volume = 1.0f)
    {
        // Godot implementation
    }
    // ... implement other methods
}
```

### Custom Audio Components

Create specialized audio components:

```csharp
public class DialogueAudio : Component
{
    public List<string> LineClipIds { get; set; }
    public int CurrentLine { get; set; }
    public float TimeBetweenLines { get; set; } = 1.0f;
}
```

## Troubleshooting

### No Sound Playing
1. Verify audio files exist in `Assets/Resources/Audio/`
2. Check that audio format is supported (OGG, WAV, MP3)
3. Ensure `AudioListener` is in the scene
4. Verify volume levels aren't set to 0

### Audio Stuttering
1. Preload frequently used audio files
2. Use compressed audio formats (OGG preferred)
3. Adjust audio source pool size
4. Check for memory leaks in audio callbacks

### Volume Not Changing
1. Verify `AudioManager` entity exists in game
2. Ensure volume values are between 0-1
3. Check that `IsMuted` is false
4. Verify master volume multiplier is not 0

## Performance Tips

1. **Preload audio** - Load frequently used sounds before gameplay
2. **Use compressed formats** - OGG reduces memory usage
3. **Limit concurrent sounds** - Pool audio sources in production
4. **Cache audio clips** - The system caches loaded clips automatically
5. **Fade music** - Smooth transitions rather than abrupt stops

## Examples

See [AudioExample.cs](AudioExample.cs) for complete usage examples including:
- Playing simple sound effects
- Background music management
- Volume control
- Sound queuing
- Collision sounds
- UI sounds
- Dialogue audio
- Level audio preloading
- Complete game initialization

## Integration with Other Systems

### Collectible Pickup
```csharp
// Already integrated! CollectibleSystem uses audio
// Plays: "collect_{collectible_type}"
```

### Hazard Damage
```csharp
// Already integrated! HazardSystem uses audio
// Plays damage/hit sounds on collision
```

### Dialogue
```csharp
// Already integrated! DialogueSystem uses audio
// Plays dialogue voice lines from audio files
```
