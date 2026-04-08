using NUnit.Framework;
using GameCore;
using System.Collections.Generic;

namespace GameTests
{
    /// <summary>
    /// Tests for the Scene/Level Management System - level loading, transitions, checkpoints, progression.
    /// </summary>
    [TestFixture]
    public class SceneManagerLevelTests
    {
        private Game _game;
        private SceneManager _sceneManager;
        private LevelLoader _levelLoader;

        [SetUp]
        public void Setup()
        {
            _game = new Game(null, null);
            _sceneManager = new SceneManager(null, null);
            _levelLoader = new LevelLoader(_game, null);
            _game.AddSystem(_sceneManager);
        }

        #region Level Tests

        [Test]
        public void Level_InitializesWithProperties()
        {
            // Arrange & Act
            var level = new Level("1", "Forest Path", 1);
            level.DifficultyMultiplier = 1.0f;

            // Assert
            Assert.That(level.LevelId, Is.EqualTo("1"));
            Assert.That(level.LevelName, Is.EqualTo("Forest Path"));
            Assert.That(level.LevelNumber, Is.EqualTo(1));
            Assert.That(level.DifficultyMultiplier, Is.EqualTo(1.0f));
        }

        [Test]
        public void Level_SetSpawnPoint()
        {
            // Arrange
            var level = new Level("1", "Test Level", 1);

            // Act
            level.PlayerSpawnPoint = new Vector3(0, 1, 0);

            // Assert
            Assert.That(level.PlayerSpawnPoint, Is.EqualTo(new Vector3(0, 1, 0)));
        }

        [Test]
        public void Level_SetCameraStartPosition()
        {
            // Arrange
            var level = new Level("1", "Test Level", 1);

            // Act
            level.CameraStartPosition = new Vector3(10, 5, -10);

            // Assert
            Assert.That(level.CameraStartPosition, Is.EqualTo(new Vector3(10, 5, -10)));
        }

        [Test]
        public void Level_DifficultyScaling()
        {
            // Arrange
            var level = new Level("2", "Hard Level", 2);
            level.DifficultyMultiplier = 1.5f;

            // Act
            float scaledHealth = 100 * level.DifficultyMultiplier;
            float scaledDamage = 25 * level.DifficultyMultiplier;

            // Assert
            Assert.That(scaledHealth, Is.EqualTo(150));
            Assert.That(scaledDamage, Is.EqualTo(37.5f));
        }

        #endregion

        #region Level Checkpoint Tests

        [Test]
        public void LevelCheckpoint_InitializesWithProperties()
        {
            // Arrange & Act
            var checkpoint = new LevelCheckpoint("1", "Forest Entrance", Vector3.Zero, 1);

            // Assert
            Assert.That(checkpoint.CheckpointId, Is.EqualTo("1"));
            Assert.That(checkpoint.Name, Is.EqualTo("Forest Entrance"));
            Assert.That(checkpoint.Position, Is.EqualTo(Vector3.Zero));
            Assert.That(checkpoint.Order, Is.EqualTo(1));
        }

        [Test]
        public void LevelCheckpoint_ActivateCheckpoint()
        {
            // Arrange
            var checkpoint = new LevelCheckpoint("1", "Test", Vector3.Zero);

            // Act
            checkpoint.IsActivated = true;

            // Assert
            Assert.That(checkpoint.IsActivated, Is.True);
        }

        [Test]
        public void LevelCheckpoint_RecordActivationTime()
        {
            // Arrange
            var checkpoint = new LevelCheckpoint("1", "Checkpoint", Vector3.Zero);
            var beforeActivation = System.DateTime.Now;

            // Act
            checkpoint.IsActivated = true;
            checkpoint.ActivationTime = System.DateTime.Now;
            var afterActivation = System.DateTime.Now;

            // Assert
            Assert.That(checkpoint.IsActivated, Is.True);
            Assert.That(checkpoint.ActivationTime, Is.GreaterThanOrEqualTo(beforeActivation));
            Assert.That(checkpoint.ActivationTime, Is.LessThanOrEqualTo(afterActivation));
        }

