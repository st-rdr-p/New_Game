using System;

namespace GameCore
{
    /// <summary>
    /// Base UI element component.
    /// </summary>
    public class UIElement : Component
    {
        public enum AnchorPosition
        {
            TopLeft,
            TopCenter,
            TopRight,
            MiddleLeft,
            MiddleCenter,
            MiddleRight,
            BottomLeft,
            BottomCenter,
            BottomRight
        }

        /// <summary>
        /// Anchor position on screen.
        /// </summary>
        public AnchorPosition Anchor { get; set; } = AnchorPosition.TopLeft;

        /// <summary>
        /// Offset from anchor position in pixels.
        /// </summary>
        public Vector3 Offset { get; set; } = Vector3.Zero;

        /// <summary>
        /// whether this UI element is visible.
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Alpha/opacity (0-1).
        /// </summary>
        public float Opacity { get; set; } = 1.0f;

        /// <summary>
        /// Whether to update even when game is paused.
        /// </summary>
        public bool UpdateWhenPaused { get; set; } = false;

        /// <summary>
        /// Callback when UI element is clicked.
        /// </summary>
        public Action<UIElement> OnClick { get; set; }

        /// <summary>
        /// Callback when UI element is hovered.
        /// </summary>
        public Action<UIElement> OnHover { get; set; }

        /// <summary>
        /// Unique identifier for this UI element.
        /// </summary>
        public string Id { get; set; }
    }

    /// <summary>
    /// Health bar UI component.
    /// </summary>
    public class HealthBar : UIElement
    {
        /// <summary>
        /// Target entity whose health to display.
        /// </summary>
        public Entity TargetEntity { get; set; }

        /// <summary>
        /// Width of the health bar in pixels.
        /// </summary>
        public float Width { get; set; } = 200f;

        /// <summary>
        /// Height of the health bar in pixels.
        /// </summary>
        public float Height { get; set; } = 20f;

        /// <summary>
        /// Background color (when empty).
        /// </summary>
        public string BackgroundColor { get; set; } = "gray";

        /// <summary>
        /// Fill color (when full).
        /// </summary>
        public string FillColor { get; set; } = "green";

        /// <summary>
        /// Color when health is low (< 25%).
        /// </summary>
        public string LowHealthColor { get; set; } = "red";

        /// <summary>
        /// Show health text above bar.
        /// </summary>
        public bool ShowHealthText { get; set; } = true;

        /// <summary>
        /// Animation speed for fill bar transitions (0 = instant).
        /// </summary>
        public float AnimationSpeed { get; set; } = 5f;

        /// <summary>
        /// Current displayed health (animates toward actual health).
        /// </summary>
        public float DisplayedHealth { get; set; }

        /// <summary>
        /// Max health for scaling.
        /// </summary>
        public float MaxHealth { get; set; } = 100f;

        public HealthBar()
        {
            Id = "healthbar_player";
            Anchor = AnchorPosition.TopLeft;
            Offset = new Vector3(20, 20, 0);
        }
    }

    /// <summary>
    /// Score display UI component.
    /// </summary>
    public class ScoreDisplay : UIElement
    {
        /// <summary>
        /// Current score to display.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Previous score (for animation).
        /// </summary>
        public int PreviousScore { get; set; }

        /// <summary>
        /// Label text (e.g., "Score:").
        /// </summary>
        public string Label { get; set; } = "Score:";

        /// <summary>
        /// Font size in pixels.
        /// </summary>
        public int FontSize { get; set; } = 24;

        /// <summary>
        /// Text color.
        /// </summary>
        public string TextColor { get; set; } = "white";

        /// <summary>
        /// Show score pop-up animations.
        /// </summary>
        public bool ShowPopups { get; set; } = true;

        /// <summary>
        /// Duration of score popups in seconds.
        /// </summary>
        public float PopupDuration { get; set; } = 1.0f;

        public ScoreDisplay()
        {
            Id = "score_display";
            Anchor = AnchorPosition.TopRight;
            Offset = new Vector3(-20, 20, 0);
        }
    }

    /// <summary>
    /// Score popup that floats away and fades.
    /// </summary>
    public class ScorePopup : UIElement
    {
        /// <summary>
        /// Amount being displayed.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Time remaining before popup disappears.
        /// </summary>
        public float TimeRemaining { get; set; } = 1.0f;

        /// <summary>
        /// Color of the popup text.
        /// </summary>
        public string PopupColor { get; set; } = "yellow";

        /// <summary>
        /// Font size.
        /// </summary>
        public int FontSize { get; set; } = 18;

        /// <summary>
        /// Speed at which popup moves upward.
        /// </summary>
        public float FloatSpeed { get; set; } = 50f;

        public ScorePopup(int amount, Vector3 position)
        {
            Amount = amount;
            Offset = position;
            TimeRemaining = 1.0f;
        }
    }

    /// <summary>
    /// Minimap/radar UI component.
    /// </summary>
    public class Minimap : UIElement
    {
        /// <summary>
        /// Target entity to follow on minimap.
        /// </summary>
        public Entity TargetEntity { get; set; }

        /// <summary>
        /// Size of minimap in pixels.
        /// </summary>
        public float Size { get; set; } = 150f;

        /// <summary>
        /// World space size the minimap covers.
        /// </summary>
        public float WorldRange { get; set; } = 100f;

        /// <summary>
        /// Background color.
        /// </summary>
        public string BackgroundColor { get; set; } = "black";

        /// <summary>
        /// Border color.
        /// </summary>
        public string BorderColor { get; set; } = "white";

        /// <summary>
        /// Player indicator color.
        /// </summary>
        public string PlayerColor { get; set; } = "green";

        /// <summary>
        /// Enemy color.
        /// </summary>
        public string EnemyColor { get; set; } = "red";

