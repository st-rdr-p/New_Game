using NUnit.Framework;
using GameCore;
using System.Collections.Generic;

namespace GameTests
{
    /// <summary>
    /// Tests for the Menu System - menu registration, opening/closing, pause mechanics.
    /// </summary>
    [TestFixture]
    public class MenuSystemTests
    {
        private MenuSystem? _menuSystem;
        private Game? _game;

        [SetUp]
        public void Setup()
        {
            // Use a mock game that doesn't require null parameters
            _game = new Game(new MockEngineInput(), new MockEngineAudio());
            _menuSystem = new MenuSystem(_game);
        }

        #region MenuButton Tests

        [Test]
        public void MenuButton_InitializesWithText()
        {
            // Arrange & Act
            var button = new MenuButton("Start Game");

            // Assert
            Assert.That(button.Text, Is.EqualTo("Start Game"));
            Assert.That(button.IsSelected, Is.False);
        }

        [Test]
        public void MenuButton_CallbackOnPressed()
        {
            // Arrange
            bool pressed = false;
            var button = new MenuButton("Options", () => { pressed = true; });

            // Act
            button.OnPressed?.Invoke();

            // Assert
            Assert.That(pressed, Is.True);
        }

        [Test]
        public void MenuButton_CallbackOnSelected()
        {
            // Arrange
            bool selected = false;
            var button = new MenuButton("Resume");
            button.OnSelected = () => { selected = true; };

            // Act
            button.OnSelected?.Invoke();

            // Assert
            Assert.That(selected, Is.True);
        }

        [Test]
        public void MenuButton_SelectionState()
        {
            // Arrange
            var button = new MenuButton("Test Button");

            // Act
            button.IsSelected = true;

            // Assert
            Assert.That(button.IsSelected, Is.True);
        }

        #endregion

        #region MenuComponent Tests

        [Test]
        public void MenuComponent_InitializesWithType()
        {
            // Arrange & Act
            var menu = new MenuComponent(MenuComponent.MenuType.MainMenu, "Main Menu");

            // Assert
            Assert.That(menu.Type, Is.EqualTo(MenuComponent.MenuType.MainMenu));
            Assert.That(menu.Title, Is.EqualTo("Main Menu"));
        }

        [Test]
        public void MenuComponent_InitializesPauseMenu()
        {
            // Arrange & Act
            var menu = new MenuComponent(MenuComponent.MenuType.PauseMenu, "Paused");

            // Assert
            Assert.That(menu.Type, Is.EqualTo(MenuComponent.MenuType.PauseMenu));
            Assert.That(menu.Title, Is.EqualTo("Paused"));
        }

        [Test]
        public void MenuComponent_AddsButtons()
        {
            // Arrange
            var menu = new MenuComponent(MenuComponent.MenuType.MainMenu, "Main");
            var button = new MenuButton("Start Game");

            // Act
            menu.Buttons.Add(button);

            // Assert
            Assert.That(menu.Buttons.Count, Is.EqualTo(1));
            Assert.That(menu.Buttons[0].Text, Is.EqualTo("Start Game"));
        }

        [Test]
        public void MenuComponent_SelectsButtonByIndex()
        {
            // Arrange
            var menu = new MenuComponent(MenuComponent.MenuType.MainMenu, "");
            menu.Buttons.Add(new MenuButton("Button 1"));
            menu.Buttons.Add(new MenuButton("Button 2"));
            menu.Buttons.Add(new MenuButton("Button 3"));

            // Act
            var selectedIndex = menu.SelectedButtonIndex;

            // Assert
            Assert.That(selectedIndex, Is.EqualTo(0)); // Defaults to first
        }

        [Test]
        public void MenuComponent_MultipleMenuTypes()
        {
            // Arrange
            var mainMenu = new MenuComponent(MenuComponent.MenuType.MainMenu, "");
            var pauseMenu = new MenuComponent(MenuComponent.MenuType.PauseMenu, "");
            var settingsMenu = new MenuComponent(MenuComponent.MenuType.Settings, "");

            // Act & Assert
            Assert.That(mainMenu.Type, Is.EqualTo(MenuComponent.MenuType.MainMenu));
            Assert.That(pauseMenu.Type, Is.EqualTo(MenuComponent.MenuType.PauseMenu));
            Assert.That(settingsMenu.Type, Is.EqualTo(MenuComponent.MenuType.Settings));
        }

        #endregion

        #region MenuSystem Initialization Tests

        [Test]
        public void MenuSystem_InitializesWithGame()
        {
            // Arrange & Act
            var system = new MenuSystem(_game!);

            // Assert
            Assert.That(system, Is.Not.Null);
            Assert.That(system.IsGamePaused, Is.False);
        }

        [Test]
        public void MenuSystem_RegisterMenu()
        {
            // Arrange
            var system = new MenuSystem(_game!);
            var menu = new MenuComponent(MenuComponent.MenuType.MainMenu, "Main");

            // Act
            system.RegisterMenu("main", menu);

            // Assert - Menu is registered (verify by trying to open it)
            Assert.DoesNotThrow(() => system.OpenMenu("main"));
        }

