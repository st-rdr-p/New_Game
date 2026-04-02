using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// Represents a single line of dialogue.
    /// </summary>
    public class DialogueLine
    {
        /// <summary>Character name speaking the line.</summary>
        public string Speaker { get; set; }

        /// <summary>The dialogue text.</summary>
        public string Text { get; set; }

        /// <summary>How long to display this line (seconds). 0 = wait for player input.</summary>
        public float Duration { get; set; }

        /// <summary>Audio file to play (if any).</summary>
        public string AudioClip { get; set; }

        /// <summary>Optional callback when this line is displayed.</summary>
        public Action OnLineStart { get; set; }

        /// <summary>Optional callback when this line ends.</summary>
        public Action OnLineEnd { get; set; }

        public DialogueLine(string speaker, string text, float duration = 0)
        {
            Speaker = speaker;
            Text = text;
            Duration = duration;
        }
    }

    /// <summary>
    /// Represents a dialogue choice/option for player branching dialogue.
    /// </summary>
    public class DialogueChoice
    {
        /// <summary>Display text for this choice.</summary>
        public string ChoiceText { get; set; }

        /// <summary>Index of the next dialogue sequence to play.</summary>
        public int NextSequenceIndex { get; set; }

        /// <summary>Optional callback when choice is selected.</summary>
        public Action OnChosen { get; set; }

        public DialogueChoice(string text, int nextSequenceIndex)
        {
            ChoiceText = text;
            NextSequenceIndex = nextSequenceIndex;
        }
    }

    /// <summary>
    /// A single dialogue sequence that can contain multiple lines and branching choices.
    /// </summary>
    public class DialogueSequence
    {
        /// <summary>Unique identifier for this sequence.</summary>
        public int Id { get; set; }

        /// <summary>List of dialogue lines in order.</summary>
        public List<DialogueLine> Lines { get; set; } = new();

        /// <summary>Player's dialogue choices at the end of this sequence (optional).</summary>
        public List<DialogueChoice> Choices { get; set; } = new();

        /// <summary>If no choices, index of next sequence to auto-play.</summary>
        public int NextSequenceIndex { get; set; } = -1;

        /// <summary>Callback when this sequence completes.</summary>
        public Action OnSequenceEnd { get; set; }

        public DialogueSequence(int id)
        {
            Id = id;
        }

        public void AddLine(DialogueLine line)
        {
            Lines.Add(line);
        }

        public void AddChoice(DialogueChoice choice)
        {
            Choices.Add(choice);
        }
    }

    /// <summary>
    /// Component attached to entities that have dialogue (NPCs, enemies, bosses).
    /// </summary>
    public class DialogueComponent : Component
    {
        /// <summary>Name of the entity speaking (display name).</summary>
        public string EntityName { get; set; }

        /// <summary>All dialogue sequences for this entity.</summary>
        public Dictionary<int, DialogueSequence> Sequences { get; set; } = new();

        /// <summary>Current active dialogue sequence index.</summary>
        public int CurrentSequenceIndex { get; private set; } = -1;

        /// <summary>Current line being displayed in the active sequence.</summary>
        public int CurrentLineIndex { get; private set; } = -1;

        /// <summary>Whether dialogue is currently active.</summary>
        public bool IsDialogueActive { get; private set; } = false;

        /// <summary>Callback when dialogue starts.</summary>
        public Action<int> OnDialogueStart { get; set; }  // Pass sequence ID

        /// <summary>Callback when dialogue ends.</summary>
        public Action OnDialogueEnd { get; set; }

        /// <summary>Callback when a choice is displayed.</summary>
        public Action<List<DialogueChoice>> OnChoicesDisplayed { get; set; }

        public DialogueComponent(string entityName)
        {
            EntityName = entityName;
        }

        /// <summary>Start dialogue from a specific sequence.</summary>
        public void StartDialogue(int sequenceId)
        {
            if (Sequences.ContainsKey(sequenceId))
            {
                CurrentSequenceIndex = sequenceId;
                CurrentLineIndex = -1;
                IsDialogueActive = true;
                OnDialogueStart?.Invoke(sequenceId);
            }
        }

        /// <summary>Advance to the next line of dialogue.</summary>
        public bool AdvanceLine()
        {
            if (!IsDialogueActive || CurrentSequenceIndex < 0)
                return false;

            var sequence = Sequences[CurrentSequenceIndex];
            CurrentLineIndex++;

            // Check if we've finished all lines
            if (CurrentLineIndex >= sequence.Lines.Count)
            {
                // Display choices if available
                if (sequence.Choices.Count > 0)
                {
                    OnChoicesDisplayed?.Invoke(sequence.Choices);
                    return true;  // Wait for player choice
                }

                // Auto-advance to next sequence if available
                if (sequence.NextSequenceIndex >= 0)
                {
                    StartDialogue(sequence.NextSequenceIndex);
                    return true;
                }

                // Dialogue complete
                IsDialogueActive = false;
                sequence.OnSequenceEnd?.Invoke();
                OnDialogueEnd?.Invoke();
                return false;
            }

            return true;  // More lines to display
        }

        /// <summary>Choose a dialogue option and continue.</summary>
        public void ChooseOption(int choiceIndex)
        {
            if (!IsDialogueActive || CurrentSequenceIndex < 0)
                return;

            var sequence = Sequences[CurrentSequenceIndex];
            if (choiceIndex < 0 || choiceIndex >= sequence.Choices.Count)
                return;

            var choice = sequence.Choices[choiceIndex];
            choice.OnChosen?.Invoke();

            // Start the next sequence
            if (choice.NextSequenceIndex >= 0)
            {
                StartDialogue(choice.NextSequenceIndex);
            }
            else
            {
                IsDialogueActive = false;
                OnDialogueEnd?.Invoke();
            }
        }

        /// <summary>End current dialogue without playing remaining lines.</summary>
        public void EndDialogue()
        {
            IsDialogueActive = false;
            CurrentSequenceIndex = -1;
            CurrentLineIndex = -1;
            OnDialogueEnd?.Invoke();
        }

        /// <summary>Get the current dialogue line being displayed.</summary>
        public DialogueLine GetCurrentLine()
        {
            if (!IsDialogueActive || CurrentSequenceIndex < 0 || CurrentLineIndex < 0)
                return null;

            var sequence = Sequences[CurrentSequenceIndex];
            if (CurrentLineIndex >= sequence.Lines.Count)
                return null;

            return sequence.Lines[CurrentLineIndex];
        }

        /// <summary>Get choices from the current sequence.</summary>
        public List<DialogueChoice> GetCurrentChoices()
        {
            if (!IsDialogueActive || CurrentSequenceIndex < 0)
                return new();

            return Sequences[CurrentSequenceIndex].Choices;
        }
    }

    /// <summary>
    /// Component that triggers an encounter (opens dialogue, starts combat, etc).
    /// </summary>
    public class EncounterTrigger : Component
    {
        /// <summary>Types of encounters.</summary>
        public enum EncounterType { NPC, Enemy, Boss, CutsceneEvent }

        /// <summary>Type of this encounter.</summary>
        public EncounterType Type { get; set; }

        /// <summary>Dialogue component reference.</summary>
        public DialogueComponent DialogueComponent { get; set; }

        /// <summary>Dialogue sequence ID to start on encounter.</summary>
        public int InitialDialogueSequence { get; set; } = 0;

        /// <summary>Detection range for automatic encounter trigger.</summary>
        public float TriggerRange { get; set; } = 10f;

        /// <summary>Whether this encounter has already been triggered.</summary>
        public bool HasBeenTriggered { get; set; } = false;

        /// <summary>Whether this encounter can trigger multiple times.</summary>
        public bool CanTriggerMultipleTimes { get; set; } = true;

        /// <summary>Callback when encounter is triggered.</summary>
        public Action OnEncounterStart { get; set; }

        /// <summary>Callback when encounter ends.</summary>
        public Action OnEncounterEnd { get; set; }

        /// <summary>For enemies: whether to start combat after dialogue.</summary>
        public bool StartCombatAfter { get; set; } = false;

        public EncounterTrigger(EncounterType type, DialogueComponent dialogue)
        {
            Type = type;
            DialogueComponent = dialogue;
        }

        /// <summary>Trigger this encounter.</summary>
        public void Trigger()
        {
            if (!CanTriggerMultipleTimes && HasBeenTriggered)
                return;

            HasBeenTriggered = true;
            OnEncounterStart?.Invoke();

            if (DialogueComponent != null)
            {
                DialogueComponent.StartDialogue(InitialDialogueSequence);
            }
        }

        /// <summary>End the encounter.</summary>
        public void End()
        {
            OnEncounterEnd?.Invoke();

            if (StartCombatAfter && DialogueComponent.Owner.TryGetComponent<EnemyAI>(out var enemyAI))
            {
                enemyAI.CurrentState = EnemyAI.State.Chase;
            }
        }
    }
}
