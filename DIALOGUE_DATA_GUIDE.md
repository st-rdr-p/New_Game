# Dialogue Data File Guide

This guide explains how to add and edit dialogue for your game using the `dialogue_data.json` file.

## Overview

The `dialogue_data.json` file is where you write all the dialogue for NPCs, enemies, and bosses. Each entry contains:
- **Speaker name** - Character saying the line
- **Dialogue text** - What they say (for voice acting)
- **Duration** - How long the line displays
- **Audio file** - Path to the voice-acted audio file

## File Structure

```json
{
  "entities": [
    {
      "id": "unique_identifier",
      "name": "Display Name",
      "type": "NPC|Enemy|Boss",
      "triggerRange": 10.0,
      "canRepeat": true,
      "startCombatAfter": false,
      "sequences": [...]
    }
  ]
}
```

### Top-Level Fields

| Field | Type | Description |
|-------|------|-------------|
| `entities` | Array | List of all dialogue entities |

## Entity Fields

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| `id` | String | Required | Unique identifier (e.g., "merchant_elara", "boss_shadow_king") |
| `name` | String | Required | Display name shown in game |
| `type` | String | Required | "NPC", "Enemy", or "Boss" |
| `triggerRange` | Float | 10.0 | Distance for auto-triggering encounter (in game units) |
| `canRepeat` | Boolean | true | Can dialogue be triggered multiple times? |
| `startCombatAfter` | Boolean | false | For enemies/bosses: Start combat after dialogue? |
| `sequences` | Array | Required | Dialogue sequences (conversation branches) |

## Sequences

Each entity can have multiple sequences for branching conversations.

```json
{
  "id": 0,
  "name": "greeting",
  "lines": [...],
  "choices": [...],
  "nextSequence": -1
}
```

### Sequence Fields

