using System;

namespace GameCore
{
    /// <summary>
    /// UI screen component that displays a full lore entry when collected/triggered.
    /// Supports corrupted file aesthetic and voice acting playback.
    /// </summary>
    public class LoreDisplayScreen : Component
    {
        public enum DisplayState
        {
            Hidden,
            Displaying,
            Paused,
            Closed
        }

        public DisplayState CurrentState { get; private set; }
        public LoreEntry? CurrentEntry { get; private set; }
        public Action<LoreEntry>? OnScreenOpened { get; set; }
        public Action<LoreEntry>? OnScreenClosed { get; set; }
        public Action<LoreEntry>? OnVoiceLineStart { get; set; }

        private float _displayDuration;
        private float _elapsedTime;
        private bool _isPlayingVoice;
        private string? _audioClipName;

        public LoreDisplayScreen()
        {
            CurrentState = DisplayState.Hidden;
        }

        public void DisplayLore(LoreEntry entry, string audioClipName = null)
        {
            if (entry == null)
                return;

            CurrentEntry = entry;
            CurrentState = DisplayState.Displaying;
            _audioClipName = audioClipName;
            _isPlayingVoice = false;
            _elapsedTime = 0f;

            OnScreenOpened?.Invoke(entry);
        }

        public void StartVoicePlayback()
        {
            if (CurrentEntry == null)
                return;

            _isPlayingVoice = true;
            OnVoiceLineStart?.Invoke(CurrentEntry);
        }

        public void StopVoicePlayback()
        {
            _isPlayingVoice = false;
        }

        public void CloseLore()
        {
            if (CurrentState == DisplayState.Hidden)
                return;

            var closingEntry = CurrentEntry;
            CurrentState = DisplayState.Closed;
            CurrentEntry = null;
            _isPlayingVoice = false;

            OnScreenClosed?.Invoke(closingEntry);
        }

        public bool IsPlayerInputBlocked()
        {
            return CurrentState != DisplayState.Hidden && CurrentState != DisplayState.Closed;
        }

        public void SetDisplayDuration(float duration)
        {
            _displayDuration = duration;
        }

        public float GetElapsedTime()
        {
            return _elapsedTime;
        }

        public bool IsVoicePlaying()
        {
            return _isPlayingVoice;
        }

        public string GetAudioClipName()
        {
            return _audioClipName;
        }
    }
}
