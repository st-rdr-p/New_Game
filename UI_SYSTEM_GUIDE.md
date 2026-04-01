# UI System Guide

## Overview

The UI system provides a complete, engine-agnostic solution for in-game HUD, menus, and UI elements. It handles health bars, score displays, damage popups, minimap, interaction prompts, and more through an extensible component-based architecture.

## Architecture

### Core Components

1. **IEngineRenderer Interface** - Extended with UI rendering methods
   - DrawHealthBar
   - DrawUIText
   - DrawCrosshair
   - DrawMinimap

2. **UISystem** - Main system that processes UI entities
   - Manages UI element positioning and visibility
   - Handles animations (health bar lerp, popup floats)
   - Processes damage numbers and score popups
   - Updates interaction prompts

3. **UnityRendererBridge** - Unity implementation of UI drawing
   - Converts world positions to screen space
   - Manages UI canvas and text rendering
   - Handles color schemes and opacity

### UI Components

- **UIElement** (base) - Fundamental UI element with anchoring
- **HealthBar** - Entity health display with animation
- **ScoreDisplay** - Score counter with optional popups
- **DamageNumber** - Floating damage integers
- **ScorePopup** - Floating score animations
- **Minimap** - World overview radar
- **TextDisplay** - Generic text UI
- **StatusEffectDisplay** - Active buff/debuff icons
- **Crosshair** - Aiming reticle
- **InteractionPrompt** - "Press E" prompts
- **UIManager** - Global UI state

## Quick Start

### 1. Initialize Game with UI

```csharp
var game = new Game(inputBridge, audioBridge);
GameSetup.SetupSonicLikeGame(game, inputBridge, rendererBridge, audioBridge);
// UISystem is automatically added to the system pipeline
// UIManager entity is automatically created
```

### 2. Create a Health Bar

```csharp
var healthBar = new HealthBar
{
    TargetEntity = player,
    Width = 200f,
    Height = 20f,
    Anchor = UIElement.AnchorPosition.TopLeft,
    Offset = new Vector3(20, 20, 0),
    FillColor = "green",
    ShowHealthText = true
};

var uiEntity = game.CreateEntity();
uiEntity.AddComponent(healthBar);
```

### 3. Display Score

```csharp
var scoreEntity = game.CreateEntity();
var scoreDisplay = scoreEntity.AddComponent(new ScoreDisplay
{
    Label = "Score:",
    FontSize = 24,
    Anchor = UIElement.AnchorPosition.TopRight,
    Offset = new Vector3(-20, 20, 0),
    Score = 0
});

// Find UISystem and add score
var uiSystem = /* get from systems OR from AudioExample */;
uiSystem.AddScore(scoreEntity, 100);
```

### 4. Show Damage Numbers

```csharp
uiSystem.ShowDamageNumber(hitPosition, damageAmount, isCritical: false);
uiSystem.ShowDamageNumber(criticalHitPosition, 50, isCritical: true);
```

### 5. Show Score Popups

```csharp
uiSystem.ShowScorePopup(coinPosition, 10);  // Show "+10" popup
```

## Usage Patterns

### Anchor Positions

Positions UI elements relative to screen corners:

```csharp
UIElement.AnchorPosition.TopLeft            // Upper left
UIElement.AnchorPosition.TopCenter          // Top middle
UIElement.AnchorPosition.TopRight           // Upper right
UIElement.AnchorPosition.MiddleLeft         // Left middle
UIElement.AnchorPosition.MiddleCenter       // Screen center
UIElement.AnchorPosition.MiddleRight        // Right middle
UIElement.AnchorPosition.BottomLeft         // Lower left
UIElement.AnchorPosition.BottomCenter       // Bottom middle
UIElement.AnchorPosition.BottomRight        // Lower right
```

### Health Bar

```csharp
public class HealthBar : UIElement
{
    public Entity TargetEntity { get; set; }      // What entity to track
    public float Width { get; set; } = 200f;
    public float Height { get; set; } = 20f;
    public string FillColor { get; set; } = "green";
    public string BackgroundColor { get; set; } = "gray";
    public float AnimationSpeed { get; set; } = 5f;  // Lerp animation
    public bool ShowHealthText { get; set; } = true;
}
```

