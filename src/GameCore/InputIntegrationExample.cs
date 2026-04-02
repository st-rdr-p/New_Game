using System;

namespace GameCore
{
    /// <summary>
    /// Example usage of the input system with menus, dialogue, and save/load.
    /// Shows how to wire everything together.
    /// </summary>
    public class InputIntegrationExample
    {
        /// <summary>
        /// Complete example: Set up input-driven menus.
        /// </summary>
        public static void SetupMenuInputExample()
        {
            // Create input manager
            var inputManager = new InputManager();

            // Create menu system
            var game = new Game();
            var menuSystem = new MenuSystem(game);

            // Create menu input handler
            var menuInputHandler = new MenuInputHandler(inputManager);

            // Create a simple main menu
            var mainMenu = new MenuComponent { MenuId = "main_menu" };
            mainMenu.AddButton(new MenuButton { Text = "Start Game", Index = 0 });
            mainMenu.AddButton(new MenuButton { Text = "Settings", Index = 1 });
            mainMenu.AddButton(new MenuButton { Text = "Quit", Index = 2 });

            // Wire up menu input
            menuInputHandler.SetActiveMenu(mainMenu);

            // Handle menu selection
            menuInputHandler.OnMenuItemSelected += (button) =>
            {
                Console.WriteLine($"Selected: {button.Text}");
                if (button.Text == "Quit")
                    Environment.Exit(0);
            };

            // Game loop
            while (true)
            {
                menuInputHandler.Update();
            }
        }

        /// <summary>
        /// Complete example: Set up input-driven dialogue.
        /// </summary>
        public static void SetupDialogueInputExample()
        {
            // Create input manager
            var inputManager = new InputManager();

            // Create dialogue component
            var dialogue = new DialogueComponent();
            dialogue.Sequences = new DialogueSequence[]
            {
                new DialogueSequence
                {
                    Lines = new[]
                    {
                        new DialogueLine { Speaker = "NPC", Text = "Hello, adventurer!" }
                    },
                    Choices = new[]
                    {
                        new DialogueChoice { Text = "Who are you?", NextSequenceIndex = 1 },
                        new DialogueChoice { Text = "I'm in a hurry.", NextSequenceIndex = 2 }
                    }
                },
                new DialogueSequence
                {
                    Lines = new[]
                    {
                        new DialogueLine { Speaker = "NPC", Text = "I'm the local merchant." }
                    }
                },
                new DialogueSequence
                {
                    Lines = new[]
                    {
                        new DialogueLine { Speaker = "NPC", Text = "Goodbye then!" }
                    }
                }
            };

            // Create dialogue input handler
            var dialogueInputHandler = new DialogueInputHandler(inputManager);
            dialogue.StartDialogue(0);
            dialogueInputHandler.SetActiveDialogue(dialogue);

            // Handle dialogue events
            dialogueInputHandler.OnDialogueAdvanced += () =>
            {
                Console.WriteLine("Dialogue advanced");
            };

            dialogueInputHandler.OnChoiceSelected += (choiceIndex) =>
            {
                Console.WriteLine($"Choice selected: {choiceIndex}");
            };

            // Game loop
            while (dialogue.IsDialogueActive)
            {
                dialogueInputHandler.Update();
            }
        }

        /// <summary>
        /// Complete example: Set up input-driven save/load.
        /// </summary>
        public static void SetupSaveLoadInputExample()
        {
            // Create input manager
            var inputManager = new InputManager();

            // Create save/load system
            var game = new Game();
            var saveLoadSystem = new SaveLoadSystem(game);

            // Create save/load input handler
            var saveLoadInputHandler = new SaveLoadInputHandler(inputManager, saveLoadSystem);

            // Handle save requests (F5)
            saveLoadInputHandler.OnSaveRequested += (slotIndex) =>
            {
                Console.WriteLine($"Saving to slot {slotIndex}...");
                // saveLoadSystem.SaveGame(slotIndex, "Auto Save", ...);
            };

            // Handle load requests (F9)
            saveLoadInputHandler.OnLoadRequested += (slotIndex) =>
            {
                Console.WriteLine($"Loading from slot {slotIndex}...");
                // saveLoadSystem.LoadGame(slotIndex, ...);
            };

            // Game loop
            for (int i = 0; i < 100; i++)
            {
                saveLoadInputHandler.Update();
            }
        }

        /// <summary>
        /// Complete example: Set up player movement input.
        /// </summary>
        public static void SetupPlayerMovementExample()
        {
            // Create input manager
            var inputManager = new InputManager();

            // Create player input handler
            var playerInputHandler = new PlayerInputHandler(inputManager);

            // Handle movement
            playerInputHandler.OnMovementInput += (direction) =>
            {
                if (direction.Magnitude > 0)
                    Console.WriteLine($"Moving: {direction}");
            };

            // Handle jump
            playerInputHandler.OnJumpPressed += () =>
            {
                Console.WriteLine("Jump!");
            };

            // Handle dash
            playerInputHandler.OnDashPressed += () =>
            {
                Console.WriteLine("Dash!");
            };

            // Game loop
            for (int i = 0; i < 100; i++)
            {
                playerInputHandler.Update();
            }
        }

        /// <summary>
        /// Integrated example: Menu -> Dialogue -> Game with input handling.
        /// </summary>
        public static void FullIntegrationExample()
        {
            // Create input manager (shared across all handlers)
            var inputManager = new InputManager();

            // Create menu
            var mainMenu = new MenuComponent { MenuId = "main_menu" };
            mainMenu.AddButton(new MenuButton { Text = "New Game", Index = 0 });
            mainMenu.AddButton(new MenuButton { Text = "Load Game", Index = 1 });
            mainMenu.AddButton(new MenuButton { Text = "Quit", Index = 2 });

            // Create menu input handler
            var menuInputHandler = new MenuInputHandler(inputManager);
            menuInputHandler.SetActiveMenu(mainMenu);

            // Menu selection handler
            menuInputHandler.OnMenuItemSelected += (button) =>
            {
                Console.WriteLine($"[MENU] Selected: {button.Text}");

                if (button.Text == "New Game")
                {
                    Console.WriteLine("[GAME] Starting new game...");
                    // Transition to dialogue/gameplay
                }
                else if (button.Text == "Quit")
                {
                    Console.WriteLine("[GAME] Exiting...");
                    Environment.Exit(0);
                }
            };

            Console.WriteLine("=== INPUT SYSTEM READY ===");
            Console.WriteLine("Menu: Arrow Keys to navigate, Enter to select");
            Console.WriteLine("Commands: F5=Save, F9=Load, ESC=Menu");
            Console.WriteLine("Movement: WASD or Arrow Keys");
            Console.WriteLine("================================");

            // Game loop
            int frameCount = 0;
            while (frameCount < 1000)
            {
                menuInputHandler.Update();
                frameCount++;

                // This would normally be controlled by a game loop timer
                if (frameCount % 100 == 0)
                    Console.WriteLine($"[FRAME {frameCount}]");
            }
        }
    }
}
