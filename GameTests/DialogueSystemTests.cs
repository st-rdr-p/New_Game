using NUnit.Framework;
using GameCore;
using System.Collections.Generic;

namespace GameTests
{
    /// <summary>
    /// Tests for the Dialogue System - branching dialogue, encounters, voice clips.
    /// </summary>
    [TestFixture]
    public class DialogueSystemTests
    {
        private DialogueComponent? _dialogueComponent;

        [SetUp]
        public void Setup()
        {
            _dialogueComponent = new DialogueComponent("Test NPC");
        }

        #region Dialogue Line Tests

        [Test]
        public void DialogueLine_InitializesWithProperties()
        {
            // Arrange & Act
            var line = new DialogueLine("NPC", "Hello there!", 3.0f);

            // Assert
            Assert.That(line.Speaker, Is.EqualTo("NPC"));
            Assert.That(line.Text, Is.EqualTo("Hello there!"));
            Assert.That(line.Duration, Is.EqualTo(3.0f));
        }

        [Test]
        public void DialogueLine_DefaultDuration()
        {
            // Arrange & Act
            var line = new DialogueLine("NPC", "Test dialog");

            // Assert
            Assert.That(line.Duration, Is.EqualTo(0)); // Default wait for input
        }

        [Test]
        public void DialogueLine_AudioClip()
        {
            // Arrange & Act
            var line = new DialogueLine("NPC", "Hello", 2.0f)
            {
                AudioClip = "voice/npc_greeting.ogg"
            };

            // Assert
            Assert.That(line.AudioClip, Is.EqualTo("voice/npc_greeting.ogg"));
        }

        [Test]
        public void DialogueLine_Callbacks()
        {
            // Arrange
            bool lineStarted = false;
            bool lineEnded = false;
            var line = new DialogueLine("NPC", "Test")
            {
                OnLineStart = () => { lineStarted = true; },
                OnLineEnd = () => { lineEnded = true; }
            };

            // Act
            line.OnLineStart?.Invoke();
            line.OnLineEnd?.Invoke();

            // Assert
            Assert.That(lineStarted, Is.True);
            Assert.That(lineEnded, Is.True);
        }

        #endregion

        #region Dialogue Component Tests

        [Test]
        public void DialogueComponent_InitializesWithName()
        {
            // Arrange & Act
            var dialogue = new DialogueComponent("Test NPC");

            // Assert
            Assert.That(dialogue.EntityName, Is.EqualTo("Test NPC"));
            Assert.That(dialogue.IsDialogueActive, Is.False);
            Assert.That(dialogue.CurrentSequenceIndex, Is.EqualTo(-1));
        }

        [Test]
        public void DialogueComponent_CreateAndAddSequence()
        {
            // Arrange
            var dialogue = new DialogueComponent("NPC");
            var sequence = new DialogueSequence(0);
            sequence.AddLine(new DialogueLine("NPC", "Hello"));
            sequence.AddLine(new DialogueLine("NPC", "Goodbye"));

            // Act
            dialogue.Sequences[0] = sequence;

            // Assert
            Assert.That(dialogue.Sequences[0].Lines.Count, Is.EqualTo(2));
            Assert.That(dialogue.Sequences[0].Lines[0].Text, Is.EqualTo("Hello"));
        }

        [Test]
        public void DialogueComponent_StartDialogue()
        {
            // Arrange
            var dialogue = new DialogueComponent("NPC");
            var sequence = new DialogueSequence(0);
            sequence.AddLine(new DialogueLine("NPC", "Hello"));
            dialogue.Sequences[0] = sequence;

            // Act
            dialogue.StartDialogue(0);

            // Assert
            Assert.That(dialogue.IsDialogueActive, Is.True);
            Assert.That(dialogue.CurrentSequenceIndex, Is.EqualTo(0));
        }

        [Test]
        public void DialogueComponent_AdvanceLine()
        {
            // Arrange
            var dialogue = new DialogueComponent("NPC");
            var sequence = new DialogueSequence(0);
            sequence.AddLine(new DialogueLine("NPC", "Line 1"));
            sequence.AddLine(new DialogueLine("NPC", "Line 2"));
            sequence.AddLine(new DialogueLine("NPC", "Line 3"));
            dialogue.Sequences[0] = sequence;

            dialogue.StartDialogue(0);

            // Act & Assert
            Assert.That(dialogue.AdvanceLine(), Is.True); // Move to line 0
            Assert.That(dialogue.CurrentLineIndex, Is.EqualTo(0));
            Assert.That(dialogue.GetCurrentLine().Text, Is.EqualTo("Line 1"));

            Assert.That(dialogue.AdvanceLine(), Is.True); // Move to line 1
            Assert.That(dialogue.GetCurrentLine().Text, Is.EqualTo("Line 2"));

            Assert.That(dialogue.AdvanceLine(), Is.True); // Move to line 2
            Assert.That(dialogue.GetCurrentLine().Text, Is.EqualTo("Line 3"));
        }

