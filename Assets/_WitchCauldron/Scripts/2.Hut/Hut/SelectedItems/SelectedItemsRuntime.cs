using System;
using System.Collections.Generic;
using Core.Run;
using Hut.SO;

namespace Hut.SelectedItems
{
    public class SelectedItemsRuntime : IDisposable
    {
        private readonly List<string> _selectedItemsIds = new();
        private readonly int _initialMaxSelectedItemsCount;
        private readonly RunState _runState;

        public event Action SelectionChanged;
        public event Action SelectionLimitChanged;

        public int MinimumSelectedItemsCount { get; }
        public int MaxSelectedItemsCount { get; private set; }
        public IReadOnlyList<string> SelectedItemsIds => _selectedItemsIds;
        public int SelectedItemsCount => _selectedItemsIds.Count;
        public bool HasRequiredSelectedItems => _selectedItemsIds.Count >= MinimumSelectedItemsCount;

        public SelectedItemsRuntime(HutSettings hutSettings, RunState runState)
        {
            _runState = runState;
            MinimumSelectedItemsCount = Math.Max(0, hutSettings?.MacroSettings?.MinimumSelectedItemsCount ?? 1);
            _initialMaxSelectedItemsCount = Math.Max(0, hutSettings?.MacroSettings?.InitialSelectedItemsCount ?? 0);
            RefreshMaxSelectedItemsCount();

            if (_runState != null)
                _runState.SelectedItemsCapacity.AdditionalCapacityChanged += OnAdditionalCapacityChanged;
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

        public void Dispose()
        {
            if (_runState != null)
                _runState.SelectedItemsCapacity.AdditionalCapacityChanged -= OnAdditionalCapacityChanged;
        }

        private void OnAdditionalCapacityChanged(int _)
        {
            RefreshMaxSelectedItemsCount();
            SelectionLimitChanged?.Invoke();
        }

        private void RefreshMaxSelectedItemsCount()
        {
            var additionalCapacity = _runState != null ? _runState.SelectedItemsCapacity.AdditionalCapacity : 0;
            MaxSelectedItemsCount = _initialMaxSelectedItemsCount + additionalCapacity;
        }

        private bool IsSelectionLimitReached => MaxSelectedItemsCount > 0 && _selectedItemsIds.Count >= MaxSelectedItemsCount;
    }
}
