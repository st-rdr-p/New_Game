using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// Complete game setup for a 3D Sonic-like game with all systems.
    /// Shows how to initialize entities, systems, components, and all managers (Audio, Menus, Dialogue, Lore).
    /// </summary>
    public class GameSetup
    {
        /// <summary>
        /// Initialize a complete 3D game with player, enemy, dialogue, menus, lore, and all interactive elements.
        /// Requires bridges for Input, Renderer, and Audio.
        /// </summary>
        public static void SetupSonicLikeGame(Game game, IEngineInput input, IEngineRenderer renderer, IEngineAudio audio)
        {
            // Create systems that need references for initialization
            var dialogueSystem = new DialogueSystem(audio);
            var menuSystem = new MenuSystem(game);
            var sceneManager = new SceneManager(game, audio);
            var checkpointSystem = new CheckpointSystem();
            
            // Add all systems in execution order
            game.AddSystem(new Physics3DSystem());
            game.AddSystem(new AudioSystem(audio));
            game.AddSystem(new UISystem(renderer));
            game.AddSystem(new CollisionSystem());
            game.AddSystem(new PlayerInputSystem(input));
            game.AddSystem(new ProjectileSystem());
            game.AddSystem(new CollectibleSystem(audio));
            game.AddSystem(new HazardSystem(audio));
            game.AddSystem(new PlatformSystem());
            game.AddSystem(new PowerUpSystem());
            game.AddSystem(new EnemyAISystem());
            game.AddSystem(new InteractionSystem(input));
            game.AddSystem(dialogueSystem);
            game.AddSystem(menuSystem);
            game.AddSystem(sceneManager);
            game.AddSystem(checkpointSystem);
            game.AddSystem(new CameraSystem(input));
            game.AddSystem(new SceneTransitionSystem(renderer));
            game.AddSystem(new ScreenFlashSystem(renderer));
            game.AddSystem(new RetroGraphicsSystem(renderer));

            // ==================== INITIALIZE MANAGERS ====================
            
            // Initialize Lore Database with all story entries
            var loreDatabase = LoreEntryDefinitions.Initialize();

            // Initialize Audio Manager and preload music
            var audioManager = game.CreateEntity();
            audioManager.AddComponent(new Tag("AudioManager"));
            audioManager.AddComponent(new AudioManager { IsInitialized = true });
            
            // Preload audio assets
            if (audio != null)
            {
                PreloadAudioAssets(audio);
            }

            // Create UI manager entity
            var uiManager = game.CreateEntity();
            uiManager.AddComponent(new Tag("UIManager"));
            uiManager.AddComponent(new UIManager { ShowAllUI = true });

            // Initialize Menu System with main menus
            CreateMainMenu(menuSystem);
            CreatePauseMenu(menuSystem);
            CreateSettingsMenu(menuSystem);

            // Create Lore Manager for accessing collected lore in-game
            CreateLoreManager(game, loreDatabase);

            // Initialize Dialogue System with NPCs and encounters
            SetupDialogueEncounters(game, dialogueSystem);

            // ==================== INITIALIZE SCENE/LEVEL SYSTEM ====================

            // Setup level progression and scene management
            SetupLevelSystem(game, sceneManager, checkpointSystem);

            // ==================== CREATE GAME WORLD ====================

            // Note: Game world is now created through levels
            // Levels are loaded by SceneManager and populated by LevelLoader
            // See SetupLevelSystem() for level initialization
        }

        /// <summary>
        /// Setup the level/scene management system with test levels.
        /// </summary>
        private static void SetupLevelSystem(Game game, SceneManager sceneManager, CheckpointSystem checkpointSystem)
        {
            Console.WriteLine("Initializing scene/level system...");

            // Create test levels (levels 1-3)
            var level1 = LevelLoader.CreateTestLevel(1);
            var level2 = LevelLoader.CreateTestLevel(2);
            var level3 = LevelLoader.CreateTestLevel(3);

            // Register levels in scene manager
            sceneManager.RegisterLevel(level1);
            sceneManager.RegisterLevel(level2);
            sceneManager.RegisterLevel(level3);

            // Setup scene transition callbacks
            sceneManager.OnSceneLoaded += (scene, level) =>
            {
                Console.WriteLine($"Scene loaded: {level.LevelName}");
                Console.WriteLine($"Enemies: {level.EnemyCount} | Collectibles: {level.CollectibleCount}");
            };

            sceneManager.OnTransitionStart += (oldLevel, newLevel) =>
            {
                Console.WriteLine($"Transitioning from {oldLevel?.LevelName ?? "Start"} to {newLevel.LevelName}...");
            };

            sceneManager.OnTransitionComplete += (level) =>
            {
                Console.WriteLine($"Now playing: {level.LevelName}");
            };

            // Setup checkpoint system callbacks
            checkpointSystem.OnCheckpointActivated += (checkpoint) =>
            {
                Console.WriteLine($"Checkpoint activated: {checkpoint.Name}");
            };

            // Load first level
            Console.WriteLine("Loading first level...");
            sceneManager.LoadLevel(level1);
        }

        /// <summary>
        /// Setup traditional game world (for demo/testing without level system).
        /// </summary>
        private static void SetupTraditionalGameWorld(Game game)
        {
            // This method preserves the old game world setup if needed for testing
            Console.WriteLine("Setting up traditional game world...");
        }

        /// <summary>
        /// Preload all audio assets (music and sound effects) into memory.
        /// </summary>
        private static void PreloadAudioAssets(IEngineAudio audio)
        {
            if (audio == null) return;

            // Preload background music
            // Note: Adjust these paths to match your actual audio file locations
            var musicTracks = new[]
            {
                ("menu_music", "audio/music/menu_theme.ogg"),
                ("main_gameplay", "audio/music/main_theme.ogg"),
                ("boss_music", "audio/music/boss_battle.ogg"),
                ("ambient_zone1", "audio/music/zone1_ambient.ogg"),
            };

            // Preload sound effects
            var sfxTracks = new[]
            {
                ("jump", "audio/sfx/jump.wav"),
                ("dash", "audio/sfx/dash.wav"),
                ("damage", "audio/sfx/damage.wav"),
                ("collect_ring", "audio/sfx/ring.wav"),
                ("hit_enemy", "audio/sfx/hit.wav"),
                ("lava_damage", "audio/sfx/lava.wav"),
            };

            // These would be loaded via the audio bridge
            // Implementation depends on your IEngineAudio interface
            Console.WriteLine("Audio assets preloaded (customize paths for your game)");
        }

        /// <summary>
        /// Create and register the main menu.
        /// </summary>
        private static void CreateMainMenu(MenuSystem menuSystem)
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

            menu.AddButton(new MenuButton("Continue", () =>
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

            menu.AddButton(new MenuButton("Lore", () =>
            {
                Console.WriteLine("Opening lore database...");
                menuSystem.OpenMenu("lore_menu");
            }));

            menu.AddButton(new MenuButton("Credits", () =>
            {
                Console.WriteLine("Showing credits...");
                menuSystem.OpenMenu("credits_menu");
            }));

            menu.AddButton(new MenuButton("Quit", () =>
            {
                Console.WriteLine("Quitting game...");
                Environment.Exit(0);
            }));

            menuSystem.RegisterMenu("main_menu", menu);
        }

        /// <summary>
        /// Create and register the pause menu.
        /// </summary>
        private static void CreatePauseMenu(MenuSystem menuSystem)
        {
            var menu = new MenuComponent(MenuComponent.MenuType.PauseMenu, "Paused")
            {
                PausesGame = true,
                OnMenuOpen = () => Console.WriteLine("Game paused"),
                OnMenuClose = () => Console.WriteLine("Game resumed")
            };

            menu.AddButton(new MenuButton("Resume", () =>
            {
                Console.WriteLine("Resuming...");
                menuSystem.CloseCurrentMenu();
            }));

            menu.AddButton(new MenuButton("Settings", () =>
            {
                menuSystem.OpenMenu("settings_menu");
            }));

            menu.AddButton(new MenuButton("Lore Database", () =>
            {
                menuSystem.OpenMenu("lore_menu");
            }));

            menu.AddButton(new MenuButton("Save Game", () =>
            {
                Console.WriteLine("Game saved");
            }));

            menu.AddButton(new MenuButton("Main Menu", () =>
            {
                menuSystem.ResumeGame();
                menuSystem.CloseCurrentMenu();
                menuSystem.OpenMenu("main_menu");
            }));

            menuSystem.RegisterMenu("pause_menu", menu);
        }

        /// <summary>
        /// Create and register the settings menu.
        /// </summary>
        private static void CreateSettingsMenu(MenuSystem menuSystem)
        {
            var menu = new MenuComponent(MenuComponent.MenuType.Settings, "Settings")
            {
                PausesGame = true,
                OnMenuOpen = () => Console.WriteLine("Settings menu opened"),
                OnMenuClose = () => Console.WriteLine("Settings closed")
            };

            menu.AddButton(new MenuButton("Audio (100%)", () =>
            {
                Console.WriteLine("Opening audio settings...");
            }));

            menu.AddButton(new MenuButton("Graphics", () =>
            {
                Console.WriteLine("Opening graphics settings...");
            }));

            menu.AddButton(new MenuButton("Controls", () =>
            {
                Console.WriteLine("Opening controls...");
            }));

            menu.AddButton(new MenuButton("Difficulty", () =>
            {
                Console.WriteLine("Opening difficulty...");
            }));

            menu.AddButton(new MenuButton("Back", () =>
            {
                menuSystem.CloseCurrentMenu();
            }));

            menuSystem.RegisterMenu("settings_menu", menu);
        }

        /// <summary>
        /// Create the Lore Manager entity that tracks collected lore entries.
        /// </summary>
        private static void CreateLoreManager(Game game, LoreDatabase database)
        {
            var loreManager = game.CreateEntity();
            loreManager.AddComponent(new Tag("LoreManager"));
            loreManager.AddComponent(new LoreItemComponent { Database = database });
            
            // Example: Set up callback when lore is collected
            database.OnEntryCollected += (entry) =>
            {
                Console.WriteLine($"Lore collected: {entry.Title} ({entry.Id})");
                Console.WriteLine($"Collected: {database.GetCollectedCount()}/{database.GetTotalCount()}");
            };
        }

        /// <summary>
        /// Setup dialogue encounters with NPCs and dialogue trees.
        /// </summary>
        private static void SetupDialogueEncounters(Game game, DialogueSystem dialogueSystem)
        {
            // Create first NPC with dialogue encounter
            var npc1 = game.CreateEntity();
            npc1.AddComponent(new Tag("NPC"));
            npc1.AddComponent(new Transform3D { Position = new Vector3(5, 0, 5) });
            npc1.AddComponent(new CollisionSphere { Radius = 1.0f, IsTrigger = true });
            
            // Create dialogue sequence for NPC1
            var npc1Dialogue = new DialogueComponent
            {
                CharacterName = "Curious Traveler",
                IsDialogueActive = false
            };
            
            var dialogue1 = new DialogueLine
            {
                Speaker = "Curious Traveler",
                Text = "Greetings, traveler. I sense something strange in this land.",
                Duration = 3.0f,
                VoiceClip = "voice_scripts/npc1_greeting.ogg"
            };
            
            var dialogue2 = new DialogueLine
            {
                Speaker = "Curious Traveler",
                Text = "Have you noticed the anomalies? They grow stronger each day.",
                Duration = 3.0f,
                VoiceClip = "voice_scripts/npc1_warning.ogg"
            };
            
            npc1Dialogue.Add(dialogue1);
            npc1Dialogue.Add(dialogue2);
            
            npc1.AddComponent(npc1Dialogue);
            
            // Create encounter trigger for NPC1
            npc1.AddComponent(new EncounterTrigger
            {
                EncounterId = "npc1_first_meeting",
                TriggerRadius = 3.0f,
                DialogueToTrigger = npc1Dialogue,
                OnEncounterStart = () => Console.WriteLine("Encountered: Curious Traveler"),
                OnEncounterEnd = () => Console.WriteLine("Dialogue ended with Curious Traveler")
            });

            // Create second NPC for later game
            var npc2 = game.CreateEntity();
            npc2.AddComponent(new Tag("NPC"));
            npc2.AddComponent(new Transform3D { Position = new Vector3(-10, 0, -5) });
            npc2.AddComponent(new CollisionSphere { Radius = 1.0f, IsTrigger = true });
            
            var npc2Dialogue = new DialogueComponent
            {
                CharacterName = "Mysterious Guide",
                IsDialogueActive = false
            };
            
            var dialogue3 = new DialogueLine
            {
                Speaker = "Mysterious Guide",
                Text = "You're making progress. Keep moving forward.",
                Duration = 2.5f
            };
            
            var dialogue4 = new DialogueLine
            {
                Speaker = "Mysterious Guide",
                Text = "The answers you seek lie deeper. But beware.",
                Duration = 3.0f
            };
            
            npc2Dialogue.Add(dialogue3);
            npc2Dialogue.Add(dialogue4);
            npc2.AddComponent(npc2Dialogue);
            
            npc2.AddComponent(new EncounterTrigger
            {
                EncounterId = "npc2_mid_game",
                TriggerRadius = 3.0f,
                DialogueToTrigger = npc2Dialogue,
                OnEncounterStart = () => Console.WriteLine("Encountered: Mysterious Guide"),
                OnEncounterEnd = () => Console.WriteLine("Dialogue ended with Mysterious Guide")
            });
        }

        /// <summary>
        /// Spawns a rock projectile when player dashes.
        /// </summary>
        private static void SpawnDashRock(Game game, Entity player, Vector3 direction)
        {
            if (!player.TryGetComponent<Transform3D>(out var playerTransform))
                return;

            // Create rock projectile
            var rock = game.CreateEntity();
            rock.AddComponent(new Tag("Projectile"));
            
            // Spawn slightly in front of player
            var spawnPos = playerTransform.Position + direction * 2f;
            rock.AddComponent(new Transform3D { Position = spawnPos });
            
            rock.AddComponent(new Rigidbody3D 
            { 
                IsKinematic = true,
                Velocity = direction * 25f  // Rock inherits dash velocity
            });
            
            rock.AddComponent(new CollisionSphere { Radius = 0.5f, IsTrigger = true });
            rock.AddComponent(new MeshRenderer { MeshId = "rock_mesh", MaterialId = "rock_material" });
            
            // Rock projectile: 7 second lifetime, 15 damage
            rock.AddComponent(new Projectile("rock", speed: 25f, lifetime: 7f) 
            { 
                Launcher = player 
            });
            
            rock.AddComponent(new DamageSource 
            { 
                Damage = 15, 
                SourceTag = "player", 
                KnockbackForce = 20f 
            });
        }
    }
}
