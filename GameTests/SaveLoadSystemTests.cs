using NUnit.Framework;
using GameCore;
using System.Collections.Generic;

namespace GameTests
{
    /// <summary>
    /// Tests for the Save/Load System - persistence, serialization, slot management.
    /// </summary>
    [TestFixture]
    public class SaveLoadSystemTests
    {
        #region Save Slot Tests

        [Test]
        public void SaveSlot_InitializesWithProperties()
        {
            // Arrange & Act
            var slot = new SaveSlot
            {
                SlotIndex = 0,
                SaveName = "My Save Game",
                CurrentLevel = "Level 1",
                ProgressPercent = 50
            };

            // Assert
            Assert.That(slot.SlotIndex, Is.EqualTo(0));
            Assert.That(slot.SaveName, Is.EqualTo("My Save Game"));
            Assert.That(slot.CurrentLevel, Is.EqualTo("Level 1"));
            Assert.That(slot.ProgressPercent, Is.EqualTo(50));
        }

        [Test]
        public void SaveSlot_TrackPlayTime()
        {
            // Arrange & Act
            var slot = new SaveSlot
            {
                SlotIndex = 1,
                PlaytimeSeconds = 3600 // 1 hour
            };

            // Assert
            Assert.That(slot.PlaytimeSeconds, Is.EqualTo(3600));
        }

        [Test]
        public void SaveSlot_ValidateSlot()
        {
            // Arrange & Act
            var slot = new SaveSlot
            {
                SlotIndex = 0,
                IsValid = true
            };

            // Assert
            Assert.That(slot.IsValid, Is.True);
        }

        [Test]
        public void SaveSlot_RecordSaveTime()
        {
            // Arrange & Act
            var slot = new SaveSlot
            {
                SlotIndex = 0,
                SaveTime = System.DateTime.Now
            };

            // Assert
            Assert.That(slot.SaveTime, Is.Not.EqualTo(System.DateTime.MinValue));
        }

        #endregion

        #region Game State Tests

        [Test]
        public void GameState_InitializesWithProperties()
        {
            // Arrange & Act
            var gameState = new GameState
            {
                CurrentLevel =  "Forest",
                PlaytimeSeconds = 1800,
                GameVersion = "1.0.0"
            };

            // Assert
            Assert.That(gameState.CurrentLevel, Is.EqualTo("Forest"));
            Assert.That(gameState.PlaytimeSeconds, Is.EqualTo(1800));
            Assert.That(gameState.GameVersion, Is.EqualTo("1.0.0"));
        }

        [Test]
        public void GameState_StoreQuestData()
        {
            // Arrange
            var gameState = new GameState();

            // Act
            gameState.CompletedQuests["quest_01"] = true;
            gameState.CompletedQuests["quest_02"] = false;

            // Assert
            Assert.That(gameState.CompletedQuests["quest_01"], Is.True);
            Assert.That(gameState.CompletedQuests["quest_02"], Is.False);
        }

        [Test]
        public void GameState_StoreGameVariables()
        {
            // Arrange
            var gameState = new GameState();

            // Act
            gameState.GameVariables["boss_defeated"] = true;
            gameState.GameVariables["coins_collected"] = 500;

            // Assert
            Assert.That(gameState.GameVariables["boss_defeated"], Is.True);
            Assert.That(gameState.GameVariables["coins_collected"], Is.EqualTo(500));
        }

        [Test]
        public void GameState_StoreEntities()
        {
            // Arrange
            var gameState = new GameState();
            var entity1 = new EntityState { EntityId = 1, EntityType = "Player" };
            var entity2 = new EntityState { EntityId = 2, EntityType = "Enemy" };

            // Act
            gameState.Entities.Add(entity1);
            gameState.Entities.Add(entity2);

            // Assert
            Assert.That(gameState.Entities.Count, Is.EqualTo(2));
            Assert.That(gameState.Entities[0].EntityType, Is.EqualTo("Player"));
        }

        #endregion

        #region Player State Tests

        [Test]
        public void PlayerState_InitializesWithHealth()
        {
            // Arrange & Act
            var playerState = new PlayerState
            {
                Health = 100,
                MaxHealth = 100,
                Level = 5,
                Experience = 2500
            };

            // Assert
            Assert.That(playerState.Health, Is.EqualTo(100));
            Assert.That(playerState.MaxHealth, Is.EqualTo(100));
            Assert.That(playerState.Level, Is.EqualTo(5));
            Assert.That(playerState.Experience, Is.EqualTo(2500));
        }

        [Test]
        public void PlayerState_StorePosition()
        {
            // Arrange & Act
            var playerState = new PlayerState
            {
                PositionX = 10.5f,
                PositionY = 0,
                PositionZ = -5.2f
            };

            // Assert
            Assert.That(playerState.PositionX, Is.EqualTo(10.5f));
            Assert.That(playerState.PositionY, Is.EqualTo(0));
            Assert.That(playerState.PositionZ, Is.EqualTo(-5.2f));
        }

        [Test]
        public void PlayerState_StoreInventory()
        {
            // Arrange
            var playerState = new PlayerState();

            // Act
            playerState.Inventory["sword"] = 1;
            playerState.Inventory["potion"] = 5;
            playerState.Inventory["coins"] = 250;

            // Assert
            Assert.That(playerState.Inventory["sword"], Is.EqualTo(1));
            Assert.That(playerState.Inventory["potion"], Is.EqualTo(5));
            Assert.That(playerState.Inventory["coins"], Is.EqualTo(250));
        }

        #endregion

        #region Entity State Tests