        [Test]
        public void DialogueComponent_EndDialogue()
        {
            // Arrange
            var dialogue = new DialogueComponent("NPC");
            var sequence = new DialogueSequence(0);
            sequence.AddLine(new DialogueLine("NPC", "Test"));
            dialogue.Sequences[0] = sequence;
            dialogue.StartDialogue(0);

            // Act
            dialogue.EndDialogue();

            // Assert
            Assert.That(dialogue.IsDialogueActive, Is.False);
        }

        [Test]
        public void DialogueComponent_GetCurrentLine()
        {
            // Arrange
            var dialogue = new DialogueComponent("NPC");
            var sequence = new DialogueSequence(0);
            var line = new DialogueLine("NPC", "Test Line");
            sequence.AddLine(line);
            dialogue.Sequences[0] = sequence;

            dialogue.StartDialogue(0);
            dialogue.AdvanceLine();

            // Act
            var currentLine = dialogue.GetCurrentLine();

            // Assert
            Assert.That(currentLine.Text, Is.EqualTo("Test Line"));
        }

        #endregion

        #region Dialogue Sequence Tests

        [Test]
        public void DialogueSequence_InitializesWithId()
        {
            // Arrange & Act
            var sequence = new DialogueSequence(5);

            // Assert
            Assert.That(sequence.Id, Is.EqualTo(5));
            Assert.That(sequence.Lines.Count, Is.EqualTo(0));
            Assert.That(sequence.Choices.Count, Is.EqualTo(0));
        }

        [Test]
        public void DialogueSequence_AddLine()
        {
            // Arrange
            var sequence = new DialogueSequence(0);
            var line = new DialogueLine("NPC", "Hello");

            // Act
            sequence.AddLine(line);

            // Assert
            Assert.That(sequence.Lines.Count, Is.EqualTo(1));
            Assert.That(sequence.Lines[0].Text, Is.EqualTo("Hello"));
        }

        [Test]
        public void DialogueSequence_MultipleLines()
        {
            // Arrange
            var sequence = new DialogueSequence(0);

            // Act
            sequence.AddLine(new DialogueLine("NPC", "Line 1"));
            sequence.AddLine(new DialogueLine("NPC", "Line 2"));
            sequence.AddLine(new DialogueLine("NPC", "Line 3"));

            // Assert
            Assert.That(sequence.Lines.Count, Is.EqualTo(3));
        }

        [Test]
        public void DialogueSequence_OnSequenceEnd()
        {
            // Arrange
            bool sequenceEnded = false;
            var sequence = new DialogueSequence(0)
            {
                OnSequenceEnd = () => { sequenceEnded = true; }
            };

            // Act
            sequence.OnSequenceEnd?.Invoke();

            // Assert
            Assert.That(sequenceEnded, Is.True);
        }

        #endregion

        #region Dialogue Choice Tests

        [Test]
        public void DialogueChoice_InitializesWithProperties()
        {
            // Arrange & Act
            var choice = new DialogueChoice("Attack", 1);

            // Assert
            Assert.That(choice.ChoiceText, Is.EqualTo("Attack"));
            Assert.That(choice.NextSequenceIndex, Is.EqualTo(1));
        }

        [Test]
        public void DialogueChoice_MultipleChoices()
        {
            // Arrange
            var seq = new DialogueSequence(0);

            // Act
            seq.AddChoice(new DialogueChoice("Option 1", 1));
            seq.AddChoice(new DialogueChoice("Option 2", 2));
            seq.AddChoice(new DialogueChoice("Option 3", 3));

            // Assert
            Assert.That(seq.Choices.Count, Is.EqualTo(3));
            Assert.That(seq.Choices[0].ChoiceText, Is.EqualTo("Option 1"));
        }

        [Test]
        public void DialogueChoice_Callback()
        {
            // Arrange
            bool chosenCalled = false;
            var choice = new DialogueChoice("Test", 0)
            {
                OnChosen = () => { chosenCalled = true; }
            };

            // Act
            choice.OnChosen?.Invoke();

            // Assert
            Assert.That(chosenCalled, Is.True);
        }

        #endregion

        #region Encounter Trigger Tests

        [Test]
        public void EncounterTrigger_InitializesWithType()
        {
            // Arrange
            var dialogue = new DialogueComponent("NPC");

            // Act
            var trigger = new EncounterTrigger(EncounterTrigger.EncounterType.NPC, dialogue);

            // Assert
            Assert.That(trigger.Type, Is.EqualTo(EncounterTrigger.EncounterType.NPC));
            Assert.That(trigger.DialogueComponent, Is.EqualTo(dialogue));
        }

        [Test]
        public void EncounterTrigger_SetTriggerRange()
        {
            // Arrange
            var dialogue = new DialogueComponent("NPC");
            var trigger = new EncounterTrigger(EncounterTrigger.EncounterType.NPC, dialogue);

            // Act
            trigger.TriggerRange = 15.0f;

            // Assert
            Assert.That(trigger.TriggerRange, Is.EqualTo(15.0f));
        }

