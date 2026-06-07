using System;
using System.Collections.Generic;
using Hut.SO;

namespace Hut.SelectedItems
{
    public class SelectedItemsRuntime
    {
        private readonly List<string> _selectedItemsIds = new();

        public event Action SelectionChanged;

        public int MinimumSelectedItemsCount { get; }
        public int MaxSelectedItemsCount { get; }
        public IReadOnlyList<string> SelectedItemsIds => _selectedItemsIds;
        public int SelectedItemsCount => _selectedItemsIds.Count;
        public bool HasRequiredSelectedItems => _selectedItemsIds.Count >= MinimumSelectedItemsCount;

        public SelectedItemsRuntime(HutSettings hutSettings)
        {
            MinimumSelectedItemsCount = Math.Max(0, hutSettings?.MacroSettings?.MinimumSelectedItemsCount ?? 1);
            MaxSelectedItemsCount = Math.Max(0, hutSettings?.MacroSettings?.InitialSelectedItemsCount ?? 0);
        }

        public bool TrySelect(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                return false;

            if (_selectedItemsIds.Contains(itemId))
                return true;

            if (IsSelectionLimitReached)
                return false;

            _selectedItemsIds.Add(itemId);
            SelectionChanged?.Invoke();
            return true;
        }

        public bool Unselect(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                return false;

            if (!_selectedItemsIds.Remove(itemId))
                return false;

            SelectionChanged?.Invoke();
            return true;
        }

        public bool SetSelected(string itemId, bool isSelected)
        {
            if (isSelected)
                return TrySelect(itemId);

            Unselect(itemId);
            return true;
        }

        public bool IsSelected(string itemId)
        {
            return !string.IsNullOrWhiteSpace(itemId) && _selectedItemsIds.Contains(itemId);
        }

        public string[] GetSelectedItemsIds()
        {
            return _selectedItemsIds.ToArray();
        }

        public void Clear()
        {
            if (_selectedItemsIds.Count == 0)
                return;

            _selectedItemsIds.Clear();
            SelectionChanged?.Invoke();
        }

        private bool IsSelectionLimitReached => MaxSelectedItemsCount > 0 && _selectedItemsIds.Count >= MaxSelectedItemsCount;
    }
}