        [Test]
        public void LevelCheckpoint_CallbackOnReached()
        {
            // Arrange
            bool checkpointReached = false;
            var checkpoint = new LevelCheckpoint("1", "Test Checkpoint", Vector3.Zero);
            checkpoint.OnCheckpointReached = (cp) => { checkpointReached = true; };

            // Act
            checkpoint.OnCheckpointReached?.Invoke(checkpoint);

            // Assert
            Assert.That(checkpointReached, Is.True);
        }

        #endregion

        #region Checkpoint Management Tests

        [Test]
        public void Level_AddCheckpoint()
        {
            // Arrange
            var level = new Level("1", "Test Level", 1);
            var checkpoint = new LevelCheckpoint("1", "CP1", Vector3.Zero);

            // Act
            level.AddCheckpoint(checkpoint);

            // Assert
            Assert.That(level.GetCheckpoint("1"), Is.EqualTo(checkpoint));
        }

        [Test]
        public void Level_MultipleCheckpoints()
        {
            // Arrange
            var level = new Level("1", "Test Level", 1);
            var cp1 = new LevelCheckpoint("1", "Start", Vector3.Zero);
            var cp2 = new LevelCheckpoint("2", "Middle", new Vector3(50, 0, 0));
            var cp3 = new LevelCheckpoint("3", "End", new Vector3(100, 0, 0));

            // Act
            level.AddCheckpoint(cp1);
            level.AddCheckpoint(cp2);
            level.AddCheckpoint(cp3);

            // Assert
            Assert.That(level.GetCheckpoint("1")?.Name, Is.EqualTo("Start"));
            Assert.That(level.GetCheckpoint("2")?.Name, Is.EqualTo("Middle"));
            Assert.That(level.GetCheckpoint("3")?.Name, Is.EqualTo("End"));
        }

        [Test]
        public void Level_GetNearestCheckpoint()
        {
            // Arrange
            var level = new Level("1", "Test Level", 1);
            level.AddCheckpoint(new LevelCheckpoint("1", "Checkpoint", Vector3.Zero));
            level.AddCheckpoint(new LevelCheckpoint("2", "Middle", new Vector3(50, 0, 0)));
            level.AddCheckpoint(new LevelCheckpoint("3", "End", new Vector3(100, 0, 0)));

            // Act
            var playerPos = new Vector3(55, 0, 0);
            var nearest = level.GetNearestCheckpoint(playerPos);

            // Assert
            Assert.That(nearest?.CheckpointId, Is.EqualTo("2"));
        }

        #endregion

        #region Scene Tests

        [Test]
        public void Scene_InitializesWithProperties()
        {
            // Arrange & Act
            var scene = new Scene("TestScene");

            // Assert
            Assert.That(scene.Name, Is.EqualTo("TestScene"));
            // SceneId assertion removed - Scene no longer has SceneId
        }

        [Test]
        public void Scene_AddEntity()
        {
            // Arrange
            var scene = new Scene("TestScene");
            var entity = _game.CreateEntity();

            // Act
            scene.Add(entity);

            // Assert
            Assert.That(scene.Entities.Count, Is.GreaterThan(0));
        }

        [Test]
        public void Scene_RemoveEntity()
        {
            // Arrange
            var scene = new Scene("TestScene");
            var entity = _game.CreateEntity();
            scene.Add(entity);
            int countAfterAdd = scene.Entities.Count;

            // Act
            scene.Remove(entity);

            // Assert
            Assert.That(scene.Entities.Count, Is.LessThan(countAfterAdd));
        }

        #endregion

        #region Scene Manager Tests

        [Test]
        public void SceneManager_LoadLevel()
        {
            // Arrange
            var system = new SceneManager(null, null);
            var level = new Level("1", "Level 1", 1);

            // Act
            system.RegisterLevel(level);

            // Assert
            Assert.That(system.GetLevel("1"), Is.EqualTo(level));
        }

        [Test]
        public void SceneManager_GetCurrentLevel()
        {
            // Arrange
            var system = new SceneManager(null, null);
            var level = new Level("1", "Test Level", 1);
            system.RegisterLevel(level);

            // Act
            system.LoadLevel(level);
            var currentLevel = system.GetCurrentLevel();

            // Assert
            Assert.That(currentLevel, Is.EqualTo(level));
        }