| Field | Type | Description |
|-------|------|-------------|
| `id` | Integer | Unique sequence ID within entity (0, 1, 2, etc.) |
| `name` | String | Descriptive name for this sequence branch |
| `lines` | Array | Dialogue lines in order |
| `choices` | Array | Player dialogue options (leave empty for auto-advance) |
| `nextSequence` | Integer | Next sequence ID to auto-play (-1 = don't auto-advance) |

## Lines

Individual dialogue lines with speaker, text, and audio.

```json
{
  "speaker": "Character Name",
  "text": "What the character says.",
  "duration": 2.5,
  "audioFile": "dialogue/character_line_01.wav"
}
```

### Line Fields

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| `speaker` | String | Required | Character name |
| `text` | String | Required | Dialogue text (what voice actor reads) |
| `duration` | Float | 0 | How long to display (seconds). 0 = wait for player input |
| `audioFile` | String | null | Path to voice audio file |

### Duration Tips

- **0** = Wait for player to press spacebar or click
- **2.5** = Display for 2.5 seconds, then auto-advance
- Use voice file length for accurate timing (if actor speaks for 3.2 seconds, use 3.2)

## Choices

Player dialogue branches.

```json
{
  "text": "What player sees and can click",
  "nextSequence": 1
}
```

### Choice Fields

| Field | Type | Description |
|-------|------|-------------|
| `text` | String | Text displayed to player as a clickable option |
| `nextSequence` | Integer | Sequence ID to play when this choice is selected |

## Quick Examples

### Simple NPC Greeting

```json
{
  "id": "guard_john",
  "name": "Guard John",
  "type": "NPC",
  "triggerRange": 8.0,
  "canRepeat": true,
  "sequences": [
    {
      "id": 0,
      "name": "greeting",
      "lines": [
        {
          "speaker": "Guard John",
          "text": "Hail and well met, traveler!",
          "duration": 2.0,
          "audioFile": "dialogue/guard_greeting.wav"
        },
        {
          "speaker": "Guard John",
          "text": "Safe travels to you.",
          "duration": 1.5,
          "audioFile": "dialogue/guard_farewell.wav"
        }
      ]
    }
  ]
}
```

### NPC with Choices

```json
{
  "id": "blacksmith_forge",
  "name": "Blacksmith Forge",
  "type": "NPC",
  "triggerRange": 10.0,
  "canRepeat": false,
  "sequences": [
    {
      "id": 0,
      "name": "intro",
      "lines": [
        {
          "speaker": "Blacksmith",
          "text": "Welcome to my forge! What brings you here?",
          "duration": 0,
          "audioFile": "dialogue/blacksmith_welcome.wav"
        }
      ],
      "choices": [
        {
          "text": "I need a better sword.",
          "nextSequence": 1
        },
        {
          "text": "Just browsing.",
          "nextSequence": 2
        }
      ]
    },
    {
      "id": 1,
      "name": "sword_quest",
      "lines": [
        {
          "speaker": "Blacksmith",
          "text": "Ah! I have just the thing. Bring me three ingots of mithril, and I'll forge you a legendary blade!",
          "duration": 0,
          "audioFile": "dialogue/blacksmith_quest.wav"
        }
      ]
    },
    {
      "id": 2,
      "name": "browsing",
      "lines": [
        {
          "speaker": "Blacksmith",
          "text": "Take your time. Quality work deserves careful inspection.",
          "duration": 0,
          "audioFile": "dialogue/blacksmith_browse.wav"
        }
      ]
    }
  ]
}
```

### Enemy Encounter

```json
{
  "id": "goblin_cave",
  "name": "Cave Goblin",
  "type": "Enemy",
  "triggerRange": 12.0,
  "canRepeat": false,
  "startCombatAfter": true,
  "sequences": [
    {
      "id": 0,
      "name": "discovery",
      "lines": [
        {
          "speaker": "Goblin",
          "text": "WHO DARES ENTER MY CAVE?!",
          "duration": 1.5,
          "audioFile": "dialogue/goblin_angry.wav"
        },
        {
          "speaker": "Goblin",
          "text": "You will regret this!",
          "duration": 1.0,
          "audioFile": "dialogue/goblin_threat.wav"
        }
      ]
    }
  ]
}
```

### Boss with Backstory

```json
{
  "id": "dragon_ancient",
  "name": "Ancient Dragon",
  "type": "Boss",
  "triggerRange": 25.0,
  "canRepeat": false,
  "startCombatAfter": true,
  "sequences": [
    {
      "id": 0,
      "name": "awakening",
      "lines": [
        {
          "speaker": "Dragon",
          "text": "So... another mortal awakens me from my slumber.",
          "duration": 3.0,
          "audioFile": "dialogue/dragon_awake.wav"
        },
        {
          "speaker": "Dragon",
          "text": "I have seen empires crumble to dust. What are you?",
          "duration": 0,
          "audioFile": "dialogue/dragon_question.wav"
        }
      ],
      "choices": [
        {
          "text": "I am your doom!",
          "nextSequence": 1
        },
        {
          "text": "Tell me your story, ancient one.",
          "nextSequence": 2
        }
      ]
    },
    {
      "id": 1,
      "name": "defiance",
      "lines": [
        {
          "speaker": "Dragon",
          "text": "Doom? HA! Your arrogance will be your undoing!",
          "duration": 2.5,
          "audioFile": "dialogue/dragon_laugh.wav"
        },
        {
          "speaker": "Dragon",
          "text": "Let me show you what real power is!",
          "duration": 2.0,
          "audioFile": "dialogue/dragon_power.wav"
        }
      ]
    },
    {
      "id": 2,
      "name": "lore",
      "lines": [
        {
          "speaker": "Dragon",
          "text": "I have watched civilizations rise and fall. I have seen mountains turn to sand.",
          "duration": 4.0,
          "audioFile": "dialogue/dragon_history.wav"
        },
        {
          "speaker": "Dragon",
          "text": "And now, you dare challenge me? Very well. Let us see if you are worthy.",
          "duration": 3.5,
          "audioFile": "dialogue/dragon_challenge.wav"
        }
      ]
    }
  ]
}
```

## Voice Acting Tips

### For Voice Actors

1. **Check the `duration` field** - It tells you how long the line should take to deliver
2. **Note the character name** - Each speaker should have a distinct voice
3. **Audio file naming** - Files are named like `dialogue/character_action_##.wav`
   - Example: `dialogue/blacksmith_quest_01.wav`

### For Writers/Designers

1. **Keep durations realistic** - If actor speaks for 2.5 seconds, set duration to 2.5
2. **Use 0 for short lines** - If unsure about timing, set duration to 0 and player controls pacing
3. **Plan audio file paths** - Create organized folders:
   ```
   Assets/Audio/dialogue/
   ├── merchant_*.wav
   ├── guard_*.wav
   ├── goblin_*.wav
   └── dragon_*.wav
   ```

## Adding a New Character

### Step 1: Create Entry in dialogue_data.json

Add a new object to the `entities` array:

```json
{
  "id": "wizard_merlin",
  "name": "Merlin, the Wise",
  "type": "NPC",
  "triggerRange": 12.0,
  "canRepeat": true,
  "sequences": [
    {
      "id": 0,
      "name": "greeting",
      "lines": [
        {
          "speaker": "Merlin",
          "text": "ADD YOUR DIALOGUE HERE",
          "duration": 2.5,
          "audioFile": "dialogue/merlin_greeting_01.wav"
        }
      ]
    }
  ]
}
```

### Step 2: Write Dialogue Lines

Fill in the `text` fields with what the character should say. This is what the voice actor will read.

### Step 3: Record Voice Acting

Have the voice actor record each line and save as `.wav` files with the names specified in `audioFile`.

### Step 4: Place Audio Files

Put the `.wav` files in your Assets/Audio/dialogue/ folder (or wherever you reference them in the JSON).

### Step 5: Update Duration

Measure or import the audio files and set `duration` to match the audio length.

### Step 6: Load in Game

In your game code:

```csharp
var loader = new DialogueDataLoader(audioSystem);
var dialogueEntities = loader.LoadFromFile("src/GameCore/dialogue_data.json");
foreach (var entity in dialogueEntities)
{
    game.AddEntity(entity);
}
```

## Advanced: Branching Conversations

Create multi-sequence dialogues for complex interactions:

```json
{
  "id": "quest_giver",
  "sequences": [
    {
      "id": 0,
      "name": "initial",
      "lines": [...],
      "choices": [
        {"text": "Accept quest", "nextSequence": 1},
        {"text": "Decline", "nextSequence": 2}
      ]
    },
    {
      "id": 1,
      "name": "quest_accepted",
      "lines": [
        {
          "speaker": "Quest Giver",
          "text": "Thank you! Go forth and complete the task!",
          "duration": 0,
          "audioFile": "dialogue/qg_thanks.wav"
        }
      ]
    },
    {
      "id": 2,
      "name": "quest_declined",
      "lines": [
        {
          "speaker": "Quest Giver",
          "text": "Perhaps you'll reconsider later.",
          "duration": 0,
          "audioFile": "dialogue/qg_sad.wav"
        }
      ]
    }
  ]
}
```

## Troubleshooting

### Audio file not playing?
- Check the file path is correct
- Ensure file exists at that location
- Verify audio format is `.wav`

### Dialogue not triggering?
- Check `triggerRange` is large enough
- Verify entity type is correct ("NPC", "Enemy", "Boss")
- Make sure DialogueDataLoader is loading the file

### Duration seems wrong?
- Open `.wav` file in audio editor to check actual length
- Add 0.5 seconds buffer for character pauses
- Test in-game and adjust as needed

## Template

Save this as a template for new characters:

```json
{
  "id": "character_id",
  "name": "Character Name",
  "type": "NPC",
  "triggerRange": 10.0,
  "canRepeat": true,
  "startCombatAfter": false,
  "sequences": [
    {
      "id": 0,
      "name": "sequence_name",
      "lines": [
        {
          "speaker": "Character Name",
          "text": "Add dialogue here",
          "duration": 0,
          "audioFile": "dialogue/character_line_01.wav"
        }
      ],
      "choices": [],
      "nextSequence": -1
    }
  ]
}
```

---

**Ready to write dialogue?** Edit `dialogue_data.json` and add your characters and their lines!
