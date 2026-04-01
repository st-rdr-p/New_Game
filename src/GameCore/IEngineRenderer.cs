namespace GameCore
{
    public interface IEngineRenderer
    {
        void DrawSprite(string spriteId, float x, float y, float rotation, float scaleX = 1.0f, float scaleY = 1.0f, float opacity = 1.0f);
        void DrawScreenFlash(float intensity, string color = "red");
        
        /// <summary>
        /// Apply pixelation/retro graphics post-processing to the render target.
        /// </summary>
        void ApplyRetroGraphics(RetroGraphicsEffect effect);
        
        /// <summary>
        /// Get the current screen width in pixels.
        /// </summary>
        int GetScreenWidth();
        
        /// <summary>
        /// Get the current screen height in pixels.
        /// </summary>
        int GetScreenHeight();

        /// <summary>
        /// Draw a health bar above an entity.
        /// </summary>
        void DrawHealthBar(Vector3 worldPosition, float currentHealth, float maxHealth, 
            float width, float height, string fillColor = "green", string backgroundColor = "gray", 
            float opacity = 1.0f);

        /// <summary>
        /// Draw UI text at a screen position.
        /// </summary>
        void DrawUIText(string text, UIElement.AnchorPosition anchor, Vector3 offset, 
            int fontSize, string color, float opacity);

        /// <summary>
        /// Draw a crosshair at screen center.
        /// </summary>
        void DrawCrosshair(string style, float size, string color, float thickness, float opacity);

        /// <summary>
        /// Draw a minimap showing entities.
        /// </summary>
        void DrawMinimap(float size, float worldRange, UIElement.AnchorPosition anchor, 
            Vector3 offset, string backgroundColor, string borderColor, float opacity);
    }

    public interface IEngineInput
    {
        bool IsKeyDown(string key);
        float GetAxis(string axis);  // horizontal, vertical etc.
        float GetMouseX();
        float GetMouseY();
        void LockMouse(bool locked);
        bool IsMouseLocked { get; }
    }

    public interface IEngineAudio
    {
        /// <summary>
        /// Play a sound effect with optional volume.
        /// </summary>
        void PlaySound(string soundId, float volume = 1.0f);

        /// <summary>
        /// Stop playing a sound by ID.
        /// </summary>
        void StopSound(string soundId);

        /// <summary>
        /// Play background music (loops). Only one background music plays at a time.
        /// </summary>
        void PlayBackgroundMusic(string musicId, float volume = 0.5f);

        /// <summary>
        /// Stop the current background music with optional fade-out time.
        /// </summary>
        void StopBackgroundMusic(float fadeOutTime = 0f);

        /// <summary>
        /// Set the master volume (0-1).
        /// </summary>
        void SetMasterVolume(float volume);

        /// <summary>
        /// Get the master volume.
        /// </summary>
        float GetMasterVolume();

        /// <summary>
        /// Set SFX volume (0-1).
        /// </summary>
        void SetSFXVolume(float volume);

        /// <summary>
        /// Set music volume (0-1).
        /// </summary>
        void SetMusicVolume(float volume);

        /// <summary>
        /// Check if a sound is currently playing.
        /// </summary>
        bool IsSoundPlaying(string soundId);

        /// <summary>
        /// Preload audio clips for better performance.
        /// </summary>
        void PreloadSound(string soundId);
    }
}