        [Test]
        public void SceneManager_LevelProgression()
        {
            // Arrange
            var system = new SceneManager(null, null);
            var level1 = new Level("level_1", "Level 1", 1);
            var level2 = new Level("level_2", "Level 2", 2);

            system.RegisterLevel(level1);
            system.RegisterLevel(level2);

            // Act
            system.LoadLevel(level1);
            var nextLevel = system.GetNextLevel();

            // Assert
            Assert.That(nextLevel.LevelNumber, Is.EqualTo(2));
        }

        [Test]
        public void SceneManager_ReloadCurrentLevel()
        {
            // Arrange
            var system = new SceneManager(null, null);
            var level = new Level("1", "Test Level", 1);
            system.RegisterLevel(level);
            system.LoadLevel(level);

            // Act
            system.ReloadCurrentLevel();

            // Assert
            Assert.That(system.GetCurrentLevel(), Is.EqualTo(level));
        }

        [Test]
        public void SceneManager_ProgressionTracking()
        {
            // Arrange
            var system = new SceneManager(null, null);

            // Act
            system.AddProgressionPoints(500);
            system.AddProgressionPoints(300);

            // Assert
            Assert.That(system.GetProgressionPoints(), Is.EqualTo(800));
        }

        [Test]
        public void SceneManager_LevelHistory()
        {
            // Arrange
            var system = new SceneManager(null, null);
            var level1 = new Level("1", "Level 1", 1);
            var level2 = new Level("2", "Level 2", 2);

            system.RegisterLevel(level1);
            system.RegisterLevel(level2);

            // Act
            system.LoadLevel(level1);
            system.LoadLevel(level2);

            // Assert
            var history = system.GetLevelHistory();
            Assert.That(history.Count, Is.GreaterThan(0));
        }

        [Test]
        public void SceneManager_TransitionState()
        {
            // Arrange
            var system = new SceneManager(null, null);

            // Act & Assert
            Assert.That(system.IsTransitioning, Is.False);
        }

        [Test]
        public void SceneManager_TransitionDuration()
        {
            // Arrange
            var system = new SceneManager(null, null);

            // Act
            system.TransitionDuration = 1.5f;

            // Assert
            Assert.That(system.TransitionDuration, Is.EqualTo(1.5f));
        }

        #endregion

        #region Checkpoint Component Tests

        [Test]
        public void CheckpointComponent_InitializesWithDefaults()
        {
            // Arrange & Act
            var checkpointComp = new CheckpointComponent();

            // Assert
            Assert.That(checkpointComp.IsActive, Is.False);
            Assert.That(checkpointComp.ActivationRadius, Is.GreaterThan(0));
        }

        [Test]
        public void CheckpointComponent_SetActivationRadius()
        {
            // Arrange
            var checkpointComp = new CheckpointComponent();

            // Act
            checkpointComp.ActivationRadius = 15.0f;

            // Assert
            Assert.That(checkpointComp.ActivationRadius, Is.EqualTo(15.0f));
        }

        [Test]
        public void CheckpointComponent_ActivateCheckpoint()
        {
            // Arrange
            var checkpointComp = new CheckpointComponent();

            // Act
            checkpointComp.IsActive = true;

            // Assert
            Assert.That(checkpointComp.IsActive, Is.True);
        }

        #endregion

        #region Level Loader Tests

        [Test]
        public void LevelLoader_CreateTestLevel()
        {
            // Arrange & Act
            var testLevel = LevelLoader.CreateTestLevel(1);

            // Assert
            Assert.That(testLevel.LevelNumber, Is.EqualTo(1));
            Assert.That(testLevel.DifficultyMultiplier, Is.GreaterThan(0));
        }

        [Test]
        public void LevelLoader_ScalingWithDifficulty()
        {
            // Arrange
            var easyLevel = LevelLoader.CreateTestLevel(1);
            var hardLevel = LevelLoader.CreateTestLevel(3);

            // Act & Assert
            Assert.That(easyLevel.DifficultyMultiplier, Is.LessThan(hardLevel.DifficultyMultiplier));
        }

