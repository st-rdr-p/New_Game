using NUnit.Framework;
using GameCore;
using System.Collections.Generic;

namespace GameTests
{
    /// <summary>
    /// Tests for the Audio System - sound effects, music playback, volume, fade effects.
    /// </summary>
    [TestFixture]
    public class AudioSystemTests
    {
        private AudioSystem? _audioSystem;
        private MockEngineAudio? _mockAudio;

        [SetUp]
        public void Setup()
        {
            _mockAudio = new MockEngineAudio();
            _audioSystem = new AudioSystem(_mockAudio);
        }

        #region Audio Manager Tests

        [Test]
        public void AudioManager_InitializesWithDefaults()
        {
            // Arrange & Act
            var manager = new AudioManager();

            // Assert
            Assert.That(manager.MasterVolume, Is.EqualTo(1.0f));
            Assert.That(manager.SFXVolume, Is.EqualTo(1.0f));
            Assert.That(manager.MusicVolume, Is.EqualTo(1.0f));
            Assert.That(manager.IsMuted, Is.False);
        }

        [Test]
        public void AudioManager_SetVolumes()
        {
            // Arrange
            var manager = new AudioManager();

            // Act
            manager.MasterVolume = 0.8f;
            manager.SFXVolume = 0.6f;
            manager.MusicVolume = 0.7f;

            // Assert
            Assert.That(manager.MasterVolume, Is.EqualTo(0.8f));
            Assert.That(manager.SFXVolume, Is.EqualTo(0.6f));
            Assert.That(manager.MusicVolume, Is.EqualTo(0.7f));
        }

        [Test]
        public void AudioManager_MuteToggle()
        {
            // Arrange
            var manager = new AudioManager();

            // Act
            manager.IsMuted = true;

            // Assert
            Assert.That(manager.IsMuted, Is.True);
        }

        #endregion

        #region Background Music Tests

        [Test]
        public void BackgroundMusic_InitializesWithDefaults()
        {
            // Arrange & Act
            var bgMusic = new BackgroundMusic();

            // Assert
            Assert.That(bgMusic.TrackId, Is.Null.Or.Empty);
            Assert.That(bgMusic.Volume, Is.EqualTo(0.5f));
            Assert.That(bgMusic.IsPlaying, Is.False);
        }

        [Test]
        public void BackgroundMusic_SetProperties()
        {
            // Arrange
            var bgMusic = new BackgroundMusic();

            // Act
            bgMusic.TrackId = "ambient_theme";
            bgMusic.Volume = 0.7f;
            bgMusic.IsPlaying = true;

            // Assert
            Assert.That(bgMusic.TrackId, Is.EqualTo("ambient_theme"));
            Assert.That(bgMusic.Volume, Is.EqualTo(0.7f));
            Assert.That(bgMusic.IsPlaying, Is.True);
        }

        #endregion

        #region Audio Source Tests

        [Test]
        public void AudioSource_InitializesWithDefaults()
        {
            // Arrange & Act
            var audioSource = new AudioSource();

            // Assert
            Assert.That(audioSource.Volume, Is.EqualTo(1.0f));
            Assert.That(audioSource.IsPlaying, Is.False);
        }

        [Test]
        public void AudioSource_PlaySound()
        {
            // Arrange
            var audioSource = new AudioSource();

            // Act
            audioSource.ClipId = "sword_slash";
            audioSource.IsPlaying = true;

            // Assert
            Assert.That(audioSource.ClipId, Is.EqualTo("sword_slash"));
            Assert.That(audioSource.IsPlaying, Is.True);
        }

        #endregion

        #region Audio System Integration Tests

        [Test]
        public void AudioSystem_PlaySoundOnEntity()
        {
            // Arrange
            var entity = new Entity(1);
            string soundId = "footstep";

            // Act
            _audioSystem!.PlaySoundOnEntity(entity, soundId, 0.8f);

            // Assert
            Assert.That(_mockAudio!.LastPlayedSFX, Is.EqualTo(soundId));
            Assert.That(_mockAudio.LastSFXVolume, Is.GreaterThan(0f));
        }

        [Test]
        public void AudioSystem_PlayBackgroundMusic()
        {
            // Arrange
            string musicTrack = "exploration_music";

            // Act
            _audioSystem!.PlayBackgroundMusic(musicTrack, 0.6f, 1.0f);

            // Assert
            Assert.That(_mockAudio!.CurrentTrack, Is.EqualTo(musicTrack));
        }

