using GameCore;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Unity audio bridge - connects GameCore audio system to Unity's AudioSource.
/// Handles sound effects, background music, volume control, and audio management.
/// </summary>
public class UnityAudioBridge : IEngineAudio
{
    private Dictionary<string, AudioClip> _soundCache = new();
    private Dictionary<string, AudioSource> _activeAudioSources = new();
    private AudioListener _audioListener;
    private AudioSource _backgroundMusicSource;
    private GameObject _audioRoot;
    
    private float _masterVolume = 1.0f;
    private float _sfxVolume = 1.0f;
    private float _musicVolume = 0.5f;

    public UnityAudioBridge()
    {
        InitializeAudioSystem();
    }

    private void InitializeAudioSystem()
    {
        // Create root for organizing audio objects
        _audioRoot = new GameObject("AudioSystem");
        
        // Find or create audio listener
        _audioListener = Object.FindObjectOfType<AudioListener>();
        if (_audioListener == null)
        {
            var audioGO = new GameObject("AudioListener");
            audioGO.transform.SetParent(_audioRoot.transform);
            _audioListener = audioGO.AddComponent<AudioListener>();
        }

        // Create background music source
        var bgMusicGO = new GameObject("BackgroundMusic");
        bgMusicGO.transform.SetParent(_audioRoot.transform);
        _backgroundMusicSource = bgMusicGO.AddComponent<AudioSource>();
        _backgroundMusicSource.loop = true;
        _backgroundMusicSource.volume = _musicVolume;
    }

    public void PlaySound(string soundId, float volume = 1.0f)
    {
        if (string.IsNullOrEmpty(soundId))
            return;

        // Load audio clip from Resources
        var audioClip = LoadAudioClip(soundId);
        if (audioClip == null)
        {
            Debug.LogWarning($"Audio clip not found: Audio/{soundId}");
            return;
        }

        // Calculate effective volume
        float effectiveVolume = volume * _sfxVolume * _masterVolume;

        // Create temporary audio source
        var audioGO = new GameObject($"SFX_{soundId}");
        audioGO.transform.SetParent(_audioRoot.transform);
        var audioSource = audioGO.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = effectiveVolume;
        audioSource.Play();

        // Remove after sound finishes
        Object.Destroy(audioGO, audioClip.length);

        // Cache for potential stopping
        if (_activeAudioSources.ContainsKey(soundId))
        {
            StopSound(soundId);
        }
        _activeAudioSources[soundId] = audioSource;
    }

    public void StopSound(string soundId)
    {
        if (_activeAudioSources.TryGetValue(soundId, out var audioSource))
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                Object.Destroy(audioSource.gameObject);
            }
            _activeAudioSources.Remove(soundId);
        }
    }

    public void PlayBackgroundMusic(string musicId, float volume = 0.5f)
    {
        if (string.IsNullOrEmpty(musicId))
            return;

        if (_backgroundMusicSource.isPlaying && _backgroundMusicSource.clip != null)
        {
            StopBackgroundMusic(0.5f);
        }

        var audioClip = LoadAudioClip(musicId);
        if (audioClip == null)
        {
            Debug.LogWarning($"Audio clip not found: Audio/{musicId}");
            return;
        }

        _backgroundMusicSource.clip = audioClip;
        _backgroundMusicSource.volume = volume * _masterVolume;
        _backgroundMusicSource.Play();
    }

    public void StopBackgroundMusic(float fadeOutTime = 0f)
    {
        if (!_backgroundMusicSource.isPlaying)
            return;

        if (fadeOutTime > 0)
        {
            // Simple fade out (could be improved with coroutines)
            // For now, just stop immediately
            _backgroundMusicSource.Stop();
        }
        else
        {
            _backgroundMusicSource.Stop();
        }
    }

    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
    }

    public float GetMasterVolume()
    {
        return _masterVolume;
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
    }

    public bool IsSoundPlaying(string soundId)
    {
        if (_activeAudioSources.TryGetValue(soundId, out var audioSource))
        {
            return audioSource != null && audioSource.isPlaying;
        }
        return false;
    }

    public void PreloadSound(string soundId)
    {
        if (!_soundCache.ContainsKey(soundId))
        {
            var audioClip = Resources.Load<AudioClip>($"Audio/{soundId}");
            if (audioClip != null)
            {
                _soundCache[soundId] = audioClip;
            }
            else
            {
                Debug.LogWarning($"Failed to preload audio: Audio/{soundId}");
            }
        }
    }

    /// <summary>
    /// Preload multiple audio clips at once.
    /// </summary>
    public void PreloadSounds(params string[] soundIds)
    {
        foreach (var soundId in soundIds)
        {
            PreloadSound(soundId);
        }
    }

    /// <summary>
    /// Preload all audio clips from a specific folder.
    /// </summary>
    public void PreloadAudioFolder(string folderPath)
    {
        var clips = Resources.LoadAll<AudioClip>(folderPath);
        foreach (var clip in clips)
        {
            _soundCache[clip.name] = clip;
        }
    }

    private AudioClip LoadAudioClip(string soundId)
    {
        // Check cache first
        if (_soundCache.TryGetValue(soundId, out var cachedClip))
        {
            return cachedClip;
        }

        // Try to load from Resources
        var audioClip = Resources.Load<AudioClip>($"Audio/{soundId}");
        if (audioClip != null)
        {
            _soundCache[soundId] = audioClip;
        }

        return audioClip;
    }

    private void UpdateAllVolumes()
    {
        // Update background music
        if (_backgroundMusicSource != null)
        {
            _backgroundMusicSource.volume = _musicVolume * _masterVolume;
        }

        // Update active sound effects
        foreach (var soundSource in _activeAudioSources.Values)
        {
            if (soundSource != null && soundSource.isPlaying)
            {
                soundSource.volume *= (_sfxVolume * _masterVolume);
            }
        }
    }

    /// <summary>
    /// Clean up when the audio system is destroyed.
    /// </summary>
    public void Cleanup()
    {
        if (_audioRoot != null)
        {
            Object.Destroy(_audioRoot);
        }
        _soundCache.Clear();
        _activeAudioSources.Clear();
    }
}