        #endregion

        #region Scene Manager System Tests

        [Test]
        public void SceneManager_UpdateProcessesTransitions()
        {
            // Arrange
            var system = new SceneManager(null, null);
            var entities = new List<Entity>();

            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => system.Update(0.016f, entities));
        }

        [Test]
        public void SceneManager_CallbackOnSceneLoaded()
        {
            // Arrange
            var system = new SceneManager(null, null);
            bool sceneLoaded = false;

            system.OnSceneLoaded += (scene, level) => { sceneLoaded = true; };

            // Act
            system.OnSceneLoaded?.Invoke(null, null);

            // Assert
            Assert.That(sceneLoaded, Is.True);
        }

        [Test]
        public void SceneManager_CallbackOnSceneUnloaded()
        {
            // Arrange
            var system = new SceneManager(null, null);
            bool sceneUnloaded = false;

            system.OnSceneUnloaded += (scene, level) => { sceneUnloaded = true; };

            // Act
            system.OnSceneUnloaded?.Invoke(null, null);

            // Assert
            Assert.That(sceneUnloaded, Is.True);
        }

        #endregion

        #region Integration Tests

        [Test]
        public void SceneLevel_CompleteGameProgression()
        {
            // Arrange
            var system = new SceneManager(null, null);
            var levelLoader = new LevelLoader(_game, null);

            var level1 = LevelLoader.CreateTestLevel(1);
            var level2 = LevelLoader.CreateTestLevel(2);

            system.RegisterLevel(level1);
            system.RegisterLevel(level2);

            // Act - Progress through levels
            system.LoadLevel(level1);
            Assert.That(system.GetCurrentLevel(), Is.EqualTo(level1));

            system.LoadLevel(level2);
            Assert.That(system.GetCurrentLevel(), Is.EqualTo(level2));

            // Assert
            Assert.That(system.GetLevelHistory().Count, Is.GreaterThan(0));
        }

        [Test]
        public void SceneLevel_CheckpointRespawnFlow()
        {
            // Arrange
            var level = new Level("1", "Test Level", 1);
            var cp1 = new LevelCheckpoint("1", "Start", Vector3.Zero);
            var cp2 = new LevelCheckpoint("2", "Checkpoint 2", new Vector3(50, 0, 0));

            level.AddCheckpoint(cp1);
            level.AddCheckpoint(cp2);

            // Act - Simulate player movement and checkpoint activation
            cp1.IsActivated = true;
            var playerPos = new Vector3(60, 0, 0);
            var nearestCheckpoint = level.GetNearestCheckpoint(playerPos);

            cp2.IsActivated = true;

            // Assert
            Assert.That(cp1.IsActivated, Is.True);
            Assert.That(cp2.IsActivated, Is.True);
            Assert.That(nearestCheckpoint.CheckpointId, Is.EqualTo("2"));
        }

        [Test]
        public void SceneLevel_LevelDifficultyProgression()
        {
            // Arrange
            var level1 = LevelLoader.CreateTestLevel(1);
            var level2 = LevelLoader.CreateTestLevel(2);
            var level3 = LevelLoader.CreateTestLevel(3);

            // Act & Assert - Each level gets harder
            Assert.That(level1.DifficultyMultiplier, Is.GreaterThan(0));
            Assert.That(level2.DifficultyMultiplier, Is.GreaterThan(level1.DifficultyMultiplier));
            Assert.That(level3.DifficultyMultiplier, Is.GreaterThan(level2.DifficultyMultiplier));
        }

        [Test]
        public void SceneLevel_SceneTransitionSequence()
        {
            // Arrange
            var system = new SceneManager(null, null);
            var level = new Level("1", "Test Level", 1);

            // Act
            system.RegisterLevel(level);
            system.LoadLevel(level);

            // Set transition duration
            system.TransitionDuration = 1.0f;

            // Assert
            Assert.That(system.GetCurrentLevel(), Is.EqualTo(level));
            Assert.That(system.TransitionDuration, Is.EqualTo(1.0f));
        }

        #endregion
    }
}
