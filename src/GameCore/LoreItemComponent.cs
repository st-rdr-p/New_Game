using System;

namespace GameCore
{
    /// <summary>
    /// Component for a collectible lore item in the world.
    /// When the player picks this up, it triggers the LoreDisplayScreen.
    /// </summary>
    public class LoreItemComponent : Component
    {
        public string LoreEntryId { get; set; } = "";
        public Action<LoreEntry>? OnLoreCollected { get; set; }
        public Action<LoreEntry>? OnLoreDisplayed { get; set; }

        private LoreEntry? _loreEntry;
        private bool _hasBeenCollected;

        public LoreItemComponent(string loreEntryId)
        {
            LoreEntryId = loreEntryId;
            _hasBeenCollected = false;
        }

        public void SetLoreEntry(LoreEntry entry)
        {
            _loreEntry = entry;
        }

        public LoreEntry GetLoreEntry()
        {
            return _loreEntry;
        }

        public void Collect()
        {
            if (_hasBeenCollected || _loreEntry == null)
                return;

            _hasBeenCollected = true;
            _loreEntry.IsCollected = true;
            _loreEntry.DateCollected = DateTime.Now;

            OnLoreCollected?.Invoke(_loreEntry);
            OnLoreDisplayed?.Invoke(_loreEntry);
        }

        public bool IsCollected()
        {
            return _hasBeenCollected;
        }
    }
}
