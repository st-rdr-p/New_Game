# Input System Guide

Complete input system for handling keyboard input in your game. Wires menus, dialogue, save/load, and player movement to keyboard controls.

## Overview

### InputManager
Core input manager that:
- Maps console keys to InputActions
- Tracks pressed/held state for each action
- Provides event callbacks for input events
- Supports rebinding keys

### Input Handlers
Specialized handlers that bridge the input system with game systems:
- **MenuInputHandler** - Navigate menus with arrow keys
- **DialogueInputHandler** - Advance dialogue and select choices
- **SaveLoadInputHandler** - Quick save/load with F5/F9
- **PlayerInputHandler** - Movement with WASD/arrows

## Default Key Bindings

### Menu Navigation
- **Up/Down** - ↑↓ or W/S
- **Select** - Enter
- **Cancel** - Escape
- **Back** - Backspace

### Dialogue
- **Next/Advance** - Enter or Spacebar
- **Choice Up/Down** - ↑↓ or W/S
- **Choice Select** - Enter
- **Skip** - Escape

### Player Movement
- **Move** - WASD or Arrow Keys
- **Jump** - Spacebar
- **Dash** - Left Shift

### Game Control
- **Pause** - P or Escape
- **Save** - F5
- **Load** - F9
- **Quick Save** - F6
- **Quick Load** - F10

## Setup Example

```csharp
// Create input manager
var inputManager = new InputManager();

// Create menu input handler
var menuInputHandler = new MenuInputHandler(inputManager);

// Create your menu
var menu = new MenuComponent { MenuId = "main_menu" };
menu.AddButton(new MenuButton { Text = "Start", Index = 0 });
menu.AddButton(new MenuButton { Text = "Quit", Index = 1 });

// Wire up the menu
menuInputHandler.SetActiveMenu(menu);

// Handle menu events
menuInputHandler.OnMenuItemSelected += (button) =>
{
    Console.WriteLine($"Selected: {button.Text}");
};

// Game loop
while (true)
{
    menuInputHandler.Update();
}
```

## Dialogue Example

```csharp
var inputManager = new InputManager();
var dialogueInputHandler = new DialogueInputHandler(inputManager);

// Create dialogue
var dialogue = new DialogueComponent();
dialogue.Sequences = new[] {
    new DialogueSequence { 
        Lines = new[] { new DialogueLine { Text = "Hello!" } },
        Choices = new[] { 
            new DialogueChoice { Text = "Hi", NextSequenceIndex = 1 }
        }
    }
};

dialogue.StartDialogue(0);
dialogueInputHandler.SetActiveDialogue(dialogue);

dialogueInputHandler.OnChoiceSelected += (choiceIndex) =>
{
    Console.WriteLine($"Player chose option {choiceIndex}");
};

while (dialogue.IsDialogueActive)
{
    dialogueInputHandler.Update();
}
```

## Save/Load Example

```csharp
var inputManager = new InputManager();
var saveLoadSystem = new SaveLoadSystem(game);
var saveLoadInputHandler = new SaveLoadInputHandler(inputManager, saveLoadSystem);

// F5 saves
saveLoadInputHandler.OnSaveRequested += (slotIndex) =>
{
    saveLoadSystem.SaveGame(slotIndex, "Auto Save", entities);
};

// F9 loads
saveLoadInputHandler.OnLoadRequested += (slotIndex) =>
{
    saveLoadSystem.LoadGame(slotIndex, entities);
};

// Game loop
while (playing)
{
    saveLoadInputHandler.Update();
}
```

## Player Movement Example

```csharp
var inputManager = new InputManager();
var playerInputHandler = new PlayerInputHandler(inputManager);

playerInputHandler.OnMovementInput += (direction) =>
{
    if (direction.Magnitude > 0)
    {
        // Move player in this direction
        player.Move(direction * speed * deltaTime);
    }
};

playerInputHandler.OnJumpPressed += () =>
{
    player.Jump();
};

while (gameRunning)
{
    playerInputHandler.Update();
}
```

## Remapping Keys

```csharp
var inputManager = new InputManager();

// Change a specific key
inputManager.RemapKey(InputAction.MenuSelect, ConsoleKey.Enter, ConsoleKey.Spacebar);

// Or set multiple keys for one action
inputManager.SetKeyBinding(InputAction.Jump, ConsoleKey.Spacebar, ConsoleKey.W);

// Reset to defaults
inputManager.ResetToDefaults();
```

## Integration Flow

```
InputManager
    ↓
    ├→ MenuInputHandler → MenuComponent → UI Updates
    ├→ DialogueInputHandler → DialogueComponent → Text Display
    ├→ SaveLoadInputHandler → SaveLoadSystem → File I/O
    └→ PlayerInputHandler → Player Entity → Movement
```

## Testing Input

The system supports testing by:
1. Checking `IsActionPressed(action)` for frame-perfect input
2. Checking `IsActionHeld(action)` for continuous input
3. Using `OnActionPressed` events for one-time triggers
4. Accessing `GetLastKeyPressed()` for debugging

## Advanced Usage

### Custom Actions

Add your own InputAction enum values:
```csharp
public enum InputAction
{
    // ... existing actions ...
    CustomAction1,
    CustomAction2
}

// Then bind keys to them
inputManager.SetKeyBinding(InputAction.CustomAction1, ConsoleKey.E);
```

### Key Rebinding Menu

```csharp
public void RebindKey(InputAction action, string friendlyName)
{
    Console.WriteLine($"Press the key for {friendlyName}...");
    var keyInfo = Console.ReadKey(true);
    inputManager.SetKeyBinding(action, keyInfo.Key);
    Console.WriteLine($"Rebound to {keyInfo.Key}");
}
```

### Input Profiles

Save/load different control schemes:
```csharp
public void SaveInputProfile(string profileName)
{
    // Serialize _keyBindings to JSON
}

public void LoadInputProfile(string profileName)
{
    // Deserialize JSON back to _keyBindings
}
```
