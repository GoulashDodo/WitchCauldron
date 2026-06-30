using System.Collections.Generic;

namespace Core.Run
{
    public class AlmanacViewedItems
    {
        public IReadOnlyCollection<string> AllViewedItemIds => _viewedItemIds;
        private readonly HashSet<string> _viewedItemIds = new();

        public bool HasItem(string itemId)
        {
            return !string.IsNullOrWhiteSpace(itemId) && _viewedItemIds.Contains(itemId);
        }

        public void MarkViewed(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                return;

            _viewedItemIds.Add(itemId);
        }
    }
}