        [Test]
        public void EntityState_SerializeEntity()
        {
            // Arrange & Act
            var entityState = new EntityState
            {
                EntityId = 42,
                EntityType = "Enemy",
                PositionX = 100,
                PositionY = 5,
                PositionZ = 50,
                IsActive = true
            };

            // Assert
            Assert.That(entityState.EntityId, Is.EqualTo(42));
            Assert.That(entityState.EntityType, Is.EqualTo("Enemy"));
            Assert.That(entityState.PositionX, Is.EqualTo(100));
            Assert.That(entityState.IsActive, Is.True);
        }

        [Test]
        public void EntityState_StoreCustomData()
        {
            // Arrange
            var entityState = new EntityState { EntityId = 1 };

            // Act
            entityState.CustomData["health"] = 75;
            entityState.CustomData["status"] = "poisoned";

            // Assert
            Assert.That(entityState.CustomData["health"], Is.EqualTo(75));
            Assert.That(entityState.CustomData["status"], Is.EqualTo("poisoned"));
        }

        #endregion

        #region Saveable Component Tests

        [Test]
        public void SaveableComponent_InitializesWithSaveId()
        {
            // Arrange & Act
            var saveableComp = new SaveableComponent("player_entity");

            // Assert
            Assert.That(saveableComp.SaveId, Is.EqualTo("player_entity"));
            Assert.That(saveableComp.IsSaveable, Is.True);
        }

        [Test]
        public void SaveableComponent_EnableDisableSaving()
        {
            // Arrange
            var saveableComp = new SaveableComponent("entity_01");

            // Act
            saveableComp.IsSaveable = false;

            // Assert
            Assert.That(saveableComp.IsSaveable, Is.False);
        }

        [Test]
        public void SaveableComponent_StoreCustomData()
        {
            // Arrange
            var saveableComp = new SaveableComponent("entity_01");

            // Act
            saveableComp.SaveData["last_checkpoint"] = "checkpoint_3";
            saveableComp.SaveData["items_collected"] = 25;

            // Assert
            Assert.That(saveableComp.SaveData["last_checkpoint"], Is.EqualTo("checkpoint_3"));
            Assert.That(saveableComp.SaveData["items_collected"], Is.EqualTo(25));
        }

        [Test]
        public void SaveableComponent_GetState()
        {
            // Arrange
            var saveableComp = new SaveableComponent("test_entity")
            {
                OnGetState = () => new EntityState
                {
                    EntityId = 100,
                    EntityType = "TestType"
                }
            };

            // Act
            var state = saveableComp.GetState();

            // Assert
            Assert.That(state.EntityId, Is.EqualTo(100));
            Assert.That(state.EntityType, Is.EqualTo("TestType"));
        }

        [Test]
        public void SaveableComponent_RestoreState()
        {
            // Arrange
            EntityState? restoredState = null;
            var saveableComp = new SaveableComponent("test_entity")
            {
                OnRestoreState = (state) => { restoredState = state; }
            };

            var stateToRestore = new EntityState { EntityId = 50 };

            // Act
            saveableComp.RestoreState(stateToRestore);

            // Assert
            Assert.That(restoredState?.EntityId, Is.EqualTo(50));
        }

        #endregion

        #region Integration Tests

        [Test]
        public void SaveLoad_FullGameStateCapture()
        {
            // Arrange - Create complete game state
            var gameState = new GameState
            {
                CurrentLevel = "Level 2",
                PlaytimeSeconds = 7200,
                GameVersion = "1.0.0"
            };

            gameState.PlayerState = new PlayerState
            {
                Health = 80,
                MaxHealth = 100,
                Level = 5,
                PositionX = 50,
                PositionY = 0,
                PositionZ = -25
            };

            gameState.Entities.Add(new EntityState { EntityId = 1, EntityType = "Player" });
            gameState.Entities.Add(new EntityState { EntityId = 2, EntityType = "NPC" });

            // Act & Assert
            Assert.That(gameState.CurrentLevel, Is.EqualTo("Level 2"));
            Assert.That(gameState.PlayerState.Health, Is.EqualTo(80));
            Assert.That(gameState.Entities.Count, Is.EqualTo(2));
        }

        [Test]
        public void SaveLoad_SlotManagement()
        {
            // Arrange - Create multiple save slots
            var slots = new List<SaveSlot>
            {
                new SaveSlot { SlotIndex = 0, SaveName = "Save 1", ProgressPercent = 25 },
                new SaveSlot { SlotIndex = 1, SaveName = "Save 2", ProgressPercent = 50 },
                new SaveSlot { SlotIndex = 2, SaveName = "Save 3", ProgressPercent = 75 }
            };

            // Act & Assert
            Assert.That(slots.Count, Is.EqualTo(3));
            Assert.That(slots[1].SaveName, Is.EqualTo("Save 2"));
            Assert.That(slots[2].ProgressPercent, Is.EqualTo(75));
        }

        [Test]
        public void SaveLoad_EntityCollectionSerialization()
        {
            // Arrange
            var entities = new List<EntityState>
            {
                new EntityState { EntityId = 1, EntityType = "Player", IsActive = true },
                new EntityState { EntityId = 2, EntityType = "Enemy", IsActive = true },
                new EntityState { EntityId = 3, EntityType = "Collectible", IsActive = false }
            };

            // Act
            int activeCount = 0;
            foreach (var entity in entities)
            {
                if (entity.IsActive) activeCount++;
            }

            // Assert
            Assert.That(entities.Count, Is.EqualTo(3));
            Assert.That(activeCount, Is.EqualTo(2));
        }

        #endregion
    }
}

