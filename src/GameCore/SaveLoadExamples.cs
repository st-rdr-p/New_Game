using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// Examples of using the save/load system.
    /// </summary>
    public class SaveLoadExamples
    {
        /// <summary>
        /// Example: Setup game with save/load system.
        /// </summary>
        public static void SetupGameWithSaving(Game game)
        {
            // Create save system
            var saveSystem = new SaveLoadSystem(game);
            game.AddSystem(saveSystem);

            // Setup callbacks
            saveSystem.OnSaveStart += () => Console.WriteLine("Saving game...");
            saveSystem.OnSaveComplete += (success) =>
            {
                if (success)
                    Console.WriteLine("✓ Game saved successfully!");
                else
                    Console.WriteLine("✗ Failed to save game");
            };

            saveSystem.OnLoadStart += () => Console.WriteLine("Loading game...");
            saveSystem.OnLoadComplete += (success) =>
            {
                if (success)
                    Console.WriteLine("✓ Game loaded successfully!");
                else
                    Console.WriteLine("✗ Failed to load game");
            };

            saveSystem.OnSaveMessage += (msg) => Console.WriteLine($"[Save] {msg}");

            Console.WriteLine("Save system ready!");
        }

        /// <summary>
        /// Example: Create a player entity with saving support.
        /// </summary>
        public static Entity CreateSaveablePlayer(int entityId)
        {
            var player = new Entity(entityId);

            // Add basic components
            player.AddComponent(new Transform3D { Position = new Vector3(0, 1, 0) });
            player.AddComponent(new Tag("Player"));
            player.AddComponent(new Health(100f));
            player.AddComponent(new Rigidbody3D { Mass = 1.0f });

            // Add player saving component
            var playerSave = new PlayerSaveableComponent
            {
                Level = 1,
                Experience = 0
            };
            player.AddComponent(playerSave);

            // Add inventory
            playerSave.Inventory["health_potion"] = 5;
            playerSave.Inventory["mana_potion"] = 3;
            playerSave.Inventory["gold"] = 250;

            return player;
        }

        /// <summary>
        /// Example: Create a saveable enemy entity.
        /// </summary>
        public static Entity CreateSaveableEnemy(int entityId, Vector3 position)
        {
            var enemy = new Entity(entityId);

            enemy.AddComponent(new Transform3D { Position = position });
            enemy.AddComponent(new Tag("Enemy"));
            enemy.AddComponent(new Health(50f));
            enemy.AddComponent(new EnemyAI { CurrentState = EnemyAI.State.Patrol });

            // Add saveable component
            var saveable = new SaveableComponent($"enemy_{entityId}")
            {
                SaveData = new Dictionary<string, object>
                {
                    { "patrol_range", 20f },
                    { "is_defeated", false },
                    { "last_damage_time", 0f }
                }
            };
            enemy.AddComponent(saveable);

            return enemy;
        }

        /// <summary>
        /// Example: Create a saveable NPC with dialogue.
        /// </summary>
        public static Entity CreateSaveableNPC(int entityId, Vector3 position)
        {
            var npc = new Entity(entityId);

            npc.AddComponent(new Transform3D { Position = position });
            npc.AddComponent(new Tag("NPC"));

            // Add dialogue
            var dialogue = new DialogueBuilder("Merchant")
                .Sequence()
                    .Line("Merchant", "Welcome, friend!", 2f)
                .Build();
            npc.AddComponent(dialogue);

            // Add saveable component for NPC state
            var saveable = new SaveableComponent($"npc_{entityId}")
            {
                SaveData = new Dictionary<string, object>
                {
                    { "has_quest", false },
                    { "quest_completed", false },
                    { "times_talked", 0 }
                }
            };
            saveable.OnGetState += () =>
            {
                var state = new EntityState { EntityId = entityId };
                state.CustomData = new Dictionary<string, object>(saveable.SaveData);
                return state;
            };
            npc.AddComponent(saveable);

            return npc;
        }

        /// <summary>
        /// Example: Complete game with save/load.
        /// </summary>
        public static void CompleteGameSetup()
        {
            var game = new Game();

            // Setup save system
            SetupGameWithSaving(game);

            // Create player
            var player = CreateSaveablePlayer(1);
            game.AddEntity(player);

            // Create enemies
            var enemy1 = CreateSaveableEnemy(100, new Vector3(10, 0, 10));
            var enemy2 = CreateSaveableEnemy(101, new Vector3(-10, 0, -10));
            game.AddEntity(enemy1);
            game.AddEntity(enemy2);

            // Create NPCs
            var npc1 = CreateSaveableNPC(200, new Vector3(5, 0, 0));
            game.AddEntity(npc1);

            Console.WriteLine("Game ready with save/load!");
        }

        /// <summary>
        /// Example: Save game at checkpoint.
        /// </summary>
        public static void SaveAtCheckpoint(SaveLoadSystem saveSystem, IEnumerable<Entity> entities, int slotIndex = 0)
        {
            var saveName = $"Checkpoint {DateTime.Now:HH:mm:ss}";
            bool success = saveSystem.SaveGame(slotIndex, saveName, entities);
            
            if (success)
            {
                Console.WriteLine($"Checkpoint saved to slot {slotIndex}");
            }
        }

        /// <summary>
        /// Example: Load game from slot.
        /// </summary>
        public static void LoadFromSlot(SaveLoadSystem saveSystem, IEnumerable<Entity> entities, int slotIndex = 0)
        {
            if (!saveSystem.SlotHasSave(slotIndex))
            {
                Console.WriteLine($"No save found in slot {slotIndex}");
                return;
            }

            bool success = saveSystem.LoadGame(slotIndex, entities);
            if (success)
            {
                Console.WriteLine($"Loaded from slot {slotIndex}");
            }
        }

        /// <summary>
        /// Example: Display save slots in a menu.
        /// </summary>
        public static void DisplaySaveSlots(SaveLoadSystem saveSystem, MenuSystem menuSystem)
        {
            var slots = saveSystem.GetAllSaveSlots();

            // Create save selection menu
            var menu = new MenuComponent(MenuComponent.MenuType.CustomMenu, "Select Save Slot");

            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                string buttonText;

                if (slot.IsValid)
                {
                    buttonText = $"Slot {i}: {slot.SaveName} - {slot.SaveTime:MM/dd HH:mm} ({saveSystem.FormatPlaytime(slot.PlaytimeSeconds)})";
                }
                else
                {
                    buttonText = $"Slot {i}: <Empty>";
                }

                int slotIndex = i;
                menu.AddButton(new MenuButton(buttonText, () =>
                {
                    Console.WriteLine($"Selected slot {slotIndex}");
                }));
            }

            menu.AddButton(new MenuButton("Back", () => menuSystem.CloseCurrentMenu()));

            menuSystem.OpenMenu(menu);
        }

        /// <summary>
        /// Example: Quick save/load (always uses slot 0).
        /// </summary>
        public static class QuickSave
        {
            private static SaveLoadSystem _saveSystem;

            public static void Initialize(SaveLoadSystem saveSystem)
            {
                _saveSystem = saveSystem;
            }

            public static void QuickSaveGame(IEnumerable<Entity> entities)
            {
                _saveSystem?.SaveGame(0, "QuickSave", entities);
            }

            public static void QuickLoadGame(IEnumerable<Entity> entities)
            {
                _saveSystem?.LoadGame(0, entities);
            }
        }

        /// <summary>
        /// Example: Auto-save system.
        /// </summary>
        public class AutoSaveManager
        {
            private SaveLoadSystem _saveSystem;
            private IEnumerable<Entity> _entities;
            private float _autoSaveInterval = 300f; // 5 minutes
            private float _timeSinceLastSave = 0f;

            public AutoSaveManager(SaveLoadSystem saveSystem, IEnumerable<Entity> entities)
            {
                _saveSystem = saveSystem;
                _entities = entities;
            }

            public void Update(float deltaTime)
            {
                _timeSinceLastSave += deltaTime;

                if (_timeSinceLastSave >= _autoSaveInterval)
                {
                    AutoSave();
                    _timeSinceLastSave = 0;
                }
            }

            private void AutoSave()
            {
                _saveSystem.SaveGame(9, "AutoSave", _entities); // Use last slot for autosave
                Console.WriteLine("Auto-save completed");
            }
        }
    }
}