        /// <summary>
        /// Collectible color.
        /// </summary>
        public string CollectibleColor { get; set; } = "yellow";

        public Minimap()
        {
            Id = "minimap";
            Anchor = AnchorPosition.TopRight;
            Offset = new Vector3(-20, 20, 0);
        }
    }

    /// <summary>
    /// Text UI element.
    /// </summary>
    public class TextDisplay : UIElement
    {
        /// <summary>
        /// Text to display.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Font size in pixels.
        /// </summary>
        public int FontSize { get; set; } = 16;

        /// <summary>
        /// Text color.
        /// </summary>
        public string TextColor { get; set; } = "white";

        /// <summary>
        /// Text alignment (left, center, right).
        /// </summary>
        public string Alignment { get; set; } = "left";

        /// <summary>
        /// Maximum width before wrapping (0 = no wrap).
        /// </summary>
        public float MaxWidth { get; set; } = 0f;

        public TextDisplay(string text = "")
        {
            Text = text;
        }
    }

    /// <summary>
    /// Status effect indicator (showing active effects).
    /// </summary>
    public class StatusEffectDisplay : UIElement
    {
        /// <summary>
        /// Target entity to display effects for.
        /// </summary>
        public Entity TargetEntity { get; set; }

        /// <summary>
        /// Icon size in pixels.
        /// </summary>
        public float IconSize { get; set; } = 32f;

        /// <summary>
        /// Spacing between icons.
        /// </summary>
        public float IconSpacing { get; set; } = 8f;

        /// <summary>
        /// Maximum icons to display.
        /// </summary>
        public int MaxIcons { get; set; } = 10;

        public StatusEffectDisplay()
        {
            Id = "status_effects";
            Anchor = AnchorPosition.BottomLeft;
            Offset = new Vector3(20, -20, 0);
        }
    }

    /// <summary>
    /// Crosshair UI element.
    /// </summary>
    public class Crosshair : UIElement
    {
        /// <summary>
        /// Size of the crosshair in pixels.
        /// </summary>
        public float Size { get; set; } = 10f;

        /// <summary>
        /// Color of the crosshair.
        /// </summary>
        public string CrosshairColor { get; set; } = "white";

        /// <summary>
        /// Thickness of the crosshair lines.
        /// </summary>
        public float Thickness { get; set; } = 2f;

        /// <summary>
        /// Style: "cross", "dot", "circle", "target".
        /// </summary>
        public string Style { get; set; } = "cross";

        public Crosshair()
        {
            Id = "crosshair";
            Anchor = AnchorPosition.MiddleCenter;
            Offset = Vector3.Zero;
        }
    }

    /// <summary>
    /// Interaction prompt UI (press E to interact).
    /// </summary>
    public class InteractionPrompt : UIElement
    {
        /// <summary>
        /// The interactable entity this prompt refers to.
        /// </summary>
        public Entity InteractableEntity { get; set; }

        /// <summary>
        /// Text to display (e.g., "Press E to interact").
        /// </summary>
        public string PromptText { get; set; } = "Press [E] to interact";

        /// <summary>
        /// Font size.
        /// </summary>
        public int FontSize { get; set; } = 14;

        /// <summary>
        /// Text color.
        /// </summary>
        public string TextColor { get; set; } = "yellow";

        /// <summary>
        /// Distance from world position to UI position in pixels.
        /// </summary>
        public float ScreenOffset { get; set; } = 30f;

        /// <summary>
        /// Show animation/bobbing.
        /// </summary>
        public bool ShowAnimation { get; set; } = true;

        /// <summary>
        /// Animation time tracker.
        /// </summary>
        public float AnimationTime { get; set; }
    }

    /// <summary>
    /// Damage number popup (floating damage integers).
    /// </summary>
    public class DamageNumber : UIElement
    {
        /// <summary>
        /// Amount of damage to display.
        /// </summary>
        public int Damage { get; set; }

        /// <summary>
        /// Whether this is a critical hit (show in different color).
        /// </summary>
        public bool IsCritical { get; set; }

        /// <summary>
        /// Time remaining before disappearing.
        /// </summary>
        public float TimeRemaining { get; set; } = 1.5f;

        /// <summary>
        /// Font size.
        /// </summary>
        public int FontSize { get; set; } = 24;

        /// <summary>
        /// Speed to move upward.
        /// </summary>
        public float FloatSpeed { get; set; } = 80f;

        /// <summary>
        /// Color for normal damage.
        /// </summary>
        public string DamageColor { get; set; } = "red";

        /// <summary>
        /// Color for critical damage.
        /// </summary>
        public string CriticalColor { get; set; } = "yellow";

        public DamageNumber(int damage, Vector3 position, bool isCritical = false)
        {
            Damage = damage;
            Offset = position;
            IsCritical = isCritical;
            TimeRemaining = 1.5f;
        }
    }

    /// <summary>
    /// UI manager component for tracking overall UI state.
    /// </summary>
    public class UIManager : Component
    {
        /// <summary>
        /// Whether the UI is currently paused/inactive.
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// Whether to show debug UI elements.
        /// </summary>
        public bool ShowDebugUI { get; set; }

        /// <summary>
        /// Whether to show all UI elements.
        /// </summary>
        public bool ShowAllUI { get; set; } = true;

        /// <summary>
        /// Color scheme (e.g., "default", "dark", "high_contrast").
        /// </summary>
        public string ColorScheme { get; set; } = "default";

        /// <summary>
        /// HUD scale multiplier.
        /// </summary>
        public float HUDScale { get; set; } = 1.0f;

        /// <summary>
        /// Overall UI opacity.
        /// </summary>
        public float UIOpacity { get; set; } = 1.0f;
    }
}
