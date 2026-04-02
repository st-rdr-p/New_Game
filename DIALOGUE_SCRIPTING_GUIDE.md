# Dialogue Scripting Guide

Write dialogue in simple script format instead of C# code. Perfect for lore writers!

## Basic Format

```
SEQ main
  NPC: OldSage
    "Welcome, traveler!"
    "I have been expecting you."
    
  CHOICE
    [I'll help] -> SEQUENCE: accept
    [Not now] -> SEQUENCE: reject
```

## Lines with Audio

Add voice lines with parentheses:

```
NPC: OldSage
  "Welcome!" (voice_welcome.mp3, 2.5)
  "I seek your aid." (voice_help.mp3)
```

Format: `(audio_file.mp3, duration_seconds)`
- Duration is optional (auto-calculated from text)
- Duration defaults to ~0.06 seconds per character if not specified

## Multiple Sequences

Branch dialogue into different paths:

```
SEQ main
  NPC: OldSage
    "Will you help me?"
    
  CHOICE
    [Yes] -> SEQUENCE: accept_quest
    [No] -> SEQUENCE: reject_quest

SEQ accept_quest
  NPC: OldSage
    "Excellent! Go north to the mountain."
    "Bring me the crystal."

SEQ reject_quest
  NPC: OldSage
    "Perhaps another time..."
```

## Full Example: Quest NPC

```
SEQ first_meeting
  NPC: OldSage
    "Greetings, adventurer." (sage_greeting.mp3, 2.0)
    "I am the keeper of ancient knowledge."
    
  CHOICE
    [Tell me more] -> SEQUENCE: lore_dump
    [I'm in a hurry] -> SEQUENCE: rush

SEQ lore_dump
  NPC: OldSage
    "Long ago, darkness fell upon our land." (sage_lore.mp3, 4.0)
    "Only the Crystal of Light can save us."
    
  CHOICE
    [I'll find it] -> SEQUENCE: quest_start
    [Sounds hard] -> SEQUENCE: reject

SEQ quest_start
  NPC: OldSage
    "Thank you! Head to the mountain peak." (sage_thanks.mp3, 2.5)
    "Return when you have it."
    
SEQ reject
  NPC: OldSage
    "I understand. Seek me when you're ready." (sage_reject.mp3, 2.0)

SEQ rush
  NPC: OldSage
    "Ah, I see. Come back later." (sage_busy.mp3, 1.5)
```

## Loading and Using Scripts

### Load from string:

```csharp
string dialogueScript = @"
SEQ main
  NPC: OldSage
    ""Hello!""
    
  CHOICE
    [Hi] -> SEQUENCE: response1
    [Hello] -> SEQUENCE: response2

SEQ response1
  NPC: OldSage
    ""Nice to meet you!""
";

var sequences = DialogueScriptParser.ParseDialogueFile(dialogueScript);
var npc = new Entity(1);
npc.AddComponent(new DialogueComponent { Sequences = sequences });
```

### Load from file:

```csharp
string scriptContent = File.ReadAllText("Assets/Dialogue/sage_quests.txt");
var sequences = DialogueScriptParser.ParseDialogueFile(scriptContent);

var npc = new Entity(1);
npc.AddComponent(new DialogueComponent { Sequences = sequences });
```

### Using with EncounterTrigger:

```csharp
var sequences = DialogueScriptParser.ParseDialogueFile(scriptContent);

var npc = new Entity(1);
npc.AddComponent(new Transform3D { Position = new Vector3(10, 0, 0) });
npc.AddComponent(new DialogueComponent { Sequences = sequences });

var encounter = new EncounterTrigger(EncounterTrigger.EncounterType.NPC, 
    new DialogueComponent { Sequences = sequences })
{
    TriggerRange = 5f,
    CanTriggerMultipleTimes = true
};
npc.AddComponent(encounter);
```

## Advanced: Enemy/Boss Dialogue

```
SEQ boss_encounter
  NPC: DarkLord
    "So! A worthy opponent arrives!" (boss_challenge.mp3, 2.5)
    "Let me show you true power!"
    
SEQ boss_defeated
  NPC: DarkLord
    "Impossible! I... have been bested..." (boss_defeat.mp3, 3.0)
    "Take the artifact... it is yours now..."
```

Then trigger with combat:

```csharp
var sequences = DialogueScriptParser.ParseDialogueFile(bossScript);
var boss = new Entity(boss_id);
boss.AddComponent(new DialogueComponent { Sequences = sequences });

var encounter = new EncounterTrigger(..., new DialogueComponent { Sequences = sequences })
{
    StartCombatAfter = true
};
encounter.OnEncounterEnd += () => 
{
    // Trigger defeat dialogue after combat
    boss.GetComponent<DialogueComponent>().CurrentSequenceKey = "boss_defeated";
};
```

## Tips for Lore Writing

1. **Create one file per NPC or Quest**
   - `Assets/Dialogue/sage_quests.txt`
   - `Assets/Dialogue/goblin_encounters.txt`
   - `Assets/Dialogue/boss_final.txt`

2. **Use meaningful sequence names**
   - `[Good] -> SEQUENCE: happy_path`
   - `[Evil] -> SEQUENCE: dark_path`
   - `[Confused] -> SEQUENCE: question_more`

3. **Voice acting file naming**
   - `voice_npc_action_variant.mp3`
   - `voice_sage_greeting_warm.mp3`
   - `voice_sage_greeting_stern.mp3`

4. **Keep lines conversational**
   - Write like dialogue, not narration
   - Break long thoughts into multiple lines
   - Use pauses with separate lines

5. **Track player reputation**
   - Branch paths based on previous choices
   - Reference dialogue states in your script
   - Build character relationships through conversation

## File Structure

Organize dialogue files:

```
Assets/
├── Dialogue/
│   ├── NPCs/
│   │   ├── sage.txt
│   │   ├── blacksmith.txt
│   │   └── innkeeper.txt
│   ├── Encounters/
│   │   ├── goblin_first.txt
│   │   ├── goblin_boss.txt
│   │   └── dragon_final.txt
│   └── Quests/
│       ├── crystal_quest.txt
│       └── revenge_quest.txt
└── Audio/
    └── Dialogue/
        ├── voice_sage_*.mp3
        ├── voice_goblin_*.mp3
        └── voice_dragon_*.mp3
```

## Example: Complete Game Scene

```csharp
// Load all dialogues
var sageScript = File.ReadAllText("Assets/Dialogue/NPCs/sage.txt");
var goblinScript = File.ReadAllText("Assets/Dialogue/Encounters/goblin_first.txt");

var sageSequences = DialogueScriptParser.ParseDialogueFile(sageScript);
var goblinSequences = DialogueScriptParser.ParseDialogueFile(goblinScript);

// Create NPCs
var sage = new Entity(1);
sage.AddComponent(new Transform3D { Position = new Vector3(0, 0, 0) });
sage.AddComponent(new DialogueComponent { Sequences = sageSequences });
game.AddEntity(sage);

var goblin = new Entity(2);
goblin.AddComponent(new Transform3D { Position = new Vector3(20, 0, 0) });
goblin.AddComponent(new Health(20f));
goblin.AddComponent(new DialogueComponent { Sequences = goblinSequences });
game.AddEntity(goblin);
```

---

**Benefits:**
- ✅ Non-programmers can write dialogue
- ✅ Easy to version control (text files)
- ✅ Simple branch management
- ✅ Clean audio integration
- ✅ Scale to hundreds of lines
