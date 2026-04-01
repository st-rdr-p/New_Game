using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// System that manages dialogue updates, encounters, and dialogue-triggered events.
    /// </summary>
    public class DialogueSystem : ISystem
    {
        /// <summary>Currently active dialogue (if any).</summary>
        private DialogueComponent _activeDialogue;

        /// <summary>Time remaining for current dialogue line.</summary>
        private float _lineTimer = 0;

        /// <summary>Player entity reference (for proximity checks).</summary>
        private Entity _player;

        /// <summary>Callback for UI to display dialogue.</summary>
        public Action<DialogueLine> OnDisplayDialogueLine { get; set; }

        /// <summary>Callback for UI to display choices.</summary>
        public Action<List<DialogueChoice>, Action<int>> OnDisplayChoices { get; set; }

        /// <summary>Callback for UI to hide dialogue.</summary>
        public Action OnHideDialogue { get; set; }

        private readonly IEngineAudio _audio;

        public DialogueSystem(IEngineAudio audio)
        {
            _audio = audio;
        }

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

            // Update active dialogue
            if (_activeDialogue != null && _activeDialogue.IsDialogueActive)
            {
                UpdateActiveDialogue(deltaTime);
            }

            // Check for encounter triggers
            CheckEncounterTriggers(entityList);
        }

        /// <summary>Update the currently active dialogue.</summary>
        private void UpdateActiveDialogue(float deltaTime)
        {
            var currentLine = _activeDialogue.GetCurrentLine();

            if (currentLine == null)
            {
                // No line displayed yet, show first line
                if (!_activeDialogue.AdvanceLine())
                {
                    _activeDialogue = null;
                    OnHideDialogue?.Invoke();
                    return;
                }

                currentLine = _activeDialogue.GetCurrentLine();
                if (currentLine != null)
                {
                    DisplayLine(currentLine);
                }
                return;
            }

            // Update line timer
            if (currentLine.Duration > 0)
            {
                _lineTimer -= deltaTime;

                // Auto-advance when time is up
                if (_lineTimer <= 0)
                {
                    AdvanceCurrentDialogue();
                }
            }
        }

        /// <summary>Display a dialogue line on screen.</summary>
        private void DisplayLine(DialogueLine line)
        {
            _lineTimer = line.Duration;

            // Play audio if available
            if (!string.IsNullOrEmpty(line.AudioClip))
            {
                _audio?.PlaySound(line.AudioClip);
            }

            // Invoke callback for line start
            line.OnLineStart?.Invoke();

            // Notify UI to display
            OnDisplayDialogueLine?.Invoke(line);
        }

        /// <summary>Advance to next dialogue line or show choices.</summary>
        public void AdvanceCurrentDialogue()
        {
            if (_activeDialogue == null || !_activeDialogue.IsDialogueActive)
                return;

            if (!_activeDialogue.AdvanceLine())
            {
                _activeDialogue = null;
                OnHideDialogue?.Invoke();
                return;
            }

            var currentLine = _activeDialogue.GetCurrentLine();
            if (currentLine != null)
            {
                DisplayLine(currentLine);
            }
            else
            {
                // Show choices
                var choices = _activeDialogue.GetCurrentChoices();
                if (choices.Count > 0)
                {
                    OnDisplayChoices?.Invoke(choices, ChooseDialogueOption);
                }
            }
        }

        /// <summary>Player selected a dialogue choice.</summary>
        public void ChooseDialogueOption(int choiceIndex)
        {
            if (_activeDialogue == null)
                return;

            _activeDialogue.ChooseOption(choiceIndex);

            if (_activeDialogue.IsDialogueActive)
            {
                AdvanceCurrentDialogue();
            }
            else
            {
                _activeDialogue = null;
                OnHideDialogue?.Invoke();
            }
        }

        /// <summary>Start a new dialogue conversation.</summary>
        public void StartDialogue(DialogueComponent dialogueComponent, int sequenceId = 0)
        {
            // End current dialogue if active
            if (_activeDialogue != null && _activeDialogue.IsDialogueActive)
            {
                _activeDialogue.EndDialogue();
            }

            _activeDialogue = dialogueComponent;
            _lineTimer = 0;
            _activeDialogue.StartDialogue(sequenceId);
            AdvanceCurrentDialogue();
        }

        /// <summary>End current dialogue.</summary>
        public void EndCurrentDialogue()
        {
            if (_activeDialogue != null)
            {
                _activeDialogue.EndDialogue();
                _activeDialogue = null;
                OnHideDialogue?.Invoke();
            }
        }

        /// <summary>Check for encounter triggers in proximity to player.</summary>
        private void CheckEncounterTriggers(List<Entity> entities)
        {
            if (_player == null || !_player.TryGetComponent<Transform3D>(out var playerTransform))
                return;

            foreach (var entity in entities)
            {
                if (!entity.TryGetComponent<EncounterTrigger>(out var trigger))
                    continue;

                if (!entity.TryGetComponent<Transform3D>(out var entityTransform))
                    continue;

                // Check if player is in range
                var distance = Vector3.Distance(playerTransform.Position, entityTransform.Position);
                if (distance <= trigger.TriggerRange)
                {
                    // Check if should trigger
                    if (!trigger.HasBeenTriggered || trigger.CanTriggerMultipleTimes)
                    {
                        TriggerEncounter(trigger);
                    }
                }
            }
        }

        /// <summary>Trigger an encounter.</summary>
        private void TriggerEncounter(EncounterTrigger trigger)
        {
            trigger.Trigger();

            if (trigger.DialogueComponent != null)
            {
                StartDialogue(trigger.DialogueComponent, trigger.InitialDialogueSequence);
            }
        }

        /// <summary>Check if dialogue is currently active.</summary>
        public bool IsDialogueActive => _activeDialogue != null && _activeDialogue.IsDialogueActive;

        /// <summary>Get the currently active dialogue component (if any).</summary>
        public DialogueComponent GetActiveDialogue => _activeDialogue;
    }

    /// <summary>
    /// Helper builder for creating dialogue sequences easily.
    /// </summary>
    public class DialogueBuilder
    {
        private DialogueComponent _dialogueComponent;
        private DialogueSequence _currentSequence;
        private int _sequenceIdCounter = 0;

        public DialogueBuilder(string entityName)
        {
            _dialogueComponent = new DialogueComponent(entityName);
        }

        /// <summary>Start a new dialogue sequence.</summary>
        public DialogueBuilder Sequence()
        {
            _currentSequence = new DialogueSequence(_sequenceIdCounter++);
            _dialogueComponent.Sequences[_currentSequence.Id] = _currentSequence;
            return this;
        }

        /// <summary>Add a line to the current sequence.</summary>
        public DialogueBuilder Line(string speaker, string text, float duration = 0, string audioClip = null)
        {
            if (_currentSequence == null)
                Sequence();

            var line = new DialogueLine(speaker, text, duration) { AudioClip = audioClip };
            _currentSequence.AddLine(line);
            return this;
        }

        /// <summary>Add a choice to the current sequence.</summary>
        public DialogueBuilder Choice(string choiceText, int nextSequenceId)
        {
            if (_currentSequence == null)
                Sequence();

            var choice = new DialogueChoice(choiceText, nextSequenceId);
            _currentSequence.AddChoice(choice);
            return this;
        }

        /// <summary>Set the next sequence to auto-play after current.</summary>
        public DialogueBuilder ThenNext(int sequenceId)
        {
            if (_currentSequence != null)
                _currentSequence.NextSequenceIndex = sequenceId;
            return this;
        }

        /// <summary>Set a callback when this sequence ends.</summary>
        public DialogueBuilder OnEnd(Action callback)
        {
            if (_currentSequence != null)
                _currentSequence.OnSequenceEnd = callback;
            return this;
        }

        /// <summary>Build and return the dialogue component.</summary>
        public DialogueComponent Build()
        {
            return _dialogueComponent;
        }

        /// <summary>Build and apply to entity.</summary>
        public DialogueComponent BuildFor(Entity entity)
        {
            return entity.AddComponent(_dialogueComponent);
        }
    }
}
