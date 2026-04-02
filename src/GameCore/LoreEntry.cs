using System;

namespace GameCore
{
    /// <summary>
    /// Represents a single lore entry that can be collected and displayed to the player.
    /// These are typically found as items in the world or triggered by events.
    /// </summary>
    public class LoreEntry
    {
        public string Id { get; set; } = "";
        public int EntryNumber { get; set; }
        public string Title { get; set; } = "";
        public string Zone { get; set; } = "";
        public string Content { get; set; } = "";
        public string? VoiceScriptFile { get; set; }
        public bool IsCorrupted { get; set; }
        public string? CorruptionNote { get; set; }
        public string? DesignNote { get; set; }
        public string? TriggerType { get; set; }
        public bool IsCollected { get; set; }           // Whether player has found this yet
        public DateTime? DateCollected { get; set; }    // When player collected it

        public LoreEntry()
        {
            IsCorrupted = false;
            IsCollected = false;
        }

        public string GetDisplayTitle()
        {
            return $"{EntryNumber:D2}\n{Title}";
        }

        public string GetDisplayHeader()
        {
            return $"{Zone}";
        }

        public string GetDisplayContent()
        {
            if (IsCorrupted && !string.IsNullOrEmpty(CorruptionNote))
            {
                return $"{Content}\n\n[ FILE CORRUPTION — {CorruptionNote} ]";
            }
            return Content;
        }
    }
}
