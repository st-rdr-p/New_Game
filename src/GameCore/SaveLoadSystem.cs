using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace GameCore
{
    /// <summary>
    /// Manages saving and loading game state.
    /// </summary>
    public class SaveLoadSystem : ISystem
    {
        private const string SaveDirectory = "Saves";
        private const string SaveFileExtension = ".sav";
        private const int MaxSaveSlots = 10;

        private Game _game;
        private string _currentSaveFile;
        private float _playtime = 0;

        /// <summary>Callback when save starts.</summary>
        public Action OnSaveStart { get; set; }

        /// <summary>Callback when save completes.</summary>
        public Action<bool> OnSaveComplete { get; set; }

        /// <summary>Callback when load starts.</summary>
        public Action OnLoadStart { get; set; }

        /// <summary>Callback when load completes.</summary>
        public Action<bool> OnLoadComplete { get; set; }

        /// <summary>Callback for save progress updates.</summary>
        public Action<string> OnSaveMessage { get; set; }

        public SaveLoadSystem(Game game)
        {
            _game = game;
            CreateSaveDirectory();
        }

        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            // Track playtime
            _playtime += deltaTime;
        }

        /// <summary>Create save directory if it doesn't exist.</summary>
        private void CreateSaveDirectory()
        {
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
                OnSaveMessage?.Invoke($"Created save directory: {SaveDirectory}");
            }
        }

        /// <summary>Save game to a slot.</summary>
        public bool SaveGame(int slotIndex, string saveName, IEnumerable<Entity> entities)
        {
            if (slotIndex < 0 || slotIndex >= MaxSaveSlots)
            {
                OnSaveMessage?.Invoke($"Invalid slot index: {slotIndex}");
                return false;
            }

            OnSaveStart?.Invoke();

            try
            {
                var gameState = CaptureGameState(saveName, entities);
                var filePath = GetSaveFilePath(slotIndex);

                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(gameState, options);
                File.WriteAllText(filePath, json);

                _currentSaveFile = filePath;
                OnSaveMessage?.Invoke($"Game saved to slot {slotIndex}: {saveName}");
                OnSaveComplete?.Invoke(true);

                return true;
            }
            catch (Exception ex)
            {
                OnSaveMessage?.Invoke($"Save failed: {ex.Message}");
                OnSaveComplete?.Invoke(false);
                return false;
            }
        }

        /// <summary>Load game from a slot.</summary>
        public bool LoadGame(int slotIndex, IEnumerable<Entity> entities)
        {
            if (slotIndex < 0 || slotIndex >= MaxSaveSlots)
            {
                OnSaveMessage?.Invoke($"Invalid slot index: {slotIndex}");
                return false;
            }

            OnLoadStart?.Invoke();

            try
            {
                var filePath = GetSaveFilePath(slotIndex);
                if (!File.Exists(filePath))
                {
                    OnSaveMessage?.Invoke($"Save file not found: {filePath}");
                    OnLoadComplete?.Invoke(false);
                    return false;
                }

                var json = File.ReadAllText(filePath);
                var gameState = JsonSerializer.Deserialize<GameState>(json);

                if (gameState == null)
                {
                    OnSaveMessage?.Invoke("Failed to deserialize save file");
                    OnLoadComplete?.Invoke(false);
                    return false;
                }

                RestoreGameState(gameState, entities);
                _currentSaveFile = filePath;
                _playtime = gameState.PlaytimeSeconds;

                OnSaveMessage?.Invoke($"Game loaded from slot {slotIndex}: {gameState.PlayerState.Level}");
                OnLoadComplete?.Invoke(true);

                return true;
            }
            catch (Exception ex)
            {
                OnSaveMessage?.Invoke($"Load failed: {ex.Message}");
                OnLoadComplete?.Invoke(false);
                return false;
            }
        }

        /// <summary>Get save slot information.</summary>
        public SaveSlot GetSaveSlot(int slotIndex)
        {
            var slot = new SaveSlot { SlotIndex = slotIndex };
            var filePath = GetSaveFilePath(slotIndex);

            if (!File.Exists(filePath))
                return slot;

            try
            {
                var json = File.ReadAllText(filePath);
                var gameState = JsonSerializer.Deserialize<GameState>(json);

                if (gameState != null)
                {
                    slot.IsValid = true;
                    slot.SaveName = gameState.PlayerState.Level.ToString();
                    slot.SaveTime = gameState.SaveTime;
                    slot.PlaytimeSeconds = gameState.PlaytimeSeconds;
                    slot.CurrentLevel = gameState.CurrentLevel;
                    slot.PlayerLocation = $"({gameState.PlayerState.PositionX:F1}, {gameState.PlayerState.PositionY:F1}, {gameState.PlayerState.PositionZ:F1})";
                    slot.ProgressPercent = gameState.PlayerState.Level * 10; // Example calculation
                }
            }
            catch
            {
                // Failed to read slot
            }

            return slot;
        }

        /// <summary>Get all save slots.</summary>
        public List<SaveSlot> GetAllSaveSlots()
        {
            var slots = new List<SaveSlot>();
            for (int i = 0; i < MaxSaveSlots; i++)
            {
                slots.Add(GetSaveSlot(i));
            }
            return slots;
        }

        /// <summary>Delete a save slot.</summary>
        public bool DeleteSaveSlot(int slotIndex)
        {
            try
            {
                var filePath = GetSaveFilePath(slotIndex);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    OnSaveMessage?.Invoke($"Deleted save slot {slotIndex}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnSaveMessage?.Invoke($"Failed to delete save: {ex.Message}");
            }
            return false;
        }

        /// <summary>Check if a save slot is valid.</summary>
        public bool SlotHasSave(int slotIndex)
        {
            return File.Exists(GetSaveFilePath(slotIndex));
        }

        /// <summary>Capture current game state.</summary>
        private GameState CaptureGameState(string saveName, IEnumerable<Entity> entities)
        {
            var gameState = new GameState
            {
                CurrentLevel = "CurrentLevel", // TODO: Get from game
                PlaytimeSeconds = _playtime,
                SaveTime = DateTime.Now,
                GameVariables = new Dictionary<string, object>()
            };

            var entityList = new List<Entity>(entities);

            // Capture player state
            foreach (var entity in entityList)
            {
                if (entity.TryGetComponent<PlayerSaveableComponent>(out var playerSave))
                {
                    gameState.PlayerState = playerSave.GetState();
                    break;
                }
            }

            // Capture all saveable entities
            foreach (var entity in entityList)
            {
                if (entity.TryGetComponent<SaveableComponent>(out var saveable) && saveable.IsSaveable)
                {
                    gameState.Entities.Add(saveable.GetState());
                }
            }

            return gameState;
        }

        /// <summary>Restore game state from save.</summary>
        private void RestoreGameState(GameState gameState, IEnumerable<Entity> entities)
        {
            var entityList = new List<Entity>(entities);

            // Restore player
            if (gameState.PlayerState != null)
            {
                foreach (var entity in entityList)
                {
                    if (entity.TryGetComponent<PlayerSaveableComponent>(out var playerSave))
                    {
                        playerSave.RestoreState(gameState.PlayerState);
                        break;
                    }
                }
            }

            // Restore entities
            foreach (var savedEntity in gameState.Entities)
            {
                var entity = entityList.FirstOrDefault(e => e.Id == savedEntity.EntityId);
                if (entity != null && entity.TryGetComponent<SaveableComponent>(out var saveable))
                {
                    saveable.RestoreState(savedEntity);
                }
            }

            // Restore quests
            foreach (var quest in gameState.CompletedQuests)
            {
                // TODO: Apply quest state
            }
        }

        /// <summary>Get save file path for slot.</summary>
        private string GetSaveFilePath(int slotIndex)
        {
            return Path.Combine(SaveDirectory, $"save_{slotIndex}{SaveFileExtension}");
        }

        /// <summary>Get total playtime in hours.</summary>
        public float GetPlaytimeHours()
        {
            return _playtime / 3600f;
        }

        /// <summary>Format playtime for display.</summary>
        public string FormatPlaytime(float seconds)
        {
            var hours = (int)(seconds / 3600);
            var minutes = (int)((seconds % 3600) / 60);
            var secs = (int)(seconds % 60);
            return $"{hours}h {minutes}m {secs}s";
        }
    }
}