### Score Display

```csharp
public class ScoreDisplay : UIElement
{
    public int Score { get; set; }
    public string Label { get; set; } = "Score:";
    public int FontSize { get; set; } = 24;
    public bool ShowPopups { get; set; } = true;
}
```

### Damage Numbers

```csharp
public class DamageNumber : UIElement
{
    public int Damage { get; set; }
    public bool IsCritical { get; set; }
    public float FloatSpeed { get; set; } = 80f;  // Pixels/second upward
    public float TimeRemaining { get; set; } = 1.5f;
}
```

### Text Display

```csharp
public class TextDisplay : UIElement
{
    public string Text { get; set; }
    public int FontSize { get; set; } = 16;
    public string TextColor { get; set; } = "white";
    public string Alignment { get; set; } = "left";  // left, center, right
    public float MaxWidth { get; set; } = 0f;  // 0 = no wrap
}
```

### Interaction Prompt

```csharp
public class InteractionPrompt : UIElement
{
    public Entity InteractableEntity { get; set; }
    public string PromptText { get; set; } = "Press [E] to interact";
    public bool ShowAnimation { get; set; } = true;
}
```

### Minimap

```csharp
public class Minimap : UIElement
{
    public Entity TargetEntity { get; set; }
    public float Size { get; set; } = 150f;
    public float WorldRange { get; set; } = 100f;
    public string BackgroundColor { get; set; } = "black";
    public string PlayerColor { get; set; } = "green";
    public string EnemyColor { get; set; } = "red";
}
```

### Crosshair

```csharp
public class Crosshair : UIElement
{
    public float Size { get; set; } = 10f;
    public string CrosshairColor { get; set; } = "white";
    public string Style { get; set; } = "cross";  // cross, dot, circle, target
}
```

## Common Patterns

### Dynamic Color Based on Health

```csharp
var healthBar = entity.GetComponent<HealthBar>();
float healthPercent = healthBar.DisplayedHealth / healthBar.MaxHealth;

if (healthPercent > 0.5f)
    healthBar.FillColor = "green";
else if (healthPercent > 0.25f)
    healthBar.FillColor = "yellow";
else
    healthBar.FillColor = "red";
```

### Create HUD Elements

```csharp
// Crosshair
var crosshair = game.CreateEntity();
crosshair.AddComponent(new Crosshair
{
    Style = "cross",
    Size = 10f,
    Anchor = UIElement.AnchorPosition.MiddleCenter
});

// Minimap
var minimap = game.CreateEntity();
minimap.AddComponent(new Minimap
{
    Size = 150f,
    WorldRange = 100f,
    Anchor = UIElement.AnchorPosition.TopRight,
    Offset = new Vector3(-20, 20, 0)
});
```

### Pause Menu

```csharp
public static void ShowPauseMenu(Game game)
{
    var uiManager = game.Entities
        .First(e => e.TryGetComponent<UIManager>(out _))
        .GetComponent<UIManager>();
    
    uiManager.IsPaused = true;

    // Add menu entities
    var pauseTitle = game.CreateEntity();
    pauseTitle.AddComponent(new TextDisplay("PAUSED")
    {
        FontSize = 48,
        Anchor = UIElement.AnchorPosition.MiddleCenter,
        Offset = new Vector3(0, 50, 0),
        UpdateWhenPaused = true
    });
}
```

### Game Over Screen

```csharp
public static void ShowGameOver(Game game, bool won)
{
    var message = game.CreateEntity();
    message.AddComponent(new TextDisplay(won ? "YOU WIN!" : "GAME OVER")
    {
        FontSize = 48,
        TextColor = won ? "green" : "red",
        Anchor = UIElement.AnchorPosition.MiddleCenter,
        Offset = Vector3.Zero,
        UpdateWhenPaused = true
    });
}
```

## UI Manager Settings

```csharp
var uiManager = game.Entities
    .First(e => e.TryGetComponent<UIManager>(out _))
    .GetComponent<UIManager>();

uiManager.IsPaused = false;           // Pause UI updates
uiManager.ShowAllUI = true;           // Toggle all UI visibility
uiManager.HUDScale = 1.2f;            // 20% larger UI
uiManager.UIOpacity = 0.95f;          // 95% opaque
uiManager.ColorScheme = "dark";       // Color theme
uiManager.ShowDebugUI = false;        // Debug info
```

