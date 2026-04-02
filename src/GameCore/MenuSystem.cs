using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// Manages all menus in the game - main menu, pause menu, dialogs, etc.
    /// </summary>
    public class MenuSystem : ISystem
    {
        /// <summary>Currently active menu (if any).</summary>
        private MenuComponent _activeMenu;

        /// <summary>Stack of menus for proper back navigation.</summary>
        private Stack<MenuComponent> _menuStack = new();

        /// <summary>All menus in the game.</summary>
        private Dictionary<string, MenuComponent> _menus = new();

        /// <summary>Game reference for pausing.</summary>
        private Game _game;

        /// <summary>Whether the game is paused.</summary>
        public bool IsGamePaused { get; private set; } = false;

        /// <summary>Callback when menu is opened.</summary>
        public Action<MenuComponent> OnMenuOpened { get; set; }

        /// <summary>Callback when menu is closed.</summary>
        public Action<MenuComponent> OnMenuClosed { get; set; }

        /// <summary>Callback for menu button pressed.</summary>
        public Action<MenuComponent, int> OnMenuButtonPressed { get; set; }

        /// <summary>Callback when game is paused.</summary>
        public Action OnGamePaused { get; set; }

        /// <summary>Callback when game is resumed.</summary>
        public Action OnGameResumed { get; set; }

        public MenuSystem(Game game)
        {
            _game = game;
        }

        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            // Menu system runs even when game is paused
            // Input handling would go here in a real implementation
        }

        /// <summary>Register a menu in the system.</summary>
        public void RegisterMenu(string menuId, MenuComponent menu)
        {
            _menus[menuId] = menu;
        }

        /// <summary>Open a menu by ID.</summary>
        public void OpenMenu(string menuId)
        {
            if (!_menus.TryGetValue(menuId, out var menu))
            {
                Console.WriteLine($"Menu not found: {menuId}");
                return;
            }

            OpenMenu(menu);
        }

        /// <summary>Open a menu (directly).</summary>
        public void OpenMenu(MenuComponent menu)
        {
            // Save current menu to stack
            if (_activeMenu != null)
            {
                _menuStack.Push(_activeMenu);
                _activeMenu.IsActive = false;
            }

            // Open new menu
            _activeMenu = menu;
            _activeMenu.Open();

            // Pause game if menu requires it
            if (_activeMenu.PausesGame && !IsGamePaused)
            {
                PauseGame();
            }

            OnMenuOpened?.Invoke(_activeMenu);
        }

        /// <summary>Close the current menu and go back to previous (if any).</summary>
        public void CloseCurrentMenu()
        {
            if (_activeMenu == null)
                return;

            _activeMenu.Close();
            OnMenuClosed?.Invoke(_activeMenu);

            // Pop previous menu from stack
            if (_menuStack.Count > 0)
            {
                _activeMenu = _menuStack.Pop();
                _activeMenu.Open();

                // Keep paused if this menu also pauses game
                if (!_activeMenu.PausesGame && IsGamePaused)
                {
                    ResumeGame();
                }
            }
            else
            {
                _activeMenu = null;

                // Resume game if no more menus
                if (IsGamePaused)
                {
                    ResumeGame();
                }
            }
        }

        /// <summary>Pause the game.</summary>
        public void PauseGame()
        {
            if (IsGamePaused)
                return;

            IsGamePaused = true;
            OnGamePaused?.Invoke();
        }

        /// <summary>Resume the game.</summary>
        public void ResumeGame()
        {
            if (!IsGamePaused)
                return;

            IsGamePaused = false;
            OnGameResumed?.Invoke();
        }

        /// <summary>Handle menu navigation input (up/down).</summary>
        public void NavigateUp()
        {
            if (_activeMenu?.AllowInput == true)
            {
                _activeMenu.SelectPrevious();
            }
        }

        /// <summary>Handle menu navigation input (down).</summary>
        public void NavigateDown()
        {
            if (_activeMenu?.AllowInput == true)
            {
                _activeMenu.SelectNext();
            }
        }

        /// <summary>Handle menu selection input (confirm).</summary>
        public void PressSelectedButton()
        {
            if (_activeMenu?.AllowInput == true)
            {
                _activeMenu.PressSelectedButton();
                OnMenuButtonPressed?.Invoke(_activeMenu, _activeMenu.SelectedButtonIndex);
            }
        }

        /// <summary>Handle back input (close current menu).</summary>
        public void Back()
        {
            CloseCurrentMenu();
        }

        /// <summary>Get the currently active menu.</summary>
        public MenuComponent GetActiveMenu() => _activeMenu;

        /// <summary>Check if a menu is currently active.</summary>
        public bool IsMenuActive(string menuId)
        {
            return _activeMenu != null && _menus.TryGetValue(menuId, out var menu) && menu == _activeMenu;
        }

        /// <summary>Check if any menu is active.</summary>
        public bool IsAnyMenuActive => _activeMenu != null;
    }

    /// <summary>
    /// Builder for creating menus fluently.
    /// </summary>
    public class MenuBuilder
    {
        private MenuComponent _menu;

        public MenuBuilder(MenuComponent.MenuType type, string title)
        {
            _menu = new MenuComponent(type, title);
        }

        /// <summary>Add a button to the menu.</summary>
        public MenuBuilder Button(string text, Action onPressed, Action onSelected = null)
        {
            var button = new MenuButton(text, onPressed) { OnSelected = onSelected };
            _menu.AddButton(button);
            return this;
        }

        /// <summary>Set whether menu pauses the game.</summary>
        public MenuBuilder PausesGame(bool pauses)
        {
            _menu.PausesGame = pauses;
            return this;
        }

        /// <summary>Set the menu's open callback.</summary>
        public MenuBuilder OnOpen(Action callback)
        {
            _menu.OnMenuOpen = callback;
            return this;
        }

        /// <summary>Set the menu's close callback.</summary>
        public MenuBuilder OnClose(Action callback)
        {
            _menu.OnMenuClose = callback;
            return this;
        }

        /// <summary>Build the menu.</summary>
        public MenuComponent Build()
        {
            return _menu;
        }

        /// <summary>Build and register the menu.</summary>
        public MenuComponent BuildFor(MenuSystem menuSystem, string menuId)
        {
            menuSystem.RegisterMenu(menuId, _menu);
            return _menu;
        }
    }
}
