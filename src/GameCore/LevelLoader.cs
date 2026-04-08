using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// Loads level data and populates scenes with entities.
    /// Handles procedural level generation and entity instantiation.
    /// </summary>
    public class LevelLoader
    {
        private Game _game;
        private IEngineAudio _audio;

        public LevelLoader(Game game, IEngineAudio audio)
        {
            _game = game;
            _audio = audio;
        }

        /// <summary>Load a level into a scene.</summary>
        public Scene? LoadLevel(Level level)
        {
            if (level == null)
            {
                Console.WriteLine("ERROR: Cannot load null level");
                return null;
            }

            Console.WriteLine($"Loading level: {level.LevelName} (ID: {level.LevelId})");

            // Create scene for this level
            var scene = new Scene(level.LevelName);

            // Only setup graphics and entities if game instance is available
            if (_game != null)
            {
                // Setup graphics
                if (level.GraphicsPreset != null)
                {
                    var graphicsEntity = _game.CreateEntity();
                    graphicsEntity.AddComponent(new Tag("GraphicsSettings"));
                    graphicsEntity.AddComponent(level.GraphicsPreset);
                }

                // Create level entities via callback
                if (level.OnLevelLoad != null)
                {
                    level.OnLevelLoad(level, _game);
                }

                Console.WriteLine($"Level loaded: {level.LevelName} with {_game.Entities.Count} entities");
            }
            else
            {
                Console.WriteLine($"Level loaded: {level.LevelName} (Game instance not available for entity creation)");
            }

            // Play background music
            if (!string.IsNullOrEmpty(level.MusicTrackId) && _audio != null)
            {
                // AudioSystem would play this
                Console.WriteLine($"Playing music: {level.MusicTrackId}");
            }

            return scene;
        }

        /// <summary>Create a basic test level with standard gameplay elements.</summary>
        public static Level CreateTestLevel(int levelNumber = 1)
        {
            var level = new Level(
                $"level_{levelNumber}",
                $"Zone {levelNumber}",
                levelNumber
            )
            {
                PlayerSpawnPoint = Vector3.Zero,
                CameraStartPosition = new Vector3(0, 5, -10),
                MusicTrackId = "main_gameplay",
                GraphicsPreset = RetroPresets.SegaGenesis,
                DifficultyMultiplier = 1.0f + (levelNumber * 0.2f)
            };

            // Add checkpoints
            level.AddCheckpoint(new LevelCheckpoint(
                $"cp_start_{levelNumber}",
                "Start",
                Vector3.Zero,
                0
            ));

            level.AddCheckpoint(new LevelCheckpoint(
                $"cp_mid_{levelNumber}",
                "Midpoint",
                new Vector3(50, 0, 0),
                1
            ));

            level.AddCheckpoint(new LevelCheckpoint(
                $"cp_end_{levelNumber}",
                "Boss Arena",
                new Vector3(100, 0, 0),
                2
            ));

            // Setup level population
            level.OnLevelLoad = (lv, game) =>
            {
                PopulateTestLevel(game, lv);
            };

            return level;
        }

        /// <summary>Populate a test level with entities.</summary>
        private static void PopulateTestLevel(Game game, Level level)
        {
            // Create player
            var player = game.CreateEntity();
            player.AddComponent(new Tag("Player"));
            player.AddComponent(new Transform3D { Position = level.PlayerSpawnPoint });
            player.AddComponent(new Rigidbody3D { Mass = 1.0f, UseGravity = true });
            player.AddComponent(new CollisionSphere { Radius = 1.0f });
            player.AddComponent(new Health(100));
            player.AddComponent(new PlayerController
            {
                Speed = 15f,
                JumpForce = 15f,
                MaxSpeed = 25f,
                CanDoubleJump = true
            });

            // Create camera target
            var cameraTarget = game.CreateEntity();
            cameraTarget.AddComponent(new Tag("CameraTarget"));
            cameraTarget.AddComponent(new Transform3D { Position = level.CameraStartPosition });

            // Create enemies (scaled by difficulty)
            int enemyCount = (int)(3 * level.DifficultyMultiplier);
            for (int i = 0; i < enemyCount; i++)
            {
                var enemy = game.CreateEntity();
                enemy.AddComponent(new Tag("Enemy"));
                enemy.AddComponent(new Transform3D { Position = new Vector3(20 + i * 10, 0, 0) });
                enemy.AddComponent(new Rigidbody3D { Mass = 1.0f, UseGravity = true });
                enemy.AddComponent(new CollisionSphere { Radius = 1.0f });
                enemy.AddComponent(new Health((int)(50 * level.DifficultyMultiplier)));
                enemy.AddComponent(new EnemyAI
                {
                    AttackRange = 3f,
                    PatrolSpeed = 5f * level.DifficultyMultiplier,
                    ChaseSpeed = 12f * level.DifficultyMultiplier
                });
            }

            level.EnemyCount = enemyCount;

            // Create collectibles
            int collectibleCount = 10;
            for (int i = 0; i < collectibleCount; i++)
            {
                var ring = game.CreateEntity();
                ring.AddComponent(new Tag("Collectible"));
                ring.AddComponent(new Transform3D { Position = new Vector3(10 + i * 3, 1, 0) });
                ring.AddComponent(new CollisionSphere { Radius = 0.3f, IsTrigger = true });
                ring.AddComponent(new Collectible(Collectible.CollectibleType.Ring, 10));
            }

            level.CollectibleCount = collectibleCount;

            // Create hazards
            var hazard = game.CreateEntity();
            hazard.AddComponent(new Tag("Hazard"));
            hazard.AddComponent(new Transform3D { Position = new Vector3(30, -5, 0) });
            hazard.AddComponent(new CollisionSphere { Radius = 2.0f, IsTrigger = true });
            hazard.AddComponent(new Hazard(Hazard.HazardType.Spike, (int)(20 * level.DifficultyMultiplier)));

            // Create platforms
            var platform = game.CreateEntity();
            platform.AddComponent(new Tag("Platform"));
            platform.AddComponent(new Transform3D { Position = new Vector3(40, 0, 0) });
            platform.AddComponent(new CollisionSphere { Radius = 2.0f });
            platform.AddComponent(new Rigidbody3D { IsKinematic = true });
            platform.AddComponent(new MovingPlatform(
                new Vector3(40, 0, 0),
                new Vector3(60, 0, 0),
                4f
            ));

            // Create checkpoint markers (visual)
            foreach (var checkpoint in level.Checkpoints)
            {
                var checkpointEntity = game.CreateEntity();
                checkpointEntity.AddComponent(new Tag("CheckpointMarker"));
                checkpointEntity.AddComponent(new Transform3D { Position = checkpoint.Position });
                checkpointEntity.AddComponent(new CollisionSphere { Radius = 2.0f, IsTrigger = true });
                checkpointEntity.AddComponent(new Interactable
                {
                    InteractionId = checkpoint.CheckpointId,
                    OnInteract = (e) => checkpoint.Activate()
                });
            }
        }
    }
}
