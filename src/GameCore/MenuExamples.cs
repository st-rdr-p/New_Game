using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// Examples of creating and using main menu and pause menu.
    /// </summary>
    public class MenuExamples
    {
        /// <summary>
        /// Create a main menu.
        /// </summary>
        public static MenuComponent CreateMainMenu(MenuSystem menuSystem)
        {
            var menu = new MenuComponent(MenuComponent.MenuType.MainMenu, "Main Menu")
            {
                PausesGame = true,
                OnMenuOpen = () => Console.WriteLine("Main menu opened"),
                OnMenuClose = () => Console.WriteLine("Main menu closed")
            };

            menu.AddButton(new MenuButton("New Game", () =>
            {
                Console.WriteLine("Starting new game...");
                menuSystem.ResumeGame();
                menuSystem.CloseCurrentMenu();
            }));

            menu.AddButton(new MenuButton("Continue Game", () =>
            {
                Console.WriteLine("Loading saved game...");
                menuSystem.ResumeGame();
                menuSystem.CloseCurrentMenu();
            }));

            menu.AddButton(new MenuButton("Settings", () =>
            {
                Console.WriteLine("Opening settings...");
                menuSystem.OpenMenu("settings_menu");
            }));

            menu.AddButton(new MenuButton("Credits", () =>
            {
                Console.WriteLine("Showing credits...");
                menuSystem.OpenMenu("credits_menu");
            }));

            menu.AddButton(new MenuButton("Quit Game", () =>
            {
                Console.WriteLine("Quit confirmed");
            }));

            menuSystem.RegisterMenu("main_menu", menu);
            return menu;
        }

        /// <summary>
        /// Create a pause menu.
        /// </summary>
        public static MenuComponent CreatePauseMenu(MenuSystem menuSystem)
        {
            var menu = new MenuComponent(MenuComponent.MenuType.PauseMenu, "Paused")
            {
                PausesGame = true,
                OnMenuOpen = () => Console.WriteLine("Pause menu opened"),
                OnMenuClose = () => Console.WriteLine("Game resumed")
            };

            menu.AddButton(new MenuButton("Resume Game", () =>
            {
                Console.WriteLine("Resuming game...");
                menuSystem.CloseCurrentMenu();
            }));

            menu.AddButton(new MenuButton("Settings", () =>
            {
                Console.WriteLine("Opening settings from pause...");
                menuSystem.OpenMenu("settings_menu");
            }));

            menu.AddButton(new MenuButton("Save Game", () =>
            {
                Console.WriteLine("Game saved!");
            }));

            menu.AddButton(new MenuButton("Return to Main Menu", () =>
            {
                Console.WriteLine("Confirming return to main menu...");
                menuSystem.OpenMenu("confirm_menu");
            }));

            menu.AddButton(new MenuButton("Quit Game", () =>
            {
                Console.WriteLine("Quit confirmed");
            }));

            menuSystem.RegisterMenu("pause_menu", menu);
            return menu;
        }

        /// <summary>
        /// Create a settings menu.
        /// </summary>
        public static MenuComponent CreateSettingsMenu(MenuSystem menuSystem)
        {
            var menu = new MenuComponent(MenuComponent.MenuType.Settings, "Settings")
            {
                PausesGame = true,
                OnMenuOpen = () => Console.WriteLine("Settings menu opened"),
                OnMenuClose = () => Console.WriteLine("Settings menu closed")
            };

            menu.AddButton(new MenuButton("Audio Settings", () =>
            {
                Console.WriteLine("Opening audio settings...");
            }));

            menu.AddButton(new MenuButton("Video Settings", () =>
            {
                Console.WriteLine("Opening video settings...");
            }));

            menu.AddButton(new MenuButton("Controls", () =>
            {
                Console.WriteLine("Opening control settings...");
            }));

            menu.AddButton(new MenuButton("Back", () =>
            {
                Console.WriteLine("Going back from settings...");
                menuSystem.CloseCurrentMenu();
            }));

            menuSystem.RegisterMenu("settings_menu", menu);
            return menu;
        }

        /// <summary>
        /// Create a confirmation dialog.
        /// </summary>
        public static MenuComponent CreateConfirmDialog(MenuSystem menuSystem)
        {
            var menu = new MenuComponent(MenuComponent.MenuType.ConfirmDialog, "Confirm")
            {
                PausesGame = true
            };

            menu.AddButton(new MenuButton("Yes", () =>
            {
                Console.WriteLine("User confirmed");
                menuSystem.CloseCurrentMenu();
                menuSystem.OpenMenu("main_menu");
            }));

            menu.AddButton(new MenuButton("No", () =>
            {
                Console.WriteLine("User cancelled");
                menuSystem.CloseCurrentMenu();
            }));

            menuSystem.RegisterMenu("confirm_menu", menu);
            return menu;
        }

        /// <summary>
        /// Create a credits menu.
        /// </summary>
        public static MenuComponent CreateCreditsMenu(MenuSystem menuSystem)
        {
            var menu = new MenuComponent(MenuComponent.MenuType.CustomMenu, "Credits")
            {
                PausesGame = true,
                OnMenuOpen = () => Console.WriteLine("Credits menu opened")
            };

            menu.AddButton(new MenuButton("Back to Main Menu", () =>
            {
                Console.WriteLine("Returning to main menu from credits...");
                menuSystem.CloseCurrentMenu();
            }));

            menuSystem.RegisterMenu("credits_menu", menu);
            return menu;
        }

        /// <summary>
        /// Setup complete menu system with all menus.
        /// </summary>
        public static void SetupCompleteMenuSystem(Game game, MenuSystem menuSystem)
        {
            Console.WriteLine("Setting up menu system...");

            // Create all menus
            CreateMainMenu(menuSystem);
            CreatePauseMenu(menuSystem);
            CreateSettingsMenu(menuSystem);
            CreateConfirmDialog(menuSystem);
            CreateCreditsMenu(menuSystem);

            // Setup menu system callbacks
            menuSystem.OnMenuOpened += menu =>
            {
                Console.WriteLine($"[MenuSystem] Opened: {menu.Title}");
            };

            menuSystem.OnMenuClosed += menu =>
            {
                Console.WriteLine($"[MenuSystem] Closed: {menu.Title}");
            };

            menuSystem.OnGamePaused += () =>
            {
                Console.WriteLine("[MenuSystem] Game paused");
            };

            menuSystem.OnGameResumed += () =>
            {
                Console.WriteLine("[MenuSystem] Game resumed");
            };

            Console.WriteLine("Menu system ready!");
        }

        /// <summary>
        /// Example: How to handle player input for menus.
        /// </summary>
        public static void HandleMenuInput(MenuSystem menuSystem, string input)
        {
            // This would be called from your input system
            switch (input.ToLower())
            {
                case "up":
                case "w":
                    menuSystem.NavigateUp();
                    break;

                case "down":
                case "s":
                    menuSystem.NavigateDown();
                    break;

                case "select":
                case "return":
                case "space":
                    menuSystem.PressSelectedButton();
                    break;

                case "back":
                case "escape":
                    menuSystem.Back();
                    break;

                case "pause":
                case "p":
                    if (menuSystem.IsGamePaused)
                    {
                        menuSystem.CloseCurrentMenu();
                    }
                    else
                    {
                        menuSystem.OpenMenu("pause_menu");
                    }
                    break;
            }
        }

        /// <summary>
        /// Example: Complete game setup with menus.
        /// </summary>
        public static void CompleteGameSetup()
        {
            // Create game
            var game = new Game();
            var menuSystem = new MenuSystem(game);

            // Setup menus
            SetupCompleteMenuSystem(game, menuSystem);

            // Add menu system to game
            game.AddSystem(menuSystem);

            // Open main menu at start
            menuSystem.OpenMenu("main_menu");

            Console.WriteLine("Game ready with menus!");
        }
    }

    /// <summary>
    /// Alternative simpler menu setup (minimal example).
    /// </summary>
    public class SimpleMenuSetup
    {
        /// <summary>Quick pause menu setup.</summary>
        public static MenuComponent CreateSimplePauseMenu(MenuSystem menuSystem)
        {
            return new MenuBuilder(MenuComponent.MenuType.PauseMenu, "PAUSED")
                .Button("Resume", () => menuSystem.CloseCurrentMenu())
                .Button("Main Menu", () => menuSystem.CloseCurrentMenu())
                .Button("Quit", () => { })
                .BuildFor(menuSystem, "pause_menu");
        }

        /// <summary>Quick main menu setup.</summary>
        public static MenuComponent CreateSimpleMainMenu(MenuSystem menuSystem)
        {
            return new MenuBuilder(MenuComponent.MenuType.MainMenu, "MAIN MENU")
                .Button("Play", () => menuSystem.CloseCurrentMenu())
                .Button("Settings", () => { })
                .Button("Quit", () => { })
                .BuildFor(menuSystem, "main_menu");
        }
    }
}
