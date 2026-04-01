using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// Example demonstrating how to use the audio system.
    /// Shows:
    /// - Playing sound effects on entities
    /// - Background music management
    /// - Volume control
    /// - Sound queuing
    /// </summary>
    public class AudioExample
    {
        /// <summary>
        /// Example 1: Play a simple sound effect
        /// </summary>
        public static void PlaySimpleSound(AudioSystem audioSystem, Entity entity)
        {
            audioSystem.PlaySoundOnEntity(entity, "jump_sound", volume: 0.8f);
        }

        /// <summary>
        /// Example 2: Play background music
        /// </summary>
        public static void PlayBackgroundMusic(AudioSystem audioSystem, string levelName)
        {
            // Stop any existing music
            audioSystem.StopBackgroundMusic(fadeOutTime: 1.0f);

            // Play new background music with fade-in
            audioSystem.PlayBackgroundMusic(
                musicId: $"level_{levelName}_music",
                volume: 0.5f,
                fadeInTime: 2.0f
            );
        }

        /// <summary>
        /// Example 3: Set volume levels
        /// </summary>
        public static void SetAudioLevels(Game game)
        {
            // Find the audio manager entity
            foreach (var entity in game.Entities)
            {
                if (entity.TryGetComponent<AudioManager>(out var audioManager))
                {
                    audioManager.MasterVolume = 0.8f;  // Volume reduction for all audio
                    audioManager.SFXVolume = 0.9f;     // Slightly louder sound effects
                    audioManager.MusicVolume = 0.5f;   // Quieter background music
                    return;
                }
            }
        }

        /// <summary>
        /// Example 4: Queue multiple sounds to play in sequence
        /// </summary>
        public static void QueueSoundEffects(AudioSystem audioSystem, Entity entity)
        {
            var sounds = new[]
            {
                new SoundEffect("powerup_activate", volume: 0.7f, delay: 0f),
                new SoundEffect("powerup_loop", volume: 0.6f, delay: 0.5f),
                new SoundEffect("powerup_fadeout", volume: 0.5f, delay: 1.5f),
            };

            audioSystem.QueueSounds(entity, sounds);
        }

        /// <summary>
        /// Example 5: Play collision sound with physics feedback
        /// </summary>
        public static void OnCollisionWithEnemy(AudioSystem audioSystem, Entity player, Entity enemy)
        {
            // Play hit sound
            audioSystem.PlaySoundOnEntity(player, "player_hit", volume: 0.9f);

            // Could also play different enemy sound
            audioSystem.PlaySoundOnEntity(enemy, "enemy_hit", volume: 0.7f);
        }

        /// <summary>
        /// Example 6: Create a UI sound manager for menu clicks, etc.
        /// </summary>
        public static void SetupUISounds(Game game, AudioSystem audioSystem)
        {
            var uiSoundManager = game.CreateEntity();
            uiSoundManager.AddComponent(new Tag("UIAudioManager"));
            uiSoundManager.AddComponent(new SFXPlayer());

            // Queue button click sounds
            var uiSounds = new[]
            {
                new SoundEffect("ui_click", volume: 0.5f),
                new SoundEffect("ui_hover", volume: 0.3f),
                new SoundEffect("ui_select", volume: 0.6f),
            };

            audioSystem.QueueSounds(uiSoundManager, uiSounds);
        }

        /// <summary>
        /// Example 7: Mute/unmute functionality
        /// </summary>
        public static void ToggleMute(Game game)
        {
            foreach (var entity in game.Entities)
            {
                if (entity.TryGetComponent<AudioManager>(out var audioManager))
                {
                    audioManager.IsMuted = !audioManager.IsMuted;
                    return;
                }
            }
        }

        /// <summary>
        /// Example 8: Dialogue playback with audio
        /// </summary>
        public static void PlayDialogueWithAudio(
            AudioSystem audioSystem,
            Entity dialogueEntity,
            string voiceClipId,
            string dialogueText,
            float duration)
        {
            // Add dialogue audio component
            dialogueEntity.AddComponent(new AudioSettings
            {
                Type = AudioSettings.AudioType.Voice,
                UseSpatialAudio = true,
                MaxDistance = 50f
            });

            // Play voice audio
            audioSystem.PlaySoundOnEntity(dialogueEntity, voiceClipId, volume: 0.8f);
        }

        /// <summary>
        /// Example 9: Preload audio for a level
        /// </summary>
        public static void PreloadLevelAudio(IEngineAudio audio, string levelName)
        {
            // Preload all sound effects for the level
            audio.PreloadSound($"level_{levelName}_music");
            audio.PreloadSound("player_jump");
            audio.PreloadSound("player_land");
            audio.PreloadSound("collect_ring");
            audio.PreloadSound("collect_coin");
            audio.PreloadSound("enemy_hit");
            audio.PreloadSound("player_hit");
            audio.PreloadSound("powerup_activate");
        }

        /// <summary>
        /// Example 10: Complete game initialization with audio
        /// </summary>
        public static void InitializeGameWithAudio(
            IEngineInput input,
            IEngineRenderer renderer,
            IEngineAudio audio)
        {
            // Create game with audio support
            var game = new Game(input, audio);

            // Setup game with all systems including audio
            GameSetup.SetupSonicLikeGame(game, input, renderer, audio);

            // Preload audio assets
            PreloadLevelAudio(audio, "level_1");

            // Find audio system
            var audioSystem = null as AudioSystem; // Would need to store reference during setup

            // Play background music
            PlayBackgroundMusic(audioSystem, "level_1");

            // Set initial audio levels
            SetAudioLevels(game);
        }
    }
}
