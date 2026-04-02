# Menu System Guide

Complete guide for creating and using menus (main menu, pause menu, settings, dialogs) in your game.

## Overview

The menu system provides:
- **Main Menu** - Game start menu
- **Pause Menu** - In-game pause/resume
- **Settings Menu** - Audio, video, controls
- **Dialogs** - Yes/No confirmations
- **Custom Menus** - Any other menu type
- **Game Pausing** - Automatic pause when menu opens
- **Menu Stacking** - Back navigation through menus

## Quick Start

### Create a Simple Pause Menu

```csharp
var menuSystem = new MenuSystem(game);

var pauseMenu = new MenuBuilder(MenuComponent.MenuType.PauseMenu, "Paused")
    .Button("Resume", () => menuSystem.CloseCurrentMenu())
    .Button("Settings", () => menuSystem.OpenMenu("settings"))
    .Button("Quit", () => { /* quit code */ })
    .PausesGame(true)
    .BuildFor(menuSystem, "pause_menu");

// When player presses pause:
menuSystem.OpenMenu("pause_menu");
```

### Create a Main Menu

```csharp
var mainMenu = new MenuBuilder(MenuComponent.MenuType.MainMenu, "Main Menu")
    .Button("New Game", () =>
    {
        menuSystem.ResumeGame();
        menuSystem.CloseCurrentMenu();
    })
    .Button("Continue", () =>
    {
        menuSystem.ResumeGame();
        menuSystem.CloseCurrentMenu();
    })
    .Button("Settings", () => menuSystem.OpenMenu("settings"))
    .Button("Quit", () => { /* quit */ })
    .BuildFor(menuSystem, "main_menu");
```

## Core Components

### MenuComponent

The main menu component attached to entities.

**Properties:**
- `Type` - MenuType (MainMenu, PauseMenu, Settings, ConfirmDialog, CustomMenu)
- `Title` - Menu title text
- `Buttons` - List of MenuButton objects
- `SelectedButtonIndex` - Currently selected button
- `IsActive` - Whether menu is displayed
- `PausesGame` - Auto-pause game when opened
- `AllowInput` - Allow keyboard/controller input

**Methods:**
- `AddButton(MenuButton)` - Add a button
- `SelectButton(int)` - Select button by index
- `SelectNext()` / `SelectPrevious()` - Navigate buttons
- `PressSelectedButton()` - Activate selected button
- `Open()` / `Close()` - Show/hide menu
- `GetSelectedButton()` - Get current button

### MenuSystem

Manages all menus and game pause state.

**Properties:**
- `IsGamePaused` - Is game currently paused
- `GetActiveMenu()` - Get current menu
- `IsAnyMenuActive` - Is any menu open

**Methods:**
- `RegisterMenu(id, menu)` - Register a menu
- `OpenMenu(id/menu)` - Open a menu
- `CloseCurrentMenu()` - Close current menu (pop stack)
- `PauseGame()` / `ResumeGame()` - Pause/resume
- `NavigateUp()` / `NavigateDown()` - Menu navigation
- `PressSelectedButton()` - Select menu option
- `Back()` - Go back/close menu

**Callbacks:**
- `OnMenuOpened(menu)` - When menu opens
- `OnMenuClosed(menu)` - When menu closes
- `OnGamePaused()` - When game pauses
- `OnGameResumed()` - When game resumes

### MenuButton

A single button in a menu.

**Properties:**
- `Text` - Button display text
- `IsSelected` - Currently highlighted

**Callbacks:**
- `OnPressed` - Button clicked
- `OnSelected` - Button highlighted

## Complete Example

