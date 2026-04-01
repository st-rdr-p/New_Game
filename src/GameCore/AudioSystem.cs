using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// Audio system - manages sound effects, background music, and volume control.
    /// Processes audio components and coordinates with the IEngineAudio bridge.
    /// </summary>
    public class AudioSystem : ISystem
    {
        private readonly IEngineAudio _audio;
        private AudioManager _audioManager;
        private string _currentMusicId;
        private float _musicFadeTimer;
        private float _musicFadeDuration;
        private bool _isFadingOut;

        public AudioSystem(IEngineAudio audio)
        {
            _audio = audio ?? throw new ArgumentNullException(nameof(audio));
        }

        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            var entityList = new List<Entity>(entities);

            // Initialize audio manager if it doesn't exist
            if (_audioManager == null)
            {
                foreach (var entity in entityList)
                {
                    if (entity.TryGetComponent<AudioManager>(out var manager))
                    {
                        _audioManager = manager;
                        break;
                    }
                }

                if (_audioManager == null)
                {
                    // Create default audio manager
                    var managerEntity = new Entity(int.MaxValue);
                    _audioManager = new AudioManager { IsInitialized = true };
                    managerEntity.AddComponent(_audioManager);
                }
            }

            // Update volume levels
            UpdateVolumeLevels();

            // Process background music fade
            UpdateMusicFade(deltaTime);

            // Process audio sources
            foreach (var entity in entityList)
            {
                if (entity.TryGetComponent<AudioSource>(out var audioSource))
                {
                    UpdateAudioSource(audioSource, deltaTime);
                }

                if (entity.TryGetComponent<BackgroundMusic>(out var bgMusic))
                {
                    UpdateBackgroundMusic(bgMusic, deltaTime);
                }

                if (entity.TryGetComponent<SFXPlayer>(out var sfxPlayer))
                {
                    UpdateSFXPlayer(sfxPlayer, deltaTime);
                }
            }
        }

        /// <summary>
        /// Play a sound effect on an entity.
        /// </summary>
        public void PlaySoundOnEntity(Entity entity, string soundId, float volume = 1.0f)
        {
            // Get or create audio source
            if (!entity.TryGetComponent<AudioSource>(out var audioSource))
            {
                audioSource = entity.AddComponent(new AudioSource());
            }

            audioSource.ClipId = soundId;
            audioSource.Volume = volume * _audioManager.SFXVolume * _audioManager.MasterVolume;
            audioSource.IsPlaying = true;

            _audio.PlaySound(soundId, audioSource.Volume);
        }

        /// <summary>
        /// Play background music.
        /// </summary>
        public void PlayBackgroundMusic(string musicId, float volume = 0.5f, float fadeInTime = 1.0f)
        {
            if (_currentMusicId == musicId && _audioManager != null)
            {
                // Music is already playing
                return;
            }

            // Stop current music if playing
            if (_currentMusicId != null)
            {
                _audio.StopBackgroundMusic(fadeInTime);
            }

            _currentMusicId = musicId;
            _isFadingOut = false;
            _musicFadeTimer = 0;
            _musicFadeDuration = fadeInTime;

            var effectiveVolume = volume * _audioManager.MusicVolume * _audioManager.MasterVolume;
            _audio.PlayBackgroundMusic(musicId, effectiveVolume);
        }

        /// <summary>
        /// Stop background music.
        /// </summary>
        public void StopBackgroundMusic(float fadeOutTime = 0.5f)
        {
            if (!IsMusicPlaying)
                return;

            _isFadingOut = true;
            _musicFadeTimer = 0;
            _musicFadeDuration = fadeOutTime;

            _audio.StopBackgroundMusic(fadeOutTime);
            _currentMusicId = null;
        }

        /// <summary>
        /// Check if music is currently playing.
        /// </summary>
        public bool IsMusicPlaying => !string.IsNullOrEmpty(_currentMusicId);

        /// <summary>
        /// Queue multiple sound effects to play in sequence.
        /// </summary>
        public void QueueSounds(Entity entity, params SoundEffect[] sounds)
        {
            if (!entity.TryGetComponent<SFXPlayer>(out var player))
            {
                player = entity.AddComponent(new SFXPlayer());
            }

            foreach (var sound in sounds)
            {
                player.SoundQueue.Enqueue(sound);
            }
        }

        private void UpdateVolumeLevels()
        {
            if (_audioManager.IsMuted)
            {
                _audio.SetMasterVolume(0f);
            }
            else
            {
                _audio.SetMasterVolume(_audioManager.MasterVolume);
                _audio.SetSFXVolume(_audioManager.SFXVolume);
                _audio.SetMusicVolume(_audioManager.MusicVolume);
            }
        }

        private void UpdateMusicFade(float deltaTime)
        {
            if (_musicFadeDuration <= 0)
                return;

            _musicFadeTimer += deltaTime;
            float progress = Math.Min(_musicFadeTimer / _musicFadeDuration, 1.0f);

            if (_isFadingOut)
            {
                // Fade out
                var currentVolume = _audioManager.MusicVolume * _audioManager.MasterVolume * (1.0f - progress);
                _audio.SetMusicVolume(currentVolume);

                if (progress >= 1.0f)
                {
                    _isFadingOut = false;
                    _musicFadeDuration = 0;
                }
            }
            else
            {
                // Fade in
                var currentVolume = _audioManager.MusicVolume * _audioManager.MasterVolume * progress;
                _audio.SetMusicVolume(currentVolume);

                if (progress >= 1.0f)
                {
                    _musicFadeDuration = 0;
                }
            }
        }

        private void UpdateAudioSource(AudioSource audioSource, float deltaTime)
        {
            if (!audioSource.IsPlaying)
                return;

            // Check if the sound is still playing via the audio bridge
            if (!_audio.IsSoundPlaying(audioSource.ClipId))
            {
                audioSource.IsPlaying = false;
                audioSource.OnSoundFinished?.Invoke();
            }
        }

        private void UpdateBackgroundMusic(BackgroundMusic bgMusic, float deltaTime)
        {
            if (!bgMusic.IsPlaying)
                return;

            // Update fade-in progress
            if (bgMusic.FadeProgress < 1.0f)
            {
                bgMusic.FadeProgress += deltaTime / bgMusic.FadeInTime;
                if (bgMusic.FadeProgress >= 1.0f)
                {
                    bgMusic.FadeProgress = 1.0f;
                }
            }
        }

        private void UpdateSFXPlayer(SFXPlayer sfxPlayer, float deltaTime)
        {
            // If nothing is playing, get next sound from queue
            if (sfxPlayer.CurrentSound == null && sfxPlayer.SoundQueue.Count > 0)
            {
                sfxPlayer.CurrentSound = sfxPlayer.SoundQueue.Dequeue();
                sfxPlayer.CurrentSound.Delay -= deltaTime;
            }

            // Update current sound
            if (sfxPlayer.CurrentSound != null)
            {
                sfxPlayer.CurrentSound.Delay -= deltaTime;

                if (sfxPlayer.CurrentSound.Delay <= 0)
                {
                    // Play the sound
                    var volume = sfxPlayer.CurrentSound.Volume * _audioManager.SFXVolume * _audioManager.MasterVolume;
                    _audio.PlaySound(sfxPlayer.CurrentSound.ClipId, volume);

                    sfxPlayer.CurrentSound.OnComplete?.Invoke();
                    sfxPlayer.CurrentSound = null;
                }
            }
        }
    }
}
