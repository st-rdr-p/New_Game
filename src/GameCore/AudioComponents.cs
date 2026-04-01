using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// Component for entities that can play sound effects.
    /// </summary>
    public class AudioSource : Component
    {
        /// <summary>
        /// ID of the audio clip to play.
        /// </summary>
        public string ClipId { get; set; }

        /// <summary>
        /// Volume of the sound (0-1).
        /// </summary>
        public float Volume { get; set; } = 1.0f;

        /// <summary>
        /// Whether the sound should loop.
        /// </summary>
        public bool Loop { get; set; }

        /// <summary>
        /// Whether the sound is currently playing.
        /// </summary>
        public bool IsPlaying { get; set; }

        /// <summary>
        /// Callback when sound finishes playing.
        /// </summary>
        public Action OnSoundFinished { get; set; }
    }

    /// <summary>
    /// Component for background music management.
    /// </summary>
    public class BackgroundMusic : Component
    {
        /// <summary>
        /// ID of the background music track.
        /// </summary>
        public string TrackId { get; set; }

        /// <summary>
        /// Volume of the background music (0-1).
        /// </summary>
        public float Volume { get; set; } = 0.5f;

        /// <summary>
        /// Whether the music is currently playing.
        /// </summary>
        public bool IsPlaying { get; set; }

        /// <summary>
        /// Fade-in time when starting the music.
        /// </summary>
        public float FadeInTime { get; set; } = 1.0f;

        /// <summary>
        /// Fade-out time when stopping the music.
        /// </summary>
        public float FadeOutTime { get; set; } = 1.0f;

        /// <summary>
        /// Current fade progress (0-1).
        /// </summary>
        public float FadeProgress { get; set; }
    }

    /// <summary>
    /// Manager component for the audio system state.
    /// </summary>
    public class AudioManager : Component
    {
        /// <summary>
        /// Master volume (0-1).
        /// </summary>
        public float MasterVolume { get; set; } = 1.0f;

        /// <summary>
        /// SFX volume multiplier (0-1).
        /// </summary>
        public float SFXVolume { get; set; } = 1.0f;

        /// <summary>
        /// Music volume multiplier (0-1).
        /// </summary>
        public float MusicVolume { get; set; } = 1.0f;

        /// <summary>
        /// Mute all audio.
        /// </summary>
        public bool IsMuted { get; set; }

        /// <summary>
        /// Whether the audio system is initialized.
        /// </summary>
        public bool IsInitialized { get; set; }
    }

    /// <summary>
    /// Sound effect player component - plays one-shot sound effects.
    /// </summary>
    public class SFXPlayer : Component
    {
        /// <summary>
        /// Queue of sound effects to play.
        /// </summary>
        public Queue<SoundEffect> SoundQueue { get; set; } = new();

        /// <summary>
        /// Currently playing sound effect.
        /// </summary>
        public SoundEffect CurrentSound { get; set; }
    }

    /// <summary>
    /// Represents a single sound effect in the queue.
    /// </summary>
    public class SoundEffect
    {
        public string ClipId { get; set; }
        public float Volume { get; set; } = 1.0f;
        public float Delay { get; set; } = 0f;
        public Action OnComplete { get; set; }

        public SoundEffect(string clipId, float volume = 1.0f, float delay = 0f)
        {
            ClipId = clipId;
            Volume = volume;
            Delay = delay;
        }
    }

    /// <summary>
    /// Listener component for positioning audio in 3D space.
    /// </summary>
    public class AudioListener : Component
    {
        /// <summary>
        /// Whether this is the active listener for spatial audio.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Forward direction for audio listener (for spatial audio).
        /// </summary>
        public Vector3 Forward { get; set; } = Vector3.Forward;

        /// <summary>
        /// Up direction for audio listener (for spatial audio).
        /// </summary>
        public Vector3 Up { get; set; } = Vector3.Up;
    }

    /// <summary>
    /// Component for audio settings per entity (e.g., dialogue audio).
    /// </summary>
    public class AudioSettings : Component
    {
        public enum AudioType
        {
            SFX,
            Music,
            Voice,
            Ambient
        }

        /// <summary>
        /// Type of audio being played.
        /// </summary>
        public AudioType Type { get; set; } = AudioType.SFX;

        /// <summary>
        /// Pitch/speed of playback (1.0 = normal).
        /// </summary>
        public float Pitch { get; set; } = 1.0f;

        /// <summary>
        /// Fade in/out duration in seconds.
        /// </summary>
        public float FadeDuration { get; set; } = 0f;

        /// <summary>
        /// Whether to use spatial audio (3D positioning).
        /// </summary>
        public bool UseSpatialAudio { get; set; } = false;

        /// <summary>
        /// Maximum distance for spatial audio (3D).
        /// </summary>
        public float MaxDistance { get; set; } = 100f;
    }
}