        [Test]
        public void MenuSystem_RegisterMultipleMenus()
        {
            // Arrange
            var system = new MenuSystem(_game!);
            var mainMenu = new MenuComponent(MenuComponent.MenuType.MainMenu, "");
            var pauseMenu = new MenuComponent(MenuComponent.MenuType.PauseMenu, "");
            var settingsMenu = new MenuComponent(MenuComponent.MenuType.Settings, "");

            // Act
            system.RegisterMenu("main", mainMenu);
            system.RegisterMenu("pause", pauseMenu);
            system.RegisterMenu("settings", settingsMenu);

            // Assert - All menus registered without error
            Assert.Pass("All menus registered successfully");
        }

        #endregion

        #region MenuSystem Open/Close Tests

        [Test]
        public void MenuSystem_OpenMenuByComponent()
        {
            // Arrange
            var system = new MenuSystem(_game!);
            var menu = new MenuComponent(MenuComponent.MenuType.MainMenu, "Main");

            // Act
            system.OpenMenu(menu);

            // Assert - Menu should be active
            Assert.That(menu.IsActive, Is.True);
        }

        [Test]
        public void MenuSystem_OpenMenuById()
        {
            // Arrange
            var system = new MenuSystem(_game!);
            var menu = new MenuComponent(MenuComponent.MenuType.MainMenu, "");
            system.RegisterMenu("main", menu);

            // Act
            system.OpenMenu("main");

            // Assert - Menu should be active
            Assert.That(menu.IsActive, Is.True);
        }

        [Test]
        public void MenuSystem_CloseCurrentMenu()
        {
            // Arrange
            var system = new MenuSystem(_game!);
            var menu = new MenuComponent(MenuComponent.MenuType.MainMenu, "Main");
            system.OpenMenu(menu);
            Assert.That(menu.IsActive, Is.True);

            // Act
            system.CloseCurrentMenu();

            // Assert
            Assert.That(menu.IsActive, Is.False);
        }

        #endregion

        #region MenuSystem Pause Tests

        [Test]
        public void MenuSystem_PauseGame()
        {
            // Arrange
            var system = new MenuSystem(_game!);

            // Act
            system.PauseGame();

            // Assert
            Assert.That(system.IsGamePaused, Is.True);
        }

        [Test]
        public void MenuSystem_ResumeGame()
        {
            // Arrange
            var system = new MenuSystem(_game!);
            system.PauseGame();
            Assert.That(system.IsGamePaused, Is.True);

            // Act
            system.ResumeGame();

            // Assert
            Assert.That(system.IsGamePaused, Is.False);
        }

        #endregion

        #region MenuSystem Event Tests

        [Test]
        public void MenuSystem_OnMenuOpenedCallback()
        {
            // Arrange
            var system = new MenuSystem(_game!);
            var menu = new MenuComponent(MenuComponent.MenuType.MainMenu, "");
            bool menuOpenedCalled = false;

            system.OnMenuOpened += (m) => { menuOpenedCalled = true; };

            // Act
            system.OpenMenu(menu);

            // Assert
            Assert.That(menuOpenedCalled, Is.True);
        }

        [Test]
        public void MenuSystem_OnMenuClosedCallback()
        {
            // Arrange
            var system = new MenuSystem(_game!);
            var menu = new MenuComponent(MenuComponent.MenuType.MainMenu, "");
            bool menuClosedCalled = false;

            system.OnMenuClosed += (m) => { menuClosedCalled = true; };

            system.OpenMenu(menu);

            // Act
            system.CloseCurrentMenu();

            // Assert
            Assert.That(menuClosedCalled, Is.True);
        }

        [Test]
        public void MenuSystem_OnGamePausedCallback()
        {
            // Arrange
            var system = new MenuSystem(_game!);
            bool gamePausedCalled = false;

            system.OnGamePaused += () => { gamePausedCalled = true; };

            // Act
            system.PauseGame();

            // Assert
            Assert.That(gamePausedCalled, Is.True);
        }

        [Test]
        public void MenuSystem_OnGameResumedCallback()
        {
            // Arrange
            var system = new MenuSystem(_game!);
            system.PauseGame();
            bool gameResumedCalled = false;

            system.OnGameResumed += () => { gameResumedCalled = true; };

            // Act
            system.ResumeGame();

            // Assert
            Assert.That(gameResumedCalled, Is.True);
        }

        #endregion

        #region MenuSystem Update Tests

        [Test]
        public void MenuSystem_UpdateDoesNotThrow()
        {
            // Arrange
            var system = new MenuSystem(_game!);
            var entities = new List<Entity>();

            // Act & Assert
            Assert.DoesNotThrow(() => system.Update(0.016f, entities));
        }

        #endregion

        #region Integration Tests