## Opacity and Fading

All UI elements support opacity:

```csharp
var element = new TextDisplay("Fading Text");
element.Opacity = 1.0f;  // Fully visible
element.Opacity = 0.5f;  // 50% transparent
element.Opacity = 0.0f;  // Fully invisible
```

Damage numbers and popups automatically fade out:

```csharp
// DamageNumber fades from 1.5s to 0
// Alpha = (timeRemaining / 1.5f) * opacity

// ScorePopup fades from 1.0s to 0
// Alpha = (timeRemaining / 1.0f) * opacity
```

## Color Names

Supported color strings:
- "white", "black", "red", "green", "blue"
- "yellow", "cyan", "magenta", "gray"
- "orange", "purple" (implementation-dependent)

## Integration with Other Systems

### With Collectible System

```csharp
// Automatically triggered on pickup:
uiSystem.ShowScorePopup(position, points);
audioSystem.PlaySound("collect_ring", 0.8f);
```

### With Damage System

```csharp
// On entity taking damage:
uiSystem.ShowDamageNumber(position, damage, isCritical: false);
audioSystem.PlaySound("player_hit", 0.9f);
```

### With Dialogue System

```csharp
// Show dialogue speaker name at top
var speakerDisplay = new TextDisplay("NPC Name")
{
    Anchor = UIElement.AnchorPosition.TopCenter,
    FontSize = 18,
    TextColor = "cyan"
};
```

## Advanced Features

### Animation Callbacks

```csharp
var popup = new ScorePopup(points, position);
popup.OnClick = (element) => { /* Handle click */ };
popup.OnHover = (element) => { /* Handle hover */ };
```

### Conditional Visibility

```csharp
healthBar.IsVisible = true;        // Show
healthBar.IsVisible = false;       // Hide
healthBar.UpdateWhenPaused = true; // Update even when paused
```

### Screen Position vs World Position

Health bars automatically convert from world space:
```csharp
healthBar.TargetEntity = player;  // Health bar stays above player
```

Text popups use screen space:
```csharp
damageNumber.Offset = screenPosition;  // Direct screen coords
```

## Extending the System

### Custom UI Elements

```csharp
public class InventoryDisplay : UIElement
{
    public List<InventoryItem> Items { get; set; }
    public int SelectedIndex { get; set; }
}
```

### Custom Renderer Implementation

Implement UI drawing for different engines:

```csharp
public class GodotUIBridge : IEngineRenderer
{
    public void DrawHealthBar(/*...*/) { /* Godot implementation */ }
    public void DrawUIText(/*...*/) { /* Godot implementation */ }
    // ... etc
}
```

## Troubleshooting

### UI Not Appearing
1. Check `UIElement.IsVisible` is true
2. Verify `UIManager.ShowAllUI` is true
3. Ensure UI element has correct anchor and offset
4. Check that renderer implements UI drawing methods

### Text Not Showing
1. Verify text is not empty
2. Check FontSize is reasonable (16-48 typical)
3. Ensure TextColor is a valid color name
4. Verify anchor position is on screen

### Health Bar Not Following Entity
1. Confirm `TargetEntity` is set
2. Check target entity has `Health` component
3. Verify entity has `Transform3D` component

### Popups Disappearing Too Fast
1. Increase `TimeRemaining` value
2. Adjust `FloatSpeed` for faster/slower movement
3. Check `Opacity` calculations in renderer

## Performance Tips

1. **Reuse UI elements** - Don't create new ones every frame
2. **Pool popups** - Recycle damage/score popups
3. **Batch text rendering** - Group text draws together
4. **Use anchoring** - Easier than per-frame positioning
5. **Disable invisible UI** - Set `IsVisible = false` when not needed

## Examples

See [UIExample.cs](UIExample.cs) for complete usage examples including:
- Creating health bars
- Score management
- Damage numbers
- HUD elements (crosshair, minimap)
- Interaction prompts
- Status effect display
- Game over screens
- Pause menus
- Complete game initialization
