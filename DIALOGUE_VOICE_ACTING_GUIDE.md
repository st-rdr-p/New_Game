# Dialogue Writing & Voice Acting Workflow

This document explains how to write dialogue for characters and prepare it for voice actors.

## Quick Start

1. **Open** `src/GameCore/dialogue_data.json`
2. **Edit the JSON** with your character dialogue
3. **Save the file**
4. **Share with voice actors** - They read the `text` fields
5. **Record voice acting** - Save as `.wav` files
6. **Add audio file paths** - Update `audioFile` fields to point to recordings
7. **Done!** - Game loads everything automatically

## The Workflow

### Step 1: Write Dialogue

Edit `src/GameCore/dialogue_data.json` and add your characters and lines:

```json
{
  "id": "your_character",
  "name": "Character Name",
  "type": "NPC",
  "sequences": [
    {
      "id": 0,
      "lines": [
        {
          "speaker": "Character Name",
          "text": "Hello, this is my first line!",
          "duration": 0,
          "audioFile": ""
        }
      ]
    }
  ]
}
```

**Write all the dialogue first before recording.**

### Step 2: Prepare for Voice Actors

1. Export the dialogue as a script document for voice actors to read
2. Include:
   - Character names
   - All dialogue text
   - Scene context
   - Emotional direction (angry, calm, excited, etc.)

Example script for your voice actor:
```
CHARACTER: Guard John
Line 1: "Hail and well met, traveler!" (friendly, welcoming)
Line 2: "Safe travels to you." (warm farewell)

CHARACTER: Shadow King
Line 1: "At last... a worthy opponent approaches!" (ominous, menacing)
Line 2: "I will crush you!" (aggressive, confident)
```

### Step 3: Voice Actors Record

They record each line and provide you with `.wav` files named to match your JSON entries:
- `dialogue/merchant_greeting_01.wav`
- `dialogue/guard_farewell_01.wav`
- `dialogue/goblin_angry_01.wav`
- etc.

### Step 4: Update Audio File Paths

Once you have the `.wav` files, update the `audioFile` fields in the JSON:

```json
{
  "text": "Hello, this is my first line!",
  "duration": 2.5,
  "audioFile": "dialogue/character_line_01.wav"
}
```

### Step 5: Timing

Measure the voice file duration and set the `duration` field:
- Open `.wav` in Audacity or similar
- Note the length (e.g., 2.45 seconds)
- Update `duration`: 2.45

**Tip:** If unsure about timing, set `duration` to 0 and the player will control pacing with spacebar.

## File Organization

Create organized folders for audio:

```
Assets/Audio/
├── dialogue/
│   ├── merchant_*.wav
│   ├── guard_*.wav
│   ├── goblin_*.wav
│   ├── dragon_*.wav
│   └── sage_*.wav
├── music/
└── sfx/
```

## JSON Fields Reference

| Field | Purpose | Example |
|-------|---------|---------|
| `id` | Unique identifier | "wizard_merlin" |
| `name` | Display name in game | "Merlin, the Wise" |
| `type` | Character type | "NPC" \| "Enemy" \| "Boss" |
| `speaker` | Character speaking | "Merlin" |
| `text` | Dialogue to be voice acted | "Welcome, young one" |
| `duration` | Audio length in seconds | 2.5 |
| `audioFile` | Path to voice file | "dialogue/merlin_line_01.wav" |

## Using the Template

A blank template is available at `dialogue_data_template.json`:

1. Copy and rename it: `cp dialogue_data_template.json my_characters.json`
2. Add your character data
3. Load in game using DialogueDataLoader

## Loading Dialogue in Game

In your game code:

```csharp
var loader = new DialogueDataLoader(audioSystem);
var entities = loader.LoadFromFile("src/GameCore/dialogue_data.json");
foreach (var entity in entities)
{
    game.AddEntity(entity);
}
```

See `DialogueLoadingExample.cs` for complete examples.

## Tips for Voice Actors

### Recording Best Practices

1. **Read the context** - Understand the emotional tone of each line
2. **Watch timing** - Some lines have specific duration requirements
3. **Character consistency** - Keep the same voice/tone for all lines from that character
4. **Pause before/after** - Don't include lead-in silence
5. **File naming** - Use exact names specified in JSON (e.g., `merchant_greeting_01.wav`)

### Common Issues

❌ **Wrong character voice** - Use consistent voice for each character across all lines
❌ **Long pauses** - Trim silence from beginning and end of recordings  
❌ **Mismatched duration** - If file is 3 seconds, duration should be 3.0 (or close)
❌ **Wrong file name** - Name must exactly match the `audioFile` field in JSON