        [Test]
        public void Menu_PauseGameFlow()
        {
            // Arrange
            var system = new MenuSystem(_game!);
            var pauseMenu = new MenuComponent(MenuComponent.MenuType.PauseMenu, "Paused");
            pauseMenu.Buttons.Add(new MenuButton("Resume"));
            pauseMenu.Buttons.Add(new MenuButton("Settings"));
            pauseMenu.Buttons.Add(new MenuButton("Quit"));

            system.RegisterMenu("pause", pauseMenu);

            // Act - Pause and show menu
            system.PauseGame();
            system.OpenMenu("pause");

            // Assert
            Assert.That(system.IsGamePaused, Is.True);
            Assert.That(pauseMenu.IsActive, Is.True);
            Assert.That(pauseMenu.Buttons.Count, Is.EqualTo(3));
        }

        [Test]
        public void Menu_ResumeGameFlow()
        {
            // Arrange
            var system = new MenuSystem(_game!);
            var pauseMenu = new MenuComponent(MenuComponent.MenuType.PauseMenu, "");
            system.RegisterMenu("pause", pauseMenu);

            system.PauseGame();
            system.OpenMenu("pause");

            // Act - Close menu and resume
            system.CloseCurrentMenu();
            system.ResumeGame();

            // Assert
            Assert.That(system.IsGamePaused, Is.False);
            Assert.That(pauseMenu.IsActive, Is.False);
        }

        [Test]
        public void Menu_MainMenuWithButtons()
        {
            // Arrange
            var system = new MenuSystem(_game!);
            var mainMenu = new MenuComponent(MenuComponent.MenuType.MainMenu, "Main Menu");

            bool newGameClicked = false;
            bool quitClicked = false;

            mainMenu.Buttons.Add(new MenuButton("New Game", () => { newGameClicked = true; }));
            mainMenu.Buttons.Add(new MenuButton("Load Game"));
            mainMenu.Buttons.Add(new MenuButton("Settings"));
            mainMenu.Buttons.Add(new MenuButton("Quit", () => { quitClicked = true; }));

            system.RegisterMenu("main", mainMenu);

            // Act
            system.OpenMenu("main");
            mainMenu.Buttons[0].OnPressed?.Invoke();
            mainMenu.Buttons[3].OnPressed?.Invoke();

            // Assert
            Assert.That(mainMenu.IsActive, Is.True);
            Assert.That(mainMenu.Buttons.Count, Is.EqualTo(4));
            Assert.That(newGameClicked, Is.True);
            Assert.That(quitClicked, Is.True);
        }

        [Test]
        public void Menu_SettingsMenuWithSelection()
        {
            // Arrange
            var system = new MenuSystem(_game!);
            var settingsMenu = new MenuComponent(MenuComponent.MenuType.Settings, "Settings");

            var volumeButton = new MenuButton("Volume: 80%");
            var brightnessButton = new MenuButton("Brightness: 100%");
            var difficultyButton = new MenuButton("Difficulty: Normal");

            settingsMenu.Buttons.Add(volumeButton);
            settingsMenu.Buttons.Add(brightnessButton);
            settingsMenu.Buttons.Add(difficultyButton);

            // Act
            system.RegisterMenu("settings", settingsMenu);
            system.OpenMenu("settings");
            volumeButton.IsSelected = true;

            // Assert
            Assert.That(volumeButton.IsSelected, Is.True);
            Assert.That(settingsMenu.Buttons[0].Text, Is.EqualTo("Volume: 80%"));
        }

        #endregion

        #region Mock Classes for Testing

        private class MockEngineInput : IEngineInput
        {
            public bool IsKeyDown(string key) => false;
            public float GetAxis(string axis) => 0f;
            public float GetMouseX() => 0f;
            public float GetMouseY() => 0f;
            public void LockMouse(bool locked) { }
            public bool IsMouseLocked => false;
        }

        private class MockEngineRenderer : IEngineRenderer
        {
            public void DrawSprite(string spriteId, float x, float y, float rotation, float scaleX = 1.0f, float scaleY = 1.0f, float opacity = 1.0f) { }
            public void DrawScreenFlash(float intensity, string color = "red") { }
            public void ApplyRetroGraphics(RetroGraphicsEffect effect) { }
            public int GetScreenWidth() => 1920;
            public int GetScreenHeight() => 1080;
            public void DrawHealthBar(Vector3 worldPosition, float currentHealth, float maxHealth, float width, float height, string fillColor = "green", string backgroundColor = "gray", float opacity = 1.0f) { }
            public void DrawUIText(string text, UIElement.AnchorPosition anchor, Vector3 offset, int fontSize, string color, float opacity) { }
            public void DrawCrosshair(string style, float size, string color, float thickness, float opacity) { }
            public void DrawMinimap(float size, float worldRange, UIElement.AnchorPosition anchor, Vector3 offset, string backgroundColor, string borderColor, float opacity) { }
        }

        #endregion
    }
}
