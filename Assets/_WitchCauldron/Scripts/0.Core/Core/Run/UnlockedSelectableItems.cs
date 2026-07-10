using System.Collections.Generic;

namespace Core.Run
{
    public class UnlockedSelectableItems
    {
        
        public IReadOnlyCollection<string> UnlockedItems => _unlockedSelectableItems;
        private readonly HashSet<string> _unlockedSelectableItems;

        public UnlockedSelectableItems(string[] initialUnlockedItems)
        {
            _unlockedSelectableItems = new HashSet<string>();

            if (initialUnlockedItems == null)
                return;

            foreach (var itemId in initialUnlockedItems)
                UnlockNewItem(itemId);
        }
        
        public bool HasItem(string itemId)
        {
            return !string.IsNullOrWhiteSpace(itemId) && _unlockedSelectableItems.Contains(itemId);
        }

        public void UnlockNewItem(string unlockedItem)
        {
            if (string.IsNullOrWhiteSpace(unlockedItem))
                return;

            _unlockedSelectableItems.Add(unlockedItem);
        }
        
    }
}
