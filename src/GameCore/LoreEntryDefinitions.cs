using System;

namespace GameCore
{
    /// <summary>
    /// Initializes all lore entries for the game.
    /// Add new lore entries here as you create them.
    /// </summary>
    public static class LoreEntryDefinitions
    {
        public static LoreDatabase Initialize()
        {
            var database = new LoreDatabase();

            // Entry 001: DREAD
            var dreadEntry = new LoreEntry
            {
                Id = "DREAD_001",
                EntryNumber = 1,
                Title = "DREAD",
                Zone = "Zone 1 · The Store",
                Content = "There is this one disc though… Bright red. No label on its fully transparent case. No writing on the case or disc. I reach for it and then something calls my name. When I look back — it's on the ceiling. The owner tells me to just take it. 'No one's gonna buy it.' I pass. I get to my car. The disc is on the passenger seat.",
                VoiceScriptFile = "voice_scripts/lore_dread_001.txt",
                IsCorrupted = true,
                CorruptionNote = "3 LINES MISSING",
                DesignNote = "This entry drops when the player first enters the GameStop-style hub level. The disc item exists physically in the world here — players can see it but cannot yet pick it up. Once this log is collected, the disc becomes a persistent inventory item that cannot be dropped. Frame this entry as a corrupted .txt file read through an in-world terminal.",
                TriggerType = "On entering Zone 1 hub room"
            };

            database.RegisterEntry(dreadEntry);

            // Entry 002: FEAR
            var fearEntry = new LoreEntry
            {
                Id = "FEAR_002",
                EntryNumber = 2,
                Title = "FEAR",
                Zone = "Zone 2 · The Highway",
                Content = "A car on the road in front of me — its tyre is shredded, flinging sparks. But it drives pin straight. Like something's holding the wheel. I pass it. The moment I do, it loses control. Like I cut the strings. Like it only needed to be in front of me. Like it was there to make sure I noticed.\n\nI noticed.",
                VoiceScriptFile = "voice_scripts/lore_fear_002.txt",
                IsCorrupted = true,
                CorruptionNote = "AUDIO NOTE EMBEDDED",
                DesignNote = "Unlocks mid-chase sequence where environmental hazards (vehicles, barriers) begin behaving with impossible accuracy — referencing the Entity's WorldManipulation ability. This entry should play as an audio log over ambient static rather than text, if possible. The player has no way to destroy these hazards — they can only dodge.",
                TriggerType = "First environmental hazard sequence begins"
            };

            database.RegisterEntry(fearEntry);

            // Entry 003: HORROR
            var horrorEntry = new LoreEntry
            {
                Id = "HORROR_003",
                EntryNumber = 3,
                Title = "HORROR",
                Zone = "Zone 3 · The House",
                Content = "There was something in the attic. It looked like a fox — or a dog — something without a tail. I swung at it and it disappeared in a cloud of red. Not blood. Not ash. It just — wasn't there.\n\nThen something yanked me to the floor. Nothing touched me. Nothing was there.\n\nThen I found the garage. She called my name from the garage. But her voice was wrong — like a human sound played through a machine. I grabbed the bat.",
                VoiceScriptFile = "voice_scripts/lore_horror_003.txt",
                IsCorrupted = true,
                CorruptionNote = "AUTHOR REQUEST — REDACTED SECTION",
                DesignNote = "This is when players first encounter a Shapeshifter enemy — the sub-unit creature the Entity sends ahead of itself. Blue blood on kill + a red ring drop connects this enemy to the soul ring system. The [REDACTED] block covers the fact that the Shapeshifter was mimicking someone the player knew — a reveal saved for a later log. The bat is the player's first melee weapon unlock.",
                TriggerType = "First Shapeshifter enemy defeated"
            };

            database.RegisterEntry(horrorEntry);

            // Entry 004: FREEFALL
            var freefallEntry = new LoreEntry
            {
                Id = "FREEFALL_004",
                EntryNumber = 4,
                Title = "FREEFALL",
                Zone = "Zone 4 · The Road",
                Content = "My brake cord was cut. At 120 miles an hour. I blew a red light. The back of the car got clipped. I tumbled down a hill. I don't know how long it took.\n\nHere is what I know: the disc was still in the cupholder when I crawled out. The car was on fire. Then the fire just — stopped. Like something put a cup over it.",
                VoiceScriptFile = "voice_scripts/lore_freefall_004.txt",
                IsCorrupted = true,
                CorruptionNote = "CHARACTER ENCODING LOST",
                DesignNote = "This is the midpoint revelation — the Entity is holding back. It should reframe the player's understanding of every prior encounter as deliberate restraint. In gameplay terms: after this log, the Entity's encounter AI increases in aggression (patrol range expands, reaction time tightens) but it still will not land killing blows in standard fights. Only in the boss arena does it go lethal. The disc item gains a subtle red pulse animation after this entry.",
                TriggerType = "Midpoint of Zone 4"
            };

            database.RegisterEntry(freefallEntry);

            // Entry 005: KARMA
            var karmaEntry = new LoreEntry
            {
                Id = "KARMA_005",
                EntryNumber = 5,
                Title = "KARMA",
                Zone = "Zone 5 · The Forest",
                Content = "It left things for me. A television. A CD player. The disc already inside it. A note tied to the screen in symbols I can't read. I found an axe buried in a tree. The handle was new. The blade was damaged.\n\nIt's making a puzzle out of this. It left me pieces.",
                VoiceScriptFile = "voice_scripts/lore_karma_005.txt",
                IsCorrupted = false,
                CorruptionNote = "NOTE: Author has handwritten in the margin — 'It's not a puzzle. It's a stage.'",
                DesignNote = "The \"TV + CD player + disc\" objects from this log should be placeable in the world as collectible set pieces — environmental storytelling that rewards thorough exploration. The axe is the player's second melee weapon. The margin note can appear as a handwritten overlay on the entry UI — a different font, different ink color — implying the document passed through multiple hands before the player found it.",
                TriggerType = "All three set-piece objects found in Zone 5"
            };

            database.RegisterEntry(karmaEntry);

            // Entry 006: BUILD-UP
            var buildupEntry = new LoreEntry
            {
                Id = "BUILDUP_006",
                EntryNumber = 6,
                Title = "BUILD-UP",
                Zone = "Zone 5 · The Forest — Deep",
                Content = "I finally saw it.\n\nPixelated. Blue. Straight pupils instead of circles — pupils that end at a point. It spoke. Its voice was autotuned down to something inhuman. It said: 'You should really watch what you say.'\n\nIt knew my name before I told it. It taunted me. It let me hit it. I think it was enjoying it.",
                VoiceScriptFile = "voice_scripts/lore_buildup_006.txt",
                IsCorrupted = true,
                CorruptionNote = "AUDIO SPIKE — 4 SECONDS OF SCREAMING, THEN SILENCE",
                DesignNote = "First full visual reveal of The Entity. This log unlocks the Bestiary entry for The Entity (see Codex below). The pixelated visual distortion effect should begin applying to the Entity's model from this point forward — use the existing RetroGraphicsEffect component (SegaGenesis preset) on the Entity's renderer only, not the full scene. This makes it visually distinct from the world around it.",
                TriggerType = "First direct Entity sighting"
            };

            database.RegisterEntry(buildupEntry);

            // Entry 007: TRANSPORT
            var transportEntry = new LoreEntry
            {
                Id = "TRANSPORT_007",
                EntryNumber = 7,
                Title = "TRANSPORT",
                Zone = "Zone 6 · The Portal",
                Content = "It told me the ring I'd been carrying wasn't mine. It said I killed her. I said I hadn't. It said: 'Oh but you did. You beat her senseless with a bat. I watched.'\n\nThe shapeshifter. The one in my garage. That was—",
                VoiceScriptFile = "voice_scripts/lore_transport_007.txt",
                IsCorrupted = true,
                CorruptionNote = "FILE ENDS — ABRUPT TERMINATION",
                DesignNote = "The abrupt end of this log is intentional — it cuts at the exact moment of Kale's worst realization. Do not complete the sentence. The player supplies the rest. This entry should play immediately before the Zone 6 boss portal opens, giving the player a beat of emotional weight before the hardest fight. The ring item in inventory should change appearance here — from red to a dimmer, darker tone — and gain the tooltip: \"Someone else's soul.\"",
                TriggerType = "Boss portal room entered"
            };

            database.RegisterEntry(transportEntry);

            // Entry 008: GONE (Final Entry / Credits Sequence)
            var goneEntry = new LoreEntry
            {
                Id = "GONE_008",
                EntryNumber = 8,
                Title = "GONE",
                Zone = "Post-Boss · Credits Sequence",
                Content = "'You're gonna get your soul ring ripped right out of you.'\n\nMy soul ring. That's what I'd been carrying. All those monsters — they had souls. The disc was a distraction. The ring was always the point.\n\n'Any last words?'\n\nFuck you.\n\n— K.",
                VoiceScriptFile = "voice_scripts/lore_gone_008.txt",
                IsCorrupted = false,
                CorruptionNote = "",
                DesignNote = "This log plays during the credits — not before them. It functions as a final piece of context for anyone replaying or who missed earlier logs. Kale's story ends here. The player's story — everyone else who finds the disc — continues. Consider an end-screen that reads: \"The disc is still out there.\" A new game+ that starts with the disc already in inventory, no tutorial, Entity aggression at maximum.",
                TriggerType = "Credits sequence begins"
            };

            database.RegisterEntry(goneEntry);

            return database;
        }

        public static LoreEntry CreateEntry(int number, string title, string zone, string content, 
            bool isCorrupted = false, string corruptionNote = "", string voiceScriptFile = "", 
            string designNote = "", string triggerType = "")
        {
            return new LoreEntry
            {
                Id = $"{title.ToUpper().Replace(" ", "_")}_{number:D3}",
                EntryNumber = number,
                Title = title,
                Zone = zone,
                Content = content,
                VoiceScriptFile = voiceScriptFile,
                IsCorrupted = isCorrupted,
                CorruptionNote = corruptionNote,
                DesignNote = designNote,
                TriggerType = triggerType
            };
        }
    }
}
