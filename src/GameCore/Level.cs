using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// Represents a game level with spawn points, checkpoints, and metadata.
    /// </summary>
    public class Level
    {
        /// <summary>Unique identifier for this level.</summary>
        public string LevelId { get; set; }

        /// <summary>Display name for the level.</summary>
        public string LevelName { get; set; }

        /// <summary>Optional description of the level.</summary>
        public string? Description { get; set; }

        /// <summary>Level number (for progression).</summary>
        public int LevelNumber { get; set; }

        /// <summary>Difficulty multiplier for enemies/hazards.</summary>
        public float DifficultyMultiplier { get; set; } = 1.0f;

        /// <summary>Player spawn position in the level.</summary>
        public Vector3 PlayerSpawnPoint { get; set; }

        /// <summary>Camera starting position.</summary>
        public Vector3 CameraStartPosition { get; set; }

        /// <summary>Background music track ID.</summary>
        public string? MusicTrackId { get; set; }

        /// <summary>Ambient sound layer (optional).</summary>
        public string? AmbientSoundId { get; set; }

        /// <summary>Retro graphics preset for this level.</summary>
        public RetroGraphicsEffect? GraphicsPreset { get; set; }

        /// <summary>List of all checkpoints in the level.</summary>
        public List<LevelCheckpoint> Checkpoints { get; set; } = new();

        /// <summary>Callback to populate level with entities.</summary>
        public Action<Level, Game>? OnLevelLoad { get; set; }

        /// <summary>Callback when level is unloaded.</summary>
        public Action<Level>? OnLevelUnload { get; set; }

        /// <summary>Time limit in seconds (0 = no limit).</summary>
        public float TimeLimit { get; set; } = 0;

        /// <summary>Enemies spawn count (for progression tracking).</summary>
        public int EnemyCount { get; set; } = 0;

        /// <summary>Collectibles spawn count (rings, coins, etc).</summary>
        public int CollectibleCount { get; set; } = 0;

        public Level(string levelId, string levelName, int levelNumber)
        {
            LevelId = levelId;
            LevelName = levelName;
            LevelNumber = levelNumber;
        }

        /// <summary>Add a checkpoint to this level.</summary>
        public void AddCheckpoint(LevelCheckpoint checkpoint)
        {
            Checkpoints.Add(checkpoint);
        }

        /// <summary>Get checkpoint by ID.</summary>
        public LevelCheckpoint? GetCheckpoint(string checkpointId)
        {
            return Checkpoints.Find(cp => cp.CheckpointId == checkpointId);
        }

        /// <summary>Get the nearest checkpoint to a position.</summary>
        public LevelCheckpoint? GetNearestCheckpoint(Vector3 position)
        {
            if (Checkpoints.Count == 0) return null;

            LevelCheckpoint? nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (var checkpoint in Checkpoints)
            {
                float distance = Vector3.Distance(position, checkpoint.Position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = checkpoint;
                }
            }

            return nearest;
        }
    }

    /// <summary>
    /// Checkpoint within a level - marks a respawn point for the player.
    /// </summary>
    public class LevelCheckpoint
    {
        /// <summary>Unique ID for this checkpoint.</summary>
        public string CheckpointId { get; set; }

        /// <summary>Display name.</summary>
        public string Name { get; set; }

        /// <summary>Position where player respawns.</summary>
        public Vector3 Position { get; set; }

        /// <summary>Camera target when at this checkpoint.</summary>
        public Vector3 CameraTarget { get; set; }

        /// <summary>Order of progression (lower = earlier).</summary>
        public int Order { get; set; }

        /// <summary>Whether this checkpoint has been activated.</summary>
        public bool IsActivated { get; set; } = false;

        /// <summary>Time when checkpoint was activated.</summary>
        public DateTime ActivationTime { get; set; }

        /// <summary>Callback when checkpoint is reached.</summary>
        public Action<LevelCheckpoint>? OnCheckpointReached { get; set; }

        public LevelCheckpoint(string checkpointId, string name, Vector3 position, int order = 0)
        {
            CheckpointId = checkpointId;
            Name = name;
            Position = position;
            Order = order;
        }

        /// <summary>Activate this checkpoint.</summary>
        public void Activate()
        {
            if (!IsActivated)
            {
                IsActivated = true;
                ActivationTime = DateTime.Now;
                OnCheckpointReached?.Invoke(this);
                Console.WriteLine($"Checkpoint activated: {Name}");
            }
        }
    }
}
