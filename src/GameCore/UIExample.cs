using System;
using System.Linq;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// Example demonstrating how to use the UI system.
    /// Shows:
    /// - Creating and displaying health bars
    /// - Score management and display
    /// - Damage number popups
    /// - UI element positioning
    /// - UI managers and state
    /// </summary>
    public class UIExample
    {
        /// <summary>
        /// Example 1: Create a health bar for the player
        /// </summary>
        public static void CreatePlayerHealthBar(Entity player, UISystem uiSystem)
        {
            var healthBar = new HealthBar
            {
                Id = "player_healthbar",
                TargetEntity = player,
                Width = 200f,
                Height = 20f,
                Anchor = UIElement.AnchorPosition.TopLeft,
                Offset = new Vector3(20, 20, 0),
                ShowHealthText = true,
                AnimationSpeed = 5f,
                FillColor = "green",
                BackgroundColor = "gray",
                LowHealthColor = "red"
            };

            // Add to a UI manager entity
            var uiEntity = new Entity(100);
            uiEntity.AddComponent(healthBar);
        }

        /// <summary>
        /// Example 2: Create and update score display
        /// </summary>
        public static void SetupScoreDisplay(Game game, UISystem uiSystem)
        {
            var scoreEntity = game.CreateEntity();
            scoreEntity.AddComponent(new Tag("UIElement"));
            var scoreDisplay = scoreEntity.AddComponent(new ScoreDisplay
            {
                Id = "score_display",
                Label = "Score:",
                FontSize = 24,
                TextColor = "white",
                Anchor = UIElement.AnchorPosition.TopRight,
                Offset = new Vector3(-20, 20, 0),
                Score = 0,
                ShowPopups = true
            });

            // Later in game, add score
            uiSystem.AddScore(scoreEntity, 100);
            uiSystem.AddScore(scoreEntity, 50);
        }

        /// <summary>
        /// Example 3: Show damage numbers on hit
        /// </summary>
        public static void OnDamageDealt(UISystem uiSystem, Vector3 hitPosition, int damageAmount, bool isCritical = false)
        {
            uiSystem.ShowDamageNumber(hitPosition, damageAmount, isCritical);
            
            // If critical hit, play special sound and show with different color
            if (isCritical)
            {
                // Play critical hit sound
                // audioSystem.PlaySound("critical_hit", 1.0f);
            }
        }

        /// <summary>
        /// Example 4: Create HUD elements (crosshair, minimap)
        /// </summary>
        public static void CreateHUDElements(Game game)
        {
            // Create crosshair
            var crosshairEntity = game.CreateEntity();
            crosshairEntity.AddComponent(new Tag("UIElement"));
            crosshairEntity.AddComponent(new Crosshair
            {
                Id = "crosshair",
                Size = 10f,
                CrosshairColor = "white",
                Style = "cross",
                Anchor = UIElement.AnchorPosition.MiddleCenter
            });

            // Create minimap
            var minimapEntity = game.CreateEntity();
            minimapEntity.AddComponent(new Tag("UIElement"));
            minimapEntity.AddComponent(new Minimap
            {
                Id = "minimap",
                Size = 150f,
                WorldRange = 100f,
                Anchor = UIElement.AnchorPosition.TopRight,
                Offset = new Vector3(-20, 20, 0),
                BackgroundColor = "black",
                BorderColor = "white"
            });
        }

        /// <summary>
        /// Example 5: Show interaction prompts
        /// </summary>
        public static void ShowInteractionPrompt(Entity interactable, Entity player)
        {
            var promptEntity = new Entity(200);
            var prompt = new InteractionPrompt
            {
                InteractableEntity = interactable,
                PromptText = "Press [E] to interact",
                FontSize = 14,
                TextColor = "yellow",
                ShowAnimation = true
            };
            promptEntity.AddComponent(prompt);
        }

        /// <summary>
        /// Example 6: Display player status effects
        /// </summary>
        public static void ShowStatusEffects(Entity player)
        {
            var statusEntity = new Entity(300);
            var statusDisplay = new StatusEffectDisplay
            {
                Id = "status_effects",
                TargetEntity = player,
                IconSize = 32f,
                IconSpacing = 8f,
                MaxIcons = 10,
                Anchor = UIElement.AnchorPosition.BottomLeft,
                Offset = new Vector3(20, -20, 0)
            };
            statusEntity.AddComponent(statusDisplay);
        }

        /// <summary>
        /// Example 7: Create victory/defeat messages
        /// </summary>
        public static void ShowGameOverScreen(Game game, bool isVictory)
        {
            var messageEntity = game.CreateEntity();
            messageEntity.AddComponent(new Tag("UIElement"));
            messageEntity.AddComponent(new TextDisplay(isVictory ? "YOU WIN!" : "GAME OVER")
            {
                Id = "game_over_message",
                FontSize = 48,
                TextColor = isVictory ? "green" : "red",
                Alignment = "center",
                Anchor = UIElement.AnchorPosition.MiddleCenter,
                Offset = Vector3.Zero
            });

            // Add score summary
            var scoreEntity = game.CreateEntity();
            scoreEntity.AddComponent(new TextDisplay("Press SPACE to Continue")
            {
                FontSize = 20,
                TextColor = "white",
                Anchor = UIElement.AnchorPosition.MiddleCenter,
                Offset = new Vector3(0, -50, 0)
            });
        }

        /// <summary>
        /// Example 8: Dynamic health bar colors based on health level
        /// </summary>
        public static void UpdateHealthBarColor(HealthBar healthBar, float healthPercent)
        {
            if (healthPercent > 0.5f)
            {
                healthBar.FillColor = "green";
            }
            else if (healthPercent > 0.25f)
            {
                healthBar.FillColor = "yellow";
            }
            else
            {
                healthBar.FillColor = "red";
            }
        }

        /// <summary>
        /// Example 9: Show treasure/coin collection popups
        /// </summary>
        public static void OnTreasureCollected(UISystem uiSystem, Vector3 position, int value)
        {
            // Show "+100" popup that floats upward
            uiSystem.ShowScorePopup(position, value);
        }

        /// <summary>
        /// Example 10: Create combo display UI
        /// </summary>
        public static void ShowComboDisplay(Game game, int comboCount)
        {
            var comboEntity = game.CreateEntity();
            comboEntity.AddComponent(new TextDisplay($"COMBO x{comboCount}")
            {
                Id = "combo_display",
                FontSize = 32,
                TextColor = "orange",
                Alignment = "center",
                Anchor = UIElement.AnchorPosition.MiddleCenter,
                Offset = new Vector3(0, 100, 0)
            });
        }

        /// <summary>
        /// Example 11: Create objective/mission display
        /// </summary>
        public static void ShowObjective(Game game, string objectiveText)
        {
            var objectiveEntity = game.CreateEntity();
            objectiveEntity.AddComponent(new TextDisplay(objectiveText)
            {
                Id = "objective",
                FontSize = 16,
                TextColor = "cyan",
                Anchor = UIElement.AnchorPosition.TopCenter,
                Offset = new Vector3(0, 20, 0),
                UpdateWhenPaused = true
            });
        }

        /// <summary>
        /// Example 12: Get UI manager and modify settings
        /// </summary>
        public static void ConfigureUISettings(Game game)
        {
            var uiManager = game.Entities
                .FirstOrDefault(e => e.TryGetComponent<UIManager>(out _));

            if (uiManager != null)
            {
                uiManager.TryGetComponent<UIManager>(out var manager);
                
                // Adjust UI settings
                manager.HUDScale = 1.2f;        // Make UI 20% larger
                manager.UIOpacity = 0.95f;      // Slightly transparent
                manager.ColorScheme = "dark";   // Use dark theme
                manager.ShowDebugUI = false;    // Hide debug info
            }
        }

        /// <summary>
        /// Example 13: Pause menu UI
        /// </summary>
        public static void ShowPauseMenu(Game game, UISystem uiSystem)
        {
            // Find or create UI manager
            var uiManager = game.Entities
                .FirstOrDefault(e => e.TryGetComponent<UIManager>(out _))
                ?.GetComponent<UIManager>();

            if (uiManager != null)
            {
                uiManager.IsPaused = true;
            }

            // Create pause menu background
            var pauseEntity = game.CreateEntity();
            pauseEntity.AddComponent(new TextDisplay("PAUSED")
            {
                FontSize = 48,
                TextColor = "white",
                Alignment = "center",
                Anchor = UIElement.AnchorPosition.MiddleCenter,
                Offset = new Vector3(0, 50, 0),
                UpdateWhenPaused = true
            });

            // Add menu options
            var resumeOption = game.CreateEntity();
            resumeOption.AddComponent(new TextDisplay("Resume (ESC)")
            {
                FontSize = 24,
                TextColor = "white",
                Anchor = UIElement.AnchorPosition.MiddleCenter,
                Offset = new Vector3(0, 0, 0),
                UpdateWhenPaused = true
            });

            var settingsOption = game.CreateEntity();
            settingsOption.AddComponent(new TextDisplay("Settings")
            {
                FontSize = 24,
                TextColor = "white",
                Anchor = UIElement.AnchorPosition.MiddleCenter,
                Offset = new Vector3(0, -40, 0),
                UpdateWhenPaused = true
            });

            var quitOption = game.CreateEntity();
            quitOption.AddComponent(new TextDisplay("Quit to Menu")
            {
                FontSize = 24,
                TextColor = "white",
                Anchor = UIElement.AnchorPosition.MiddleCenter,
                Offset = new Vector3(0, -80, 0),
                UpdateWhenPaused = true
            });
        }

        /// <summary>
        /// Example 14: Complete game integration
        /// </summary>
        public static void InitializeGameUIFully(
            Game game,
            IEngineInput input,
            IEngineRenderer renderer,
            IEngineAudio audio,
            UISystem uiSystem)
        {
            // Find player
            var player = game.Entities.FirstOrDefault(e => 
                e.TryGetComponent<Tag>(out var tag) && tag.Value == "Player");

            if (player == null)
                return;

            // Create health bar for player
            CreatePlayerHealthBar(player, uiSystem);

            // Setup score display
            SetupScoreDisplay(game, uiSystem);

            // Create HUD elements
            CreateHUDElements(game);

            // Show status effects
            ShowStatusEffects(player);

            // Configure UI settings
            ConfigureUISettings(game);

            // Show initial objective
            ShowObjective(game, "Collect all coins!");
        }
    }
}
