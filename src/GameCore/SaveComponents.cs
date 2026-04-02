using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// Represents a single save slot with metadata.
    /// </summary>
    public class SaveSlot
    {
        /// <summary>Slot index (0, 1, 2, etc).</summary>
        public int SlotIndex { get; set; }

        /// <summary>Save game name/title.</summary>
        public string SaveName { get; set; }

        /// <summary>When the save was created.</summary>
        public DateTime SaveTime { get; set; }

        /// <summary>Total playtime in seconds.</summary>
        public float PlaytimeSeconds { get; set; }

        /// <summary>Current level/scene name.</summary>
        public string CurrentLevel { get; set; }

        /// <summary>Player position (for quick preview).</summary>
        public string PlayerLocation { get; set; }

        /// <summary>Save progress percentage (0-100).</summary>
        public int ProgressPercent { get; set; }

        /// <summary>Whether this slot has a valid save.</summary>
        public bool IsValid { get; set; } = false;
    }

    /// <summary>
    /// Game state data to be saved/loaded.
    /// </summary>
    public class GameState
    {
        /// <summary>Player entity state.</summary>
        public PlayerState PlayerState { get; set; }

        /// <summary>Current level/scene.</summary>
        public string CurrentLevel { get; set; }

        /// <summary>All entities in the world.</summary>
        public List<EntityState> Entities { get; set; } = new();

        /// <summary>Quest/objective tracking.</summary>
        public Dictionary<string, bool> CompletedQuests { get; set; } = new();

        /// <summary>Game variables/flags.</summary>
        public Dictionary<string, object> GameVariables { get; set; } = new();

        /// <summary>Total playtime in seconds.</summary>
        public float PlaytimeSeconds { get; set; }

        /// <summary>Save timestamp.</summary>
        public DateTime SaveTime { get; set; }

        /// <summary>Game version.</summary>
        public string GameVersion { get; set; } = "1.0.0";
    }

    /// <summary>
    /// Player state for serialization.
    /// </summary>
    public class PlayerState
    {
        public float Health { get; set; }
        public float MaxHealth { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public int Level { get; set; }
        public float Experience { get; set; }
        public Dictionary<string, int> Inventory { get; set; } = new();
    }

    /// <summary>
    /// Generic entity state for serialization.
    /// </summary>
    public class EntityState
    {
        public int EntityId { get; set; }
        public string EntityType { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public bool IsActive { get; set; } = true;
        public Dictionary<string, object> CustomData { get; set; } = new();
    }

    /// <summary>
    /// Component for saveable entities.
    /// </summary>
    public class SaveableComponent : Component
    {
        /// <summary>Unique save identifier for this entity.</summary>
        public string SaveId { get; set; }

        /// <summary>Whether this entity should be saved.</summary>
        public bool IsSaveable { get; set; } = true;

        /// <summary>Custom data to save for this entity.</summary>
        public Dictionary<string, object> SaveData { get; set; } = new();

        /// <summary>Callback to get entity state for saving.</summary>
        public Func<EntityState> OnGetState { get; set; }

        /// <summary>Callback to restore entity state from save.</summary>
        public Action<EntityState> OnRestoreState { get; set; }

        public SaveableComponent(string saveId)
        {
            SaveId = saveId;
        }

        /// <summary>Get this entity's current state.</summary>
        public EntityState GetState()
        {
            if (OnGetState != null)
                return OnGetState();

            // Default state capture
            var state = new EntityState
            {
                EntityId = Owner.Id,
                EntityType = Owner.TryGetComponent<Tag>(out var tag) ? tag.Value : "Unknown",
                IsActive = true,
                CustomData = new Dictionary<string, object>(SaveData)
            };

            if (Owner.TryGetComponent<Transform3D>(out var transform))
            {
                state.PositionX = transform.Position.X;
                state.PositionY = transform.Position.Y;
                state.PositionZ = transform.Position.Z;
            }

            return state;
        }

        /// <summary>Restore entity state from save.</summary>
        public void RestoreState(EntityState state)
        {
            OnRestoreState?.Invoke(state);

            // Default restoration
            if (Owner.TryGetComponent<Transform3D>(out var transform))
            {
                transform.Position = new Vector3(state.PositionX, state.PositionY, state.PositionZ);
            }

            SaveData = new Dictionary<string, object>(state.CustomData);
        }
    }

    /// <summary>
    /// Component for player state management.
    /// </summary>
    public class PlayerSaveableComponent : Component
    {
        /// <summary>Current player level.</summary>
        public int Level { get; set; } = 1;

        /// <summary>Current experience points.</summary>
        public float Experience { get; set; } = 0;

        /// <summary>Inventory items (item ID -> quantity).</summary>
        public Dictionary<string, int> Inventory { get; set; } = new();

        /// <summary>Get current player state for saving.</summary>
        public PlayerState GetState()
        {
            var health = Owner.TryGetComponent<Health>(out var h) ? h.CurrentHealth : 100;
            var maxHealth = Owner.TryGetComponent<Health>(out var h2) ? h2.MaxHealth : 100;

            var position = Vector3.Zero;
            if (Owner.TryGetComponent<Transform3D>(out var transform))
                position = transform.Position;

            return new PlayerState
            {
                Health = health,
                MaxHealth = maxHealth,
                PositionX = position.X,
                PositionY = position.Y,
                PositionZ = position.Z,
                Level = Level,
                Experience = Experience,
                Inventory = new Dictionary<string, int>(Inventory)
            };
        }

        /// <summary>Restore player state from save.</summary>
        public void RestoreState(PlayerState state)
        {
            Level = state.Level;
            Experience = state.Experience;
            Inventory = new Dictionary<string, int>(state.Inventory);

            if (Owner.TryGetComponent<Health>(out var health))
            {
                health.CurrentHealth = state.Health;
            }

            if (Owner.TryGetComponent<Transform3D>(out var transform))
            {
                transform.Position = new Vector3(state.PositionX, state.PositionY, state.PositionZ);
            }
        }
    }
}