        [Test]
        public void EncounterTrigger_MarkTriggered()
        {
            // Arrange
            var dialogue = new DialogueComponent("NPC");
            var trigger = new EncounterTrigger(EncounterTrigger.EncounterType.NPC, dialogue);

            // Act
            trigger.Trigger();

            // Assert
            Assert.That(trigger.HasBeenTriggered, Is.True);
        }

        [Test]
        public void EncounterTrigger_CanTriggerMultipleTimes()
        {
            // Arrange
            var dialogue = new DialogueComponent("NPC");
            var trigger = new EncounterTrigger(EncounterTrigger.EncounterType.NPC, dialogue)
            {
                CanTriggerMultipleTimes = true
            };

            // Act & Assert
            trigger.Trigger();
            Assert.That(trigger.HasBeenTriggered, Is.True);

            // Should be able to trigger again
            trigger.Trigger();
            Assert.That(trigger.HasBeenTriggered, Is.True);
        }

        [Test]
        public void EncounterTrigger_OnTriggerCallbacks()
        {
            // Arrange
            bool startCalled = false;
            bool endCalled = false;
            var dialogue = new DialogueComponent("NPC");
            var trigger = new EncounterTrigger(EncounterTrigger.EncounterType.NPC, dialogue)
            {
                OnEncounterStart = () => { startCalled = true; },
                OnEncounterEnd = () => { endCalled = true; }
            };

            // Act
            trigger.Trigger();
            trigger.End();

            // Assert
            Assert.That(startCalled, Is.True);
            Assert.That(endCalled, Is.True);
        }

        #endregion

        #region Integration Tests

        [Test]
        public void Dialogue_BranchingPathSimulation()
        {
            // Arrange - Create dialogue with branching paths
            var dialogue = new DialogueComponent("Merchant");

            // Main dialogue sequence
            var mainSeq = new DialogueSequence(0);
            mainSeq.AddLine(new DialogueLine("Merchant", "Welcome to my shop"));
            mainSeq.AddLine(new DialogueLine("Merchant", "What will you buy?"));
            mainSeq.AddChoice(new DialogueChoice("Buy Health Potion", 1));
            mainSeq.AddChoice(new DialogueChoice("Never mind", -1));

            // Branch dialogue
            var branchSeq = new DialogueSequence(1);
            branchSeq.AddLine(new DialogueLine("Merchant", "That will be 50 coins"));

            dialogue.Sequences[0] = mainSeq;
            dialogue.Sequences[1] = branchSeq;

            // Act
            dialogue.StartDialogue(0);
            dialogue.AdvanceLine(); // Display line 0
            dialogue.AdvanceLine(); // Display line 1

            var currentLine = dialogue.GetCurrentLine();
            var currentChoices = dialogue.GetCurrentChoices();

            // Assert
            Assert.That(currentChoices.Count, Is.EqualTo(2));
            Assert.That(currentChoices[0].ChoiceText, Is.EqualTo("Buy Health Potion"));
        }

        [Test]
        public void Dialogue_ChoiceSelection()
        {
            // Arrange
            var dialogue = new DialogueComponent("NPC");
            var seq0 = new DialogueSequence(0);
            seq0.AddLine(new DialogueLine("NPC", "Choose an option"));
            seq0.AddChoice(new DialogueChoice("Option A", 1));
            seq0.AddChoice(new DialogueChoice("Option B", 2));

            var seq1 = new DialogueSequence(1);
            seq1.AddLine(new DialogueLine("NPC", "You chose option A"));

            dialogue.Sequences[0] = seq0;
            dialogue.Sequences[1] = seq1;

            dialogue.StartDialogue(0);
            dialogue.AdvanceLine();

            // Act - Choose option 0
            dialogue.ChooseOption(0);

            // Assert
            Assert.That(dialogue.CurrentSequenceIndex, Is.EqualTo(1));
            Assert.That(dialogue.IsDialogueActive, Is.True);
        }

        [Test]
        public void Dialogue_VoiceAssetTracking()
        {
            // Arrange
            var line = new DialogueLine("Hero", "Let's go!")
            {
                AudioClip = "voice/hero_action_01.ogg"
            };

            // Act & Assert
            Assert.That(line.AudioClip, Is.EqualTo("voice/hero_action_01.ogg"));
        }

        [Test]
        public void Dialogue_EncounterTriggerFlow()
        {
            // Arrange
            var dialogue = new DialogueComponent("Boss");
            var bossSeq = new DialogueSequence(0);
            bossSeq.AddLine(new DialogueLine("Boss", "Prepare to face me!"));

            dialogue.Sequences[0] = bossSeq;

            var trigger = new EncounterTrigger(EncounterTrigger.EncounterType.Boss, dialogue)
            {
                TriggerRange = 20.0f,
                StartCombatAfter = true
            };

            // Act
            trigger.Trigger();

            // Assert
            Assert.That(dialogue.IsDialogueActive, Is.True);
            Assert.That(dialogue.CurrentSequenceIndex, Is.EqualTo(0));
            Assert.That(trigger.HasBeenTriggered, Is.True);
        }

        #endregion
    }
}
