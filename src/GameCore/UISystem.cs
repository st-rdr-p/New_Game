using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// UI System - manages all UI elements, health bars, score display, and HUD elements.
    /// Processes UI components and coordinates with the game state.
    /// </summary>
    public class UISystem : ISystem
    {
        private readonly IEngineRenderer _renderer;
        private UIManager _uiManager;
        private List<ScorePopup> _activePopups = new();
        private List<DamageNumber> _activeDamageNumbers = new();

        public UISystem(IEngineRenderer renderer)
        {
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        }

        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            var entityList = new List<Entity>(entities);

            // Initialize UI manager if needed
            if (_uiManager == null)
            {
                foreach (var entity in entityList)
                {
                    if (entity.TryGetComponent<UIManager>(out var manager))
                    {
                        _uiManager = manager;
                        break;
                    }
                }

                // Create default UI manager if none exists
                if (_uiManager == null)
                {
                    var managerEntity = new Entity(int.MaxValue - 1);
                    _uiManager = new UIManager();
                    managerEntity.AddComponent(_uiManager);
                }
            }

            if (_uiManager.IsPaused && !_uiManager.ShowAllUI)
                return;

            // Update all UI elements
            foreach (var entity in entityList)
            {
                UpdateHealthBars(entity, entityList);
                UpdateScoreDisplay(entity);
                UpdateStatusEffects(entity);
                UpdateDamageNumbers(deltaTime);
                UpdateScorePopups(deltaTime);
                UpdateInteractionPrompts(entity, deltaTime, entityList);
            }

            // Draw all visible UI
            foreach (var entity in entityList)
            {
                if (entity.TryGetComponent<HealthBar>(out var healthBar))
                {
                    DrawHealthBar(healthBar);
                }

                if (entity.TryGetComponent<ScoreDisplay>(out var scoreDisplay))
                {
                    DrawScoreDisplay(scoreDisplay);
                }

                if (entity.TryGetComponent<TextDisplay>(out var textDisplay))
                {
                    DrawTextDisplay(textDisplay);
                }

                if (entity.TryGetComponent<Crosshair>(out var crosshair))
                {
                    DrawCrosshair(crosshair);
                }

                if (entity.TryGetComponent<Minimap>(out var minimap))
                {
                    DrawMinimap(minimap, entityList);
                }

                if (entity.TryGetComponent<InteractionPrompt>(out var prompt))
                {
                    DrawInteractionPrompt(prompt);
                }
            }

            // Draw popups
            foreach (var popup in _activePopups)
            {
                DrawScorePopup(popup);
            }

            // Draw damage numbers
            foreach (var damageNum in _activeDamageNumbers)
            {
                DrawDamageNumber(damageNum);
            }
        }

        /// <summary>
        /// Create and display a damage number popup at a position.
        /// </summary>
        public void ShowDamageNumber(Vector3 worldPosition, int damage, bool isCritical = false)
        {
            var damageNumber = new DamageNumber(damage, worldPosition, isCritical);
            _activeDamageNumbers.Add(damageNumber);
        }

        /// <summary>
        /// Create and display a score popup.
        /// </summary>
        public void ShowScorePopup(Vector3 worldPosition, int points)
        {
            var popup = new ScorePopup(points, worldPosition);
            _activePopups.Add(popup);
        }

        /// <summary>
        /// Add points to the score display.
        /// </summary>
        public void AddScore(Entity scoreEntity, int points)
        {
            if (scoreEntity.TryGetComponent<ScoreDisplay>(out var scoreDisplay))
            {
                scoreDisplay.PreviousScore = scoreDisplay.Score;
                scoreDisplay.Score += points;
            }
        }

        private void UpdateHealthBars(Entity entity, List<Entity> allEntities)
        {
            if (!entity.TryGetComponent<HealthBar>(out var healthBar))
                return;

            if (healthBar.TargetEntity == null)
                return;

            if (!healthBar.TargetEntity.TryGetComponent<Health>(out var health))
                return;

            // Animate health bar fill
            float targetHealth = health.CurrentHealth;
            healthBar.DisplayedHealth = Mathf.Lerp(
                healthBar.DisplayedHealth,
                targetHealth,
                healthBar.AnimationSpeed * Time.DeltaTime
            );

            healthBar.MaxHealth = health.MaxHealth;
        }

        private void UpdateScoreDisplay(Entity entity)
        {
            if (entity.TryGetComponent<ScoreDisplay>(out var scoreDisplay))
            {
                // Score display is updated externally via AddScore()
            }
        }

        private void UpdateStatusEffects(Entity entity)
        {
            if (!entity.TryGetComponent<StatusEffectDisplay>(out var statusDisplay))
                return;

            if (statusDisplay.TargetEntity == null)
                return;

            // Status effects are drawn based on entity components
            // This would scan for PowerUpEffect and similar components
        }

        private void UpdateDamageNumbers(float deltaTime)
        {
            var toRemove = new List<DamageNumber>();

            foreach (var damageNum in _activeDamageNumbers)
            {
                damageNum.TimeRemaining -= deltaTime;
                damageNum.Offset = damageNum.Offset + Vector3.Up * damageNum.FloatSpeed * deltaTime;

                if (damageNum.TimeRemaining <= 0)
                {
                    toRemove.Add(damageNum);
                }
            }

            foreach (var damageNum in toRemove)
            {
                _activeDamageNumbers.Remove(damageNum);
            }
        }

        private void UpdateScorePopups(float deltaTime)
        {
            var toRemove = new List<ScorePopup>();

            foreach (var popup in _activePopups)
            {
                popup.TimeRemaining -= deltaTime;
                popup.Offset = popup.Offset + Vector3.Up * popup.FloatSpeed * deltaTime;

                if (popup.TimeRemaining <= 0)
                {
                    toRemove.Add(popup);
                }
            }

            foreach (var popup in toRemove)
            {
                _activePopups.Remove(popup);
            }
        }

        private void UpdateInteractionPrompts(Entity entity, float deltaTime, List<Entity> allEntities)
        {
            if (!entity.TryGetComponent<InteractionPrompt>(out var prompt))
                return;

            if (prompt.ShowAnimation)
            {
                prompt.AnimationTime += deltaTime;
            }
        }

        private void DrawHealthBar(HealthBar healthBar)
        {
            if (!healthBar.IsVisible || healthBar.TargetEntity == null)
                return;

            // This would be implemented by the renderer
            // For now, we notify the renderer to draw the health bar
            _renderer.DrawHealthBar(
                healthBar.TargetEntity.GetComponent<Transform3D>()?.Position ?? Vector3.Zero,
                healthBar.DisplayedHealth,
                healthBar.MaxHealth,
                healthBar.Width,
                healthBar.Height,
                healthBar.FillColor,
                healthBar.BackgroundColor,
                healthBar.Opacity
            );
        }

        private void DrawScoreDisplay(ScoreDisplay scoreDisplay)
        {
            if (!scoreDisplay.IsVisible)
                return;

            _renderer.DrawUIText(
                scoreDisplay.Label + " " + scoreDisplay.Score,
                scoreDisplay.Anchor,
                scoreDisplay.Offset,
                scoreDisplay.FontSize,
                scoreDisplay.TextColor,
                scoreDisplay.Opacity
            );
        }

        private void DrawTextDisplay(TextDisplay textDisplay)
        {
            if (!textDisplay.IsVisible || string.IsNullOrEmpty(textDisplay.Text))
                return;

            _renderer.DrawUIText(
                textDisplay.Text,
                textDisplay.Anchor,
                textDisplay.Offset,
                textDisplay.FontSize,
                textDisplay.TextColor,
                textDisplay.Opacity
            );
        }

        private void DrawCrosshair(Crosshair crosshair)
        {
            if (!crosshair.IsVisible)
                return;

            _renderer.DrawCrosshair(
                crosshair.Style,
                crosshair.Size,
                crosshair.CrosshairColor,
                crosshair.Thickness,
                crosshair.Opacity
            );
        }

        private void DrawMinimap(Minimap minimap, List<Entity> entities)
        {
            if (!minimap.IsVisible)
                return;

            _renderer.DrawMinimap(
                minimap.Size,
                minimap.WorldRange,
                minimap.Anchor,
                minimap.Offset,
                minimap.BackgroundColor,
                minimap.BorderColor,
                minimap.Opacity
            );
        }

        private void DrawInteractionPrompt(InteractionPrompt prompt)
        {
            if (!prompt.IsVisible || prompt.InteractableEntity == null)
                return;

            if (!prompt.InteractableEntity.TryGetComponent<Transform3D>(out var transform))
                return;

            // Draw prompt above the interactable
            _renderer.DrawUIText(
                prompt.PromptText,
                UIElement.AnchorPosition.TopCenter,
                transform.Position + Vector3.Up * prompt.ScreenOffset,
                prompt.FontSize,
                prompt.TextColor,
                prompt.Opacity
            );
        }

        private void DrawScorePopup(ScorePopup popup)
        {
            if (!popup.IsVisible)
                return;

            // Fade out as time remains approaches zero
            float alpha = popup.Opacity * (popup.TimeRemaining / 1.0f);

            _renderer.DrawUIText(
                "+" + popup.Amount,
                UIElement.AnchorPosition.MiddleCenter,
                popup.Offset,
                popup.FontSize,
                popup.PopupColor,
                alpha
            );
        }

        private void DrawDamageNumber(DamageNumber damageNumber)
        {
            if (!damageNumber.IsVisible)
                return;

            // Fade out as time remains approaches zero
            float alpha = damageNumber.Opacity * Mathf.Max(0, damageNumber.TimeRemaining / 1.5f);
            string color = damageNumber.IsCritical ? damageNumber.CriticalColor : damageNumber.DamageColor;

            _renderer.DrawUIText(
                damageNumber.Damage.ToString(),
                UIElement.AnchorPosition.MiddleCenter,
                damageNumber.Offset,
                damageNumber.FontSize,
                color,
                alpha
            );
        }
    }

    /// <summary>
    /// Simple math utilities for UI animations.
    /// </summary>
    public static class Mathf
    {
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }

    /// <summary>
    /// Simple time tracking for UI system.
    /// </summary>
    public static class Time
    {
        public static float DeltaTime { get; set; } = 0.016f;  // ~60 FPS
    }
}