```csharp
public class GameWithMenus : Game
{
    private MenuSystem _menuSystem;

    protected override void OnInitialize()
    {
        // Create menu system
        _menuSystem = new MenuSystem(this);
        AddSystem(_menuSystem);

        // Create main menu
        new MenuBuilder(MenuComponent.MenuType.MainMenu, "Main Menu")
            .Button("New Game", StartNewGame)
            .Button("Continue", LoadGame)
            .Button("Settings", () => _menuSystem.OpenMenu("settings"))
            .Button("Quit", QuitGame)
            .BuildFor(_menuSystem, "main_menu");

        // Create pause menu
        new MenuBuilder(MenuComponent.MenuType.PauseMenu, "Paused")
            .Button("Resume", () => _menuSystem.CloseCurrentMenu())
            .Button("Settings", () => _menuSystem.OpenMenu("settings"))
            .Button("Main Menu", ConfirmMainMenu)
            .Button("Quit", QuitGame)
            .BuildFor(_menuSystem, "pause_menu");

        // Create settings menu
        new MenuBuilder(MenuComponent.MenuType.Settings, "Settings")
            .Button("Audio", () => { })
            .Button("Video", () => { })
            .Button("Controls", () => { })
            .Button("Back", () => _menuSystem.CloseCurrentMenu())
            .BuildFor(_menuSystem, "settings");

        // Open main menu at start
        _menuSystem.OpenMenu("main_menu");
    }

    private void StartNewGame()
    {
        Console.WriteLine("Starting new game...");
        _menuSystem.ResumeGame();
        _menuSystem.CloseCurrentMenu();
    }

    private void LoadGame()
    {
        Console.WriteLine("Loading game...");
        _menuSystem.ResumeGame();
        _menuSystem.CloseCurrentMenu();
    }

    private void ConfirmMainMenu()
    {
        Console.WriteLine("Returning to main menu...");
        _menuSystem.CloseCurrentMenu();
        _menuSystem.OpenMenu("main_menu");
    }

    private void QuitGame()
    {
        Console.WriteLine("Quitting game...");
        System.Environment.Exit(0);
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        // Handle menu input
        HandleMenuInput();
    }

    private void HandleMenuInput()
    {
        // This would connect to your actual input system
        // Example pseudo-code:
        
        // if (Input.GetKeyDown(KeyCode.Escape))
        //     _menuSystem.Back();
        // 
        // if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Up))
        //     _menuSystem.NavigateUp();
        // 
        // if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Down))
        //     _menuSystem.NavigateDown();
        // 
        // if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        //     _menuSystem.PressSelectedButton();
    }
}
```

## Menu Types

### MainMenu
Opening menu for new games. Typically includes:
- New Game
- Continue/Load
- Settings
- Credits
- Quit

```csharp
new MenuBuilder(MenuComponent.MenuType.MainMenu, "Main Menu")
    .Button("New Game", StartGame)
    .Button("Settings", () => { })
    .Button("Quit", QuitGame)
    .BuildFor(menuSystem, "main_menu");
```

### PauseMenu
Opened when player pauses during gameplay. Typically includes:
- Resume
- Save
- Settings
- Return to Main Menu
- Quit

```csharp
new MenuBuilder(MenuComponent.MenuType.PauseMenu, "Paused")
    .Button("Resume", () => menuSystem.CloseCurrentMenu())
    .Button("Save", SaveGame)
    .Button("Settings", () => menuSystem.OpenMenu("settings"))
    .Button("Main Menu", () => menuSystem.CloseCurrentMenu())
    .BuildFor(menuSystem, "pause_menu");
```

### Settings
Settings menu with submenus. Can be nested.

```csharp
new MenuBuilder(MenuComponent.MenuType.Settings, "Settings")
    .Button("Audio", () => menuSystem.OpenMenu("audio_settings"))
    .Button("Video", () => menuSystem.OpenMenu("video_settings"))
    .Button("Back", () => menuSystem.CloseCurrentMenu())
    .BuildFor(menuSystem, "settings");
```

### ConfirmDialog
Yes/No confirmation dialogs.

```csharp
new MenuBuilder(MenuComponent.MenuType.ConfirmDialog, "Confirm")
    .Button("Yes", OnConfirm)
    .Button("No", OnCancel)
    .BuildFor(menuSystem, "confirm");
```

### CustomMenu
Any other menu type for custom UIs.

```csharp
new MenuBuilder(MenuComponent.MenuType.CustomMenu, "Custom")
    .Button("Option 1", () => { })
    .Button("Option 2", () => { })
    .BuildFor(menuSystem, "custom");
```

## Menu Callbacks

### Menu Callbacks

```csharp
var menu = new MenuBuilder(MenuComponent.MenuType.PauseMenu, "Paused")
    .OnOpen(() => Console.WriteLine("Menu opened"))
    .OnClose(() => Console.WriteLine("Menu closed"))
    .Button("Resume", () => menuSystem.CloseCurrentMenu())
    .Build();

// Manual callbacks
menu.OnMenuOpen += () => PlayMenuSound();
menu.OnMenuClose += () => ResumeMusic();
menu.OnButtonSelected += (index) => HighlightButton(index);
menu.OnButtonPressed += (index) => PlayClickSound();
```

### System Callbacks

```csharp
menuSystem.OnMenuOpened += (menu) => Debug.Log($"Opened: {menu.Title}");
menuSystem.OnMenuClosed += (menu) => Debug.Log($"Closed: {menu.Title}");
menuSystem.OnGamePaused += () => FreezePhysics();
menuSystem.OnGameResumed += () => ResumePhysics();
menuSystem.OnMenuButtonPressed += (menu, index) => PlayUISound();
```

## Input Handling

### Connect to Input System

