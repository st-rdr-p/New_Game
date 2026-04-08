using System;

namespace GameCore
{
    /// <summary>
    /// Component for managing checkpoint interactions and player respawn.
    /// </summary>
    public class CheckpointComponent : Component
    {
        /// <summary>Reference to the level checkpoint data.</summary>
        public LevelCheckpoint? Checkpoint { get; set; }

        /// <summary>Whether this checkpoint is currently active.</summary>
        public bool IsActive { get; set; } = false;

        /// <summary>Activation radius around checkpoint.</summary>
        public float ActivationRadius { get; set; } = 2.0f;

        /// <summary>Callback when player reaches checkpoint.</summary>
        public Action<CheckpointComponent>? OnCheckpointReached { get; set; }

        /// <summary>Visual feedback (particle effect, sound, etc).</summary>
        public string ActivationEffectId { get; set; } = "checkpoint_activate";
    }

    /// <summary>
    /// System that monitors player checkpoints and handles respawn mechanics.
    /// </summary>
    public class CheckpointSystem : ISystem
    {
        private Entity? _player;
        private LevelCheckpoint? _lastActiveCheckpoint;

        public Action<LevelCheckpoint>? OnCheckpointActivated { get; set; }

        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            var entityList = new List<Entity>(entities);

            // Find player if not cached
            if (_player == null)
            {
                foreach (var entity in entityList)
                {
                    if (entity.TryGetComponent<Tag>(out var tag) && tag.Value == "Player")
                    {
                        _player = entity;
                        break;
                    }
                }
            }

            if (_player == null) return;

            if (!_player.TryGetComponent<Transform3D>(out var playerTransform))
                return;

            // Check all checkpoints
            foreach (var entity in entityList)
            {
                if (!entity.TryGetComponent<CheckpointComponent>(out var checkpointComp))
                    continue;

                if (!entity.TryGetComponent<Transform3D>(out var checkpointTransform))
                    continue;

                float distance = Vector3.Distance(playerTransform.Position, checkpointTransform.Position);

                // Activate if in range and not yet activated
                if (distance <= checkpointComp.ActivationRadius && !checkpointComp.IsActive)
                {
                    ActivateCheckpoint(checkpointComp);
                }
            }
        }

        /// <summary>Activate a checkpoint.</summary>
        private void ActivateCheckpoint(CheckpointComponent checkpoint)
        {
            checkpoint.IsActive = true;
            _lastActiveCheckpoint = checkpoint.Checkpoint;

            if (checkpoint.Checkpoint != null)
            {
                Console.WriteLine($"Checkpoint reached: {checkpoint.Checkpoint.Name}");
                checkpoint.Checkpoint.Activate();
                checkpoint.OnCheckpointReached?.Invoke(checkpoint);
                OnCheckpointActivated?.Invoke(checkpoint.Checkpoint);
            }
        }

        /// <summary>Get last activated checkpoint.</summary>
        public LevelCheckpoint? GetLastCheckpoint()
        {
            return _lastActiveCheckpoint;
        }

        /// <summary>Respawn player at last checkpoint.</summary>
        public void RespawnAtCheckpoint(Entity player)
        {
            if (_lastActiveCheckpoint == null)
            {
                Console.WriteLine("No checkpoint to respawn at");
                return;
            }

            if (player.TryGetComponent<Transform3D>(out var transform))
            {
                transform.Position = _lastActiveCheckpoint.Position;

                if (player.TryGetComponent<Health>(out var health))
                {
                    health.Heal(health.MaxHealth); // Full restore
                }

                Console.WriteLine($"Respawned at: {_lastActiveCheckpoint.Name}");
            }
        }
    }

    /// <summary>
    /// Component for scene transition effects (fade in/out).
    /// </summary>
    public class SceneTransitionEffect : Component
    {
        /// <summary>Fade duration in seconds.</summary>
        public float FadeDuration { get; set; } = 1.0f;

        /// <summary>Current fade progress (0-1).</summary>
        public float FadeProgress { get; set; } = 0;

        /// <summary>Whether transitioning in (true) or out (false).</summary>
        public bool IsFadingIn { get; set; } = false;

        /// <summary>Callback when fade completes.</summary>
        public Action? OnFadeComplete { get; set; }

        /// <summary>Color to fade to (usually black for scene transitions).</summary>
        public System.Drawing.Color FadeColor { get; set; } = System.Drawing.Color.Black;
    }

    /// <summary>
    /// System that handles scene transition visual effects (fade).
    /// </summary>
    public class SceneTransitionSystem : ISystem
    {
        private IEngineRenderer _renderer;

        public SceneTransitionSystem(IEngineRenderer renderer)
        {
            _renderer = renderer;
        }

        public void Update(float deltaTime, IEnumerable<Entity> entities)
        {
            var entityList = new List<Entity>(entities);

            foreach (var entity in entityList)
            {
                if (!entity.TryGetComponent<SceneTransitionEffect>(out var transition))
                    continue;

                // Update fade progress
                transition.FadeProgress += deltaTime / transition.FadeDuration;

                if (transition.FadeProgress >= 1.0f)
                {
                    transition.FadeProgress = 1.0f;
                    transition.OnFadeComplete?.Invoke();
                }
            }
        }

        /// <summary>Start fade out effect.</summary>
        public SceneTransitionEffect FadeOut(float duration = 1.0f)
        {
            Console.WriteLine($"Fading out over {duration}s");
            return new SceneTransitionEffect
            {
                FadeDuration = duration,
                IsFadingIn = false,
                FadeProgress = 0
            };
        }

        /// <summary>Start fade in effect.</summary>
        public SceneTransitionEffect FadeIn(float duration = 1.0f)
        {
            Console.WriteLine($"Fading in over {duration}s");
            return new SceneTransitionEffect
            {
                FadeDuration = duration,
                IsFadingIn = true,
                FadeProgress = 0
            };
        }
    }
}
