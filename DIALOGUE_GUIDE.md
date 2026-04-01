# Dialogue & Encounter System

The dialogue system enables NPCs, enemies, and bosses to engage in conversations with the player through dialogue sequences, branching choices, and encounter triggers.

## Core Components

### DialogueLine
Represents a single line of dialogue with speaker, text, duration, and optional audio.

### DialogueSequence  
A collection of dialogue lines that can branch into player choices.

### DialogueComponent
Attached to entities that speak dialogue. Manages sequences and dialogue state.

### EncounterTrigger
Automatically triggers encounters when player approaches (proximity-based or manual).

### DialogueSystem
Manages all active dialogues, encounters, and updates dialogue state each frame.

## Quick Start Example

### Creating an NPC with Dialogue

```csharp
// Create an NPC entity
var npcEntity = new Entity(1);
npcEntity.AddComponent(new Transform3D { Position = new Vector3(10, 0, 0) });
npcEntity.AddComponent(new Tag("NPC"));

// Build dialogue using the DialogueBuilder
var dialogue = new DialogueBuilder("Old Sage")
    .Sequence()
        .Line("Old Sage", "Welcome, traveler! I have been expecting you.", 3f)
        .Line("Old Sage", "I have a quest for you...", 2f)
        .Choice("I'll help you!", 1)
        .Choice("Not interested.", 2)
    .Sequence()
        .Line("Old Sage", "Excellent! Go to the mountain and retrieve the crystal.", 4f)
        .Line("Old Sage", "Return to me when done. Good luck!")
    .Sequence()
        .Line("Old Sage", "Perhaps another time then.", 2f)
    .Build();

npcEntity.AddComponent(dialogue);

// Add encounter trigger
var encounter = new EncounterTrigger(EncounterTrigger.EncounterType.NPC, dialogue)
{
    TriggerRange = 5f,
    CanTriggerMultipleTimes = true
};
npcEntity.AddComponent(encounter);
```

### Creating an Enemy with Encounter Dialogue

```csharp
// Create enemy entity
var enemyEntity = new Entity(2);
enemyEntity.AddComponent(new Transform3D { Position = new Vector3(20, 0, 0) });
enemyEntity.AddComponent(new Tag("Enemy"));
enemyEntity.AddComponent(new Health(30f));
enemyEntity.AddComponent(new EnemyAI 
{ 
    DetectionRange = 25f,
    AttackRange = 3f
});

// Create enemy dialogue
var enemyDialogue = new DialogueBuilder("Goblin")
    .Sequence()
        .Line("Goblin", "INTRUDER! You dare enter my domain?!", 2f)
        .Line("Goblin", "Prepare yourself for a fight!", 2f)
    .Build();

enemyEntity.AddComponent(enemyDialogue);

// Create encounter that starts combat after dialogue
var encounter = new EncounterTrigger(EncounterTrigger.EncounterType.Enemy, enemyDialogue)
{
    TriggerRange = 10f,
    CanTriggerMultipleTimes = false,
    StartCombatAfter = true,
    InitialDialogueSequence = 0
};
encounter.OnEncounterEnd += () =>
{
    // Start combat when dialogue ends
    if (enemyEntity.TryGetComponent<EnemyAI>(out var ai))
    {
        ai.CurrentState = EnemyAI.State.Chase;
    }
};
enemyEntity.AddComponent(encounter);
```

### Creating a Boss with Complex Dialogue Tree

```csharp
// Create boss with multi-sequence dialogue
var bossDialogue = new DialogueBuilder("Dark Lord")
    .Sequence()
        .Line("Dark Lord", "So... you've finally arrived.", 3f)
        .Line("Dark Lord", "I am the Dark Lord, guardian of this realm.", 3f)
        .Choice("I challenge you to a duel!", 1)
        .Choice("Tell me your story.", 2)
        .Choice("I must stop you!", 3)
    .Sequence()
        .Line("Dark Lord", "At last, a warrior worthy of my attention!", 2f)
        .Line("Dark Lord", "Very well. Let us battle!", 2f)
    .Sequence()
        .Line("Dark Lord", "Ah, you wish to understand my past?", 3f)
        .Line("Dark Lord", "Long ago, I was betrayed by those I trusted...", 4f)
        .Line("Dark Lord", "Now I rule alone, with no need for bonds.", 3f)
        .Line("Dark Lord", "But enough talk. PREPARE YOURSELF!", 2f)
    .Sequence()
        .Line("Dark Lord", "Your resolve is admirable, but futile!", 3f)
        .Line("Dark Lord", "NOW we FIGHT!", 2f)
    .Build();

var bossEntity = new Entity(100);
bossEntity.AddComponent(new Transform3D { Position = new Vector3(50, 0, 0) });
bossEntity.AddComponent(new Tag("Boss"));
bossEntity.AddComponent(new Health(100f));
bossEntity.AddComponent(bossDialogue);

var bossEncounter = new EncounterTrigger(EncounterTrigger.EncounterType.Boss, bossDialogue)
{
    TriggerRange = 20f,
    CanTriggerMultipleTimes = false,
    StartCombatAfter = true
};
bossEntity.AddComponent(bossEncounter);
```

### Setting Up the Game with Dialogue System