        [Test]
        public void AudioSystem_StopBackgroundMusic()
        {
            // Arrange
            _audioSystem!.PlayBackgroundMusic("music1", 0.5f, 1.0f);

            // Act
            _audioSystem.StopBackgroundMusic(0.5f);

            // Assert
// Music stop is async, but we can check it was called
            Assert.That(_audioSystem.IsMusicPlaying, Is.False);
        }

        [Test]
        public void AudioSystem_CheckMusicPlayingState()
        {
            // Arrange
            string trackId = "battle_music";

            // Act
            _audioSystem!.PlayBackgroundMusic(trackId, 0.5f, 0f);
            bool isMusicPlaying = _audioSystem.IsMusicPlaying;

            // Assert
            Assert.That(isMusicPlaying, Is.True);
        }

        #endregion

        #region Audio System Update Tests

        [Test]
        public void AudioSystem_UpdateDoesNotThrow()
        {
            // Arrange
            var entities = new List<Entity>();

            // Act & Assert
            Assert.DoesNotThrow(() => _audioSystem!.Update(0.016f, entities));
        }

        [Test]
        public void AudioSystem_ProcessAudioComponents()
        {
            // Arrange
            var entity = new Entity(1);
            var audioSource = new AudioSource { ClipId = "test_sound", IsPlaying = true };
            entity.AddComponent(audioSource);

            var entities = new List<Entity> { entity };

            // Act
            _audioSystem!.Update(0.016f, entities);

            // Assert
            Assert.That(entity.TryGetComponent<AudioSource>(out var retrieved), Is.True);
            Assert.That(retrieved!.ClipId, Is.EqualTo("test_sound"));
        }

        #endregion

        #region Queue Sounds Tests

        [Test]
        public void AudioSystem_QueueMultipleSounds()
        {
            // Arrange
            var entity = new Entity(2);
            var sound1 = new SoundEffect("sfx1", 1.0f);
            var sound2 = new SoundEffect("sfx2", 0.8f);

            // Act
            _audioSystem!.QueueSounds(entity, sound1, sound2);

            // Assert
            Assert.That(entity.TryGetComponent<SFXPlayer>(out var player), Is.True);
            Assert.That(player!.SoundQueue.Count, Is.EqualTo(2));
        }

        #endregion

        #region Integration Tests

        [Test]
        public void Audio_CompleteAudioFlow()
        {
            // Arrange - Create entities with audio
            var entity = new Entity(1);

            // Act - Play music and sound
            _audioSystem!.PlayBackgroundMusic("ambient", 0.5f, 1.0f);
            _audioSystem.PlaySoundOnEntity(entity, "effect1", 0.7f);

            // Assert
            Assert.That(_audioSystem.IsMusicPlaying, Is.True);
            Assert.That(_mockAudio!.CurrentTrack, Is.EqualTo("ambient"));
            Assert.That(_mockAudio.LastPlayedSFX, Is.EqualTo("effect1"));
        }

        [Test]
        public void Audio_MultipleAudioSources()
        {
            // Arrange
            var entity1 = new Entity(1);
            var entity2 = new Entity(2);

            // Act
            _audioSystem!.PlaySoundOnEntity(entity1, "sound1", 1.0f);
            _audioSystem.PlaySoundOnEntity(entity2, "sound2", 0.5f);

            // Assert
            Assert.That(_mockAudio!.LastPlayedSFX, Is.EqualTo("sound2"));
        }

        #endregion
    }

    /// <summary>
    /// Mock implementation of IEngineAudio for testing.
    /// </summary>
    public class MockEngineAudio : IEngineAudio
    {
        public string? CurrentTrack { get; set; }
        public string? LastPlayedSFX { get; set; }
        public float LastSFXVolume { get; set; } = 1.0f;
        public float MasterVolume { get; set; } = 1.0f;
        private List<string> _activeSounds = new List<string>();

        public void PlaySound(string id, float volume)
        {
            LastPlayedSFX = id;
            LastSFXVolume = volume;
            if (!_activeSounds.Contains(id))
                _activeSounds.Add(id);
        }

        public void StopSound(string id)
        {
            _activeSounds.Remove(id);
        }

        public void PlayBackgroundMusic(string trackId, float volume)
        {
            CurrentTrack = trackId;
            MasterVolume = volume;
        }

        public void StopBackgroundMusic(float fadeTime)
        {
            CurrentTrack = null;
        }

        public float GetMasterVolume()
        {
            return MasterVolume;
        }

        public bool IsSoundPlaying(string id)
        {
            return _activeSounds.Contains(id);
        }

        public void PreloadSound(string id)
        {
            // Mock implementation - just track that it was called
        }

        public void SetMasterVolume(float volume) => MasterVolume = volume;
        public void SetSFXVolume(float volume) { }
        public void SetMusicVolume(float volume) { }
    }
}
