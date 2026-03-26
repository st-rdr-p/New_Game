using GameCore;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Unity audio bridge - connects GameCore audio system to Unity's AudioSource.
/// </summary>
public class UnityAudioBridge : IEngineAudio
{
    private Dictionary<string, AudioClip> _soundCache = new();
    private Dictionary<string, AudioSource> _activeAudioSources = new();
    private AudioListener _audioListener;

    public UnityAudioBridge()
    {
        // Find or create audio listener
        _audioListener = Object.FindObjectOfType<AudioListener>();
        if (_audioListener == null)
        {
            var audioGO = new GameObject("AudioListener");
            _audioListener = audioGO.AddComponent<AudioListener>();
        }
    }

    public void PlaySound(string soundId)
    {
        // Load audio clip from Resources
        var audioClip = Resources.Load<AudioClip>($"Audio/{soundId}");
        if (audioClip == null)
        {
            Debug.LogWarning($"Audio clip not found: Audio/{soundId}");
            return;
        }

        // Create temporary audio source
        var audioGO = new GameObject($"AudioSource_{soundId}");
        var audioSource = audioGO.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
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

    /// <summary>
    /// Preload audio clips for better performance.
    /// </summary>
    public void PreloadSound(string soundId)
    {
        if (!_soundCache.ContainsKey(soundId))
        {
            var audioClip = Resources.Load<AudioClip>($"Audio/{soundId}");
            if (audioClip != null)
            {
                _soundCache[soundId] = audioClip;
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
}