```csharp
public class MyGame : Game
{
    private DialogueSystem _dialogueSystem;
    private AudioBridge _audio;

    protected override void OnInitialize()
    {
        _audio = new AudioBridge();
        _dialogueSystem = new DialogueSystem(_audio);
        
        // Setup UI callbacks for dialogue display
        _dialogueSystem.OnDisplayDialogueLine += DisplayDialogueLine;
        _dialogueSystem.OnDisplayChoices += DisplayChoices;
        _dialogueSystem.OnHideDialogue += HideDialogue;

        // Add dialogue system to systems
        AddSystem(_dialogueSystem);
    }

    private void DisplayDialogueLine(DialogueLine line)
    {
        // Show dialogue UI with speaker name and text
        Debug.Log($"{line.Speaker}: {line.Text}");
        // TODO: Call UI manager to display dialogue box
    }

    private void DisplayChoices(List<DialogueChoice> choices, Action<int> onChoice)
    {
        // Show choice buttons
        for (int i = 0; i < choices.Count; i++)
        {
            Debug.Log($"[{i}] {choices[i].ChoiceText}");
        }
        // TODO: Call UI manager to display choice buttons
        //       When player clicks a choice, call onChoice(choiceIndex)
    }

    private void HideDialogue()
    {
        // Hide dialogue UI
        Debug.Log("Dialogue ended");
        // TODO: Call UI manager to hide dialogue box
    }
}
```

### Handling Player Input for Dialogue

```csharp
// In your input system or game loop:

if (Input.GetKeyDown(KeyCode.Space))
{
    if (_dialogueSystem.IsDialogueActive)
    {
        // Advance dialogue on spacebar
        _dialogueSystem.AdvanceCurrentDialogue();
    }
}

// Numbering for choices (1-9)
for (int i = 0; i < 9; i++)
{
    if (Input.GetKeyDown(KeyCode.Alpha1 + i))
    {
        _dialogueSystem.ChooseDialogueOption(i);
    }
}
```

## Advanced Features

### Dialogue Callbacks

Trigger actions when dialogue lines start/end:

```csharp
var line = new DialogueLine("NPC", "Let me show you something...", 2f);
line.OnLineStart += () => 
{
    // Play animation, sound effect, or trigger event
    PlayCutsceneAnimation("npc_gestures");
};
line.OnLineEnd += () =>
{
    // Handle end of line (e.g., spawn enemy, open door)
    UnlockDoor("chamber_door");
};
```

### Sequence Callbacks

Execute code when a sequence completes:

```csharp
var sequence = new DialogueSequence(5);
sequence.OnSequenceEnd += () =>
{
    // Quest accepted, spawn objective marker
    SpawnQuestMarker(questLocation);
};
```

### Dynamic Dialogue

Modify dialogue based on game state:

```csharp
var dialogue = new DialogueBuilder("Merchant");
string greeting = playerHasMap ? "Ah, I see you found the map!" : "Welcome back, traveler.";
dialogue.Sequence()
    .Line("Merchant", greeting, 2f);
```

### Multiple Dialogue Triggers

Different sequences for different player states:

```csharp
var dialogue = new DialogueBuilder("Guard");

// First encounter
dialogue.Sequence()
    .Line("Guard", "HALT! State your business!", 2f)
    .Choice("I seek passage.", 1)
    .Choice("I am the hero!", 2);

// Friendly response
dialogue.Sequence()
    .Line("Guard", "Proceed, traveler.", 2f);

// After proving yourself
dialogue.Sequence()
    .Line("Guard", "Hail, hero! Welcome back.", 2f);

var guard = dialogue.Build();

// In code, when player proves themselves:
// Call: dialogueSystem.StartDialogue(guardEntity.GetComponent<DialogueComponent>(), 2);
```

## Integration with UI (Unity Example)

In your Unity UI script:

```csharp
public class DialogueUIManager : MonoBehaviour
{
    public Text speakerNameText;
    public Text dialogueText;
    public CanvasGroup dialoguePanel;
    public LayoutGroup choicesContainer;

    void Start()
    {
        var dialogueSystem = GameManager.Instance.DialogueSystem;
        dialogueSystem.OnDisplayDialogueLine += ShowDialogueLine;
        dialogueSystem.OnDisplayChoices += ShowChoices;
        dialogueSystem.OnHideDialogue += HideDialogue;
    }

    void ShowDialogueLine(DialogueLine line)
    {
        speakerNameText.text = line.Speaker;
        dialogueText.text = line.Text;
        dialoguePanel.alpha = 1f;
    }

    void ShowChoices(List<DialogueChoice> choices, Action<int> onChoice)
    {
        for (int i = 0; i < choices.Count; i++)
        {
            var button = Instantiate(choiceButtonPrefab, choicesContainer.transform);
            button.GetComponentInChildren<Text>().text = choices[i].ChoiceText;
            int index = i;
            button.onClick.AddListener(() => onChoice(index));
        }
    }

    void HideDialogue()
    {
        dialoguePanel.alpha = 0f;
        foreach (Transform child in choicesContainer.transform)
            Destroy(child.gameObject);
    }
}
```

## Best Practices

1. **Use DialogueBuilder** for clean, readable dialogue creation
2. **Separate dialogue sequences** for different conversation branches  
3. **Limit trigger range** to prevent accidental encounters
4. **Use callbacks** for complex interactions (spawning objects, changing quest state)
5. **Audio clips** should have consistent durations matching dialogue Duration
6. **Test edge cases** like rapid clicking, dialogue overlap, or entity despawn during dialogue