```csharp
public class InputSystem
{
    private MenuSystem _menuSystem;

    public InputSystem(MenuSystem menuSystem)
    {
        _menuSystem = menuSystem;
    }

    public void HandleInput(string input)
    {
        switch (input.ToLower())
        {
            case "up":
                _menuSystem.NavigateUp();
                break;
            case "down":
                _menuSystem.NavigateDown();
                break;
            case "select":
                _menuSystem.PressSelectedButton();
                break;
            case "back":
            case "escape":
                _menuSystem.Back();
                break;
            case "pause":
                if (_menuSystem.IsGamePaused)
                    _menuSystem.CloseCurrentMenu();
                else
                    _menuSystem.OpenMenu("pause_menu");
                break;
        }
    }
}
```

### In Update Loop

```csharp
protected override void OnUpdate(float deltaTime)
{
    if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
    {
        if (_menuSystem.IsGamePaused)
            _menuSystem.CloseCurrentMenu();
        else
            _menuSystem.OpenMenu("pause_menu");
    }

    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        _menuSystem.NavigateUp();

    if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        _menuSystem.NavigateDown();

    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        _menuSystem.PressSelectedButton();
}
```

## Menu Stacking

Menus automatically stack for back navigation:

```csharp
// Open main menu
menuSystem.OpenMenu("main_menu");

// Player opens settings from main menu
menuSystem.OpenMenu("settings");
// Stack: [main_menu, settings]

// Player opens audio from settings
menuSystem.OpenMenu("audio_settings");
// Stack: [main_menu, settings, audio_settings]

// Player presses back
menuSystem.Back();
// Returns to settings
// Stack: [main_menu, settings]

// Player presses back again
menuSystem.Back();
// Returns to main_menu
// Stack: [main_menu]
```

## Game Pausing

Menus can automatically pause the game:

```csharp
// This menu pauses the game
new MenuBuilder(MenuComponent.MenuType.PauseMenu, "Paused")
    .PausesGame(true)  // Game paused when opened
    .Button("Resume", () => menuSystem.CloseCurrentMenu())
    .BuildFor(menuSystem, "pause_menu");

// This menu doesn't pause
new MenuBuilder(MenuComponent.MenuType.MainMenu, "Main Menu")
    .PausesGame(false)  // Game not paused
    .Button("New Game", StartGame)
    .BuildFor(menuSystem, "main_menu");

// Check pause state
if (menuSystem.IsGamePaused)
{
    // Disable physics, movement, etc
}
```

## UI Integration (Unity Example)

```csharp
public class MenuUIRenderer : MonoBehaviour
{
    private MenuSystem _menuSystem;
    private Canvas _menuCanvas;
    private Text _titleText;
    private VerticalLayoutGroup _buttonsContainer;

    void Start()
    {
        _menuSystem = GameManager.Instance.MenuSystem;
        _menuSystem.OnMenuOpened += RenderMenu;
        _menuSystem.OnMenuClosed += HideMenu;
    }

    void RenderMenu(MenuComponent menu)
    {
        _menuCanvas.enabled = true;
        _titleText.text = menu.Title;

        // Clear old buttons
        foreach (Transform child in _buttonsContainer.transform)
            Destroy(child.gameObject);

        // Create new buttons
        for (int i = 0; i < menu.Buttons.Count; i++)
        {
            var buttonObj = Instantiate(buttonPrefab, _buttonsContainer.transform);
            var buttonText = buttonObj.GetComponentInChildren<Text>();
            buttonText.text = menu.Buttons[i].Text;

            var button = buttonObj.GetComponent<Button>();
            int index = i;
            button.onClick.AddListener(() => _menuSystem.SelectButton(index));
        }
    }

    void HideMenu(MenuComponent menu)
    {
        _menuCanvas.enabled = false;
    }

    void Update()
    {
        if (_menuSystem.IsAnyMenuActive)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                _menuSystem.NavigateUp();
            if (Input.GetKeyDown(KeyCode.DownArrow))
                _menuSystem.NavigateDown();
            if (Input.GetKeyDown(KeyCode.Return))
                _menuSystem.PressSelectedButton();
            if (Input.GetKeyDown(KeyCode.Escape))
                _menuSystem.Back();
        }
    }
}
```

## Best Practices

1. **Register all menus early** - Set up all menus in initialization
2. **Use menu IDs** - Reference menus by string ID, not direct reference
3. **Set up callbacks** - Use OnOpen/OnClose for UI sync
4. **Handle back button** - Always provide a "Back" option
5. **Pause game appropriately** - Only pause when needed
6. **Audio feedback** - Play sounds on navigation and selection
7. **Validate state** - Check IsGamePaused before gameplay operations
8. **Stack menus properly** - Use Back() instead of directly opening previous menu

---

**See MenuExamples.cs for complete working examples!**
