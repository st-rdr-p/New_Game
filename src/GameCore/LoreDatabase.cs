using System;
using System.Collections.Generic;
using System.Linq;

namespace GameCore
{
    /// <summary>
    /// Manages all lore entries in the game.
    /// Tracks which entries have been collected, provides lookup and filtering.
    /// </summary>
    public class LoreDatabase
    {
        private Dictionary<string, LoreEntry> _entries;
        private HashSet<string> _collectedIds;
        public Action<LoreEntry> OnEntryCollected { get; set; }

        public LoreDatabase()
        {
            _entries = new Dictionary<string, LoreEntry>();
            _collectedIds = new HashSet<string>();
        }

        public void RegisterEntry(LoreEntry entry)
        {
            if (entry == null || string.IsNullOrEmpty(entry.Id))
                return;

            _entries[entry.Id] = entry;
        }

        public void RegisterEntries(params LoreEntry[] entries)
        {
            foreach (var entry in entries)
            {
                RegisterEntry(entry);
            }
        }

        public LoreEntry GetEntry(string id)
        {
            if (_entries.TryGetValue(id, out var entry))
                return entry;
            return null;
        }

        public void CollectEntry(string id)
        {
            if (_entries.TryGetValue(id, out var entry))
            {
                _collectedIds.Add(id);
                entry.IsCollected = true;
                entry.DateCollected = DateTime.Now;
                OnEntryCollected?.Invoke(entry);
            }
        }

        public bool IsEntryCollected(string id)
        {
            return _collectedIds.Contains(id);
        }

        public int GetCollectedCount()
        {
            return _collectedIds.Count;
        }

        public int GetTotalCount()
        {
            return _entries.Count;
        }

        public List<LoreEntry> GetAllEntries()
        {
            return _entries.Values.ToList();
        }

        public List<LoreEntry> GetCollectedEntries()
        {
            return _entries.Values
                .Where(e => _collectedIds.Contains(e.Id))
                .OrderBy(e => e.EntryNumber)
                .ToList();
        }

        public List<LoreEntry> GetEntriesByZone(string zone)
        {
            return _entries.Values
                .Where(e => e.Zone == zone)
                .OrderBy(e => e.EntryNumber)
                .ToList();
        }

        public List<LoreEntry> GetUncollectedEntries()
        {
            return _entries.Values
                .Where(e => !_collectedIds.Contains(e.Id))
                .OrderBy(e => e.EntryNumber)
                .ToList();
        }

        public void ClearCollections()
        {
            _collectedIds.Clear();
            foreach (var entry in _entries.Values)
            {
                entry.IsCollected = false;
                entry.DateCollected = null;
            }
        }

        public string GetStatistics()
        {
            return $"Lore Entries: {GetCollectedCount()}/{GetTotalCount()} collected";
        }
    }
}