### File Format

- **Format:** WAV (PCM, uncompressed)
- **Sample rate:** 44.1 kHz or 48 kHz
- **Bit depth:** 16-bit stereo or mono
- **Quality:** CD quality or better

## Example: Complete Character Setup

### The JSON File Entry:
```json
{
  "id": "innkeeper_alice",
  "name": "Alice, the Innkeeper",
  "type": "NPC",
  "triggerRange": 8.0,
  "canRepeat": true,
  "sequences": [
    {
      "id": 0,
      "name": "greeting",
      "lines": [
        {
          "speaker": "Alice",
          "text": "Welcome to the Sleeping Dragon Inn!",
          "duration": 0,
          "audioFile": "dialogue/alice_greeting_01.wav"
        },
        {
          "speaker": "Alice",
          "text": "Can I get you a room or perhaps something to eat?",
          "duration": 0,
          "audioFile": "dialogue/alice_greeting_02.wav"
        }
      ]
    }
  ]
}
```

### The Voice Actor Script:
```
CHARACTER: Alice, the Innkeeper
TONE: Cheerful, welcoming

Line 1: "Welcome to the Sleeping Dragon Inn!"
Line 2: "Can I get you a room or perhaps something to eat?"
```

### After Recording:
Place these files:
- `Assets/Audio/dialogue/alice_greeting_01.wav` (2.1 seconds)
- `Assets/Audio/dialogue/alice_greeting_02.wav` (2.8 seconds)

### Update the JSON:
```json
{
  "speaker": "Alice",
  "text": "Welcome to the Sleeping Dragon Inn!",
  "duration": 2.1,
  "audioFile": "dialogue/alice_greeting_01.wav"
},
{
  "speaker": "Alice",
  "text": "Can I get you a room or perhaps something to eat?",
  "duration": 2.8,
  "audioFile": "dialogue/alice_greeting_02.wav"
}
```

## Multi-Character Support

Each entity has its own character:

```json
{
  "entities": [
    {
      "id": "npc_merlin",
      "name": "Merlin",
      "sequences": [{...}]
    },
    {
      "id": "npc_alice",
      "name": "Alice",
      "sequences": [{...}]
    },
    {
      "id": "boss_dragon",
      "name": "Ancient Dragon",
      "sequences": [{...}]
    }
  ]
}
```

Each character gets their own voice actor or can share if they have similar voices.

## Branching Dialogue with Multiple Voice Actors

If dialogue has branches (choices), you can have different voice actors per branch:

```json
{
  "sequences": [
    {
      "id": 0,
      "lines": [
        {
          "speaker": "Merchant",
          "text": "What would you like to buy?",
          "audioFile": "dialogue/merchant_question.wav"
        }
      ],
      "choices": [
        {"text": "Potions", "nextSequence": 1},
        {"text": "Weapons", "nextSequence": 2}
      ]
    },
    {
      "id": 1,
      "lines": [
        {
          "speaker": "Merchant",
          "text": "Here are my finest potions!",
          "audioFile": "dialogue/merchant_potions.wav"
        }
      ]
    },
    {
      "id": 2,
      "lines": [
        {
          "speaker": "Merchant",
          "text": "My best swords and shields right here!",
          "audioFile": "dialogue/merchant_weapons.wav"
        }
      ]
    }
  ]
}
```

Same voice actor records all three sections (or assign to different actors for variety).

## Troubleshooting

### "Dialogue not appearing in game?"
- ✓ Verify JSON is valid (use JSONLint to check)
- ✓ Check file path is correct in `audioFile`
- ✓ Ensure `type` is "NPC", "Enemy", or "Boss"
- ✓ Make sure game is loading the correct JSON file

### "Audio not playing?"
- ✓ File exists at the path specified
- ✓ File format is `.wav`
- ✓ Duration doesn't match? Adjust duration to match audio length
- ✓ Check audio file permissions

### "Dialogue doesn't sound right?"
- ✓ Check character emotion matches the context
- ✓ Verify audio quality (not clipped or distorted)
- ✓ Confirm actor is using consistent voice for continuity

## Tools Recommended

- **Audio Editing:** Audacity (free, open-source)
- **JSON Validation:** jsonlint.com
- **Voice Recording:** Audacity, OBS, or professional recording software
- **Text Editor:** VS Code (supports JSON)

---

**Ready to add voices to your game? Start writing dialogue in `dialogue_data.json`!**
