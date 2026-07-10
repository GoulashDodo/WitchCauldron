using System.Collections.Generic;

namespace Core.Run
{
    public class DiscoveredItems
    {
        public IReadOnlyCollection<string> AllDiscoveredItemIds => _discoveredItemIds;
        private readonly HashSet<string> _discoveredItemIds;

        public DiscoveredItems(string[] initialDiscoveredItemIds)
        {
            _discoveredItemIds = new HashSet<string>();

            if (initialDiscoveredItemIds == null)
                return;

            foreach (var itemId in initialDiscoveredItemIds)
                DiscoverItem(itemId);
        }

        public bool HasItem(string itemId)
        {
            return !string.IsNullOrWhiteSpace(itemId) && _discoveredItemIds.Contains(itemId);
        }

        public void DiscoverItem(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                return;

            _discoveredItemIds.Add(itemId);
        }
    }
}
