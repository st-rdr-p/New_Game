using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// System that manages scene loading, unloading, and transitions.
    /// Handles active scene state and scene cleanup.
    /// </summary>
    public class SceneManager : ISystem
    {
        /// <summary>Currently active scene.</summary>
        private Scene? _activeScene;

        /// <summary>Scene being loaded (for transition effects).</summary>
        private Scene? _loadingScene;

        /// <summary>Level loader utility.</summary>
        private LevelLoader _levelLoader;

        /// <summary>Current active level.</summary>
        private Level? _currentLevel;

        /// <summary>Progress through level (rooms/zones cleared).</summary>
        private int _progressionPoints = 0;

        /// <summary>Whether a scene transition is in progress.</summary>
        public bool IsTransitioning { get; private set; } = false;

        /// <summary>Callback when scene is loaded.</summary>
        public Action<Scene, Level>? OnSceneLoaded { get; set; }

        /// <summary>Callback when scene is unloaded.</summary>
        public Action<Scene, Level>? OnSceneUnloaded { get; set; }

        /// <summary>Callback on scene transition start.</summary>
        public Action<Level?, Level>? OnTransitionStart { get; set; }

        /// <summary>Callback on scene transition complete.</summary>
        public Action<Level>? OnTransitionComplete { get; set; }

        /// <summary>List of available levels in the game.</summary>
        private Dictionary<string, Level> _loadedLevels = new();

        /// <summary>Level progression history.</summary>
        private List<string> _levelHistory = new();

        /// <summary>Transition fade time in seconds.</summary>
        public float TransitionDuration { get; set; } = 1.0f;

        /// <summary>Current transition progress (0-1).</summary>
        private float _transitionProgress = 0;

        /// <summary>Transition direction (fade in or out).</summary>
        private bool _fadingOut = false;

        /// <summary>Game reference for entity management.</summary>
        private Game _game;

        public SceneManager(Game game, IEngineAudio audio)
        {
            _game = game;
            _levelLoader = new LevelLoader(game, audio);
        }

        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            // Handle scene transitions
            if (IsTransitioning)
            {
                UpdateTransition(deltaTime);
            }
        }

        /// <summary>Load a level into a new scene.</summary>
        public void LoadLevel(Level level)
        {
            if (level == null)
            {
                Console.WriteLine("ERROR: Cannot load null level");
                return;
            }

            // Start transition
            if (_activeScene != null)
            {
                OnTransitionStart?.Invoke(_currentLevel, level);
            }

            IsTransitioning = true;
            _fadingOut = true;
            _transitionProgress = 0;

            _currentLevel = level;
            _levelHistory.Add(level.LevelId);

            // Load the level
            var newScene = _levelLoader.LoadLevel(level);
            _loadingScene = newScene;
        }

        /// <summary>Register a level to be available for loading.</summary>
        public void RegisterLevel(Level level)
        {
            _loadedLevels[level.LevelId] = level;
            Console.WriteLine($"Registered level: {level.LevelName}");
        }

        /// <summary>Load a level by ID.</summary>
        public void LoadLevelById(string levelId)
        {
            if (_loadedLevels.TryGetValue(levelId, out var level))
            {
                LoadLevel(level);
            }
            else
            {
                Console.WriteLine($"ERROR: Level not found: {levelId}");
            }
        }

        /// <summary>Get current active scene.</summary>
        public Scene? GetActiveScene()
        {
            return _activeScene;
        }

        /// <summary>Get current active level.</summary>
        public Level? GetCurrentLevel()
        {
            return _currentLevel;
        }

        /// <summary>Get level by ID from registry.</summary>
        public Level? GetLevel(string levelId)
        {
            _loadedLevels.TryGetValue(levelId, out var level);
            return level;
        }

        /// <summary>Get next level (following progression).</summary>
        public Level? GetNextLevel()
        {
            if (_currentLevel == null) return null;

            string nextLevelId = $"level_{_currentLevel.LevelNumber + 1}";
            return GetLevel(nextLevelId);
        }

        /// <summary>Add progression points (for level completion tracking).</summary>
        public void AddProgressionPoints(int points)
        {
            _progressionPoints += points;
            Console.WriteLine($"Progression: +{points} (Total: {_progressionPoints})");
        }

        /// <summary>Get total progression points earned.</summary>
        public int GetProgressionPoints()
        {
            return _progressionPoints;
        }

        /// <summary>Get level history (levels visited in order).</summary>
        public List<string> GetLevelHistory()
        {
            return new List<string>(_levelHistory);
        }

        /// <summary>Unload current scene and cleanup.</summary>
        public void UnloadScene()
        {
            if (_activeScene != null)
            {
                Console.WriteLine($"Unloading scene: {_activeScene.Name}");
                _currentLevel?.OnLevelUnload?.Invoke(_currentLevel);
                if (_currentLevel != null)
                {
                    OnSceneUnloaded?.Invoke(_activeScene, _currentLevel);
                }
                _activeScene = null;
            }
        }

        /// <summary>Reload current level.</summary>
        public void ReloadCurrentLevel()
        {
            if (_currentLevel != null)
            {
                UnloadScene();
                LoadLevel(_currentLevel);
            }
        }

        /// <summary>Update scene transition animation.</summary>
        private void UpdateTransition(float deltaTime)
        {
            _transitionProgress += deltaTime / TransitionDuration;

            if (_fadingOut)
            {
                // Fade out phase
                if (_transitionProgress >= 1.0f)
                {
                    // Switch scenes at midpoint
                    UnloadScene();
                    _activeScene = _loadingScene;
                    _loadingScene = null;

                    if (_activeScene != null && _currentLevel != null)
                    {
                        OnSceneLoaded?.Invoke(_activeScene, _currentLevel);
                    }

                    _fadingOut = false;
                    _transitionProgress = 0;
                }
            }
            else
            {
                // Fade in phase
                if (_transitionProgress >= 1.0f)
                {
                    IsTransitioning = false;
                    if (_currentLevel != null)
                    {
                        OnTransitionComplete?.Invoke(_currentLevel);
                    }
                    Console.WriteLine($"Transition complete. Now in: {_currentLevel?.LevelName ?? "Unknown"}");
                }
            }
        }
    }
}
