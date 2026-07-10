using System.Collections.Generic;

namespace Core.Run
{
    public class PurchasedShopUpgrades
    {
        public IReadOnlyCollection<string> PurchasedUpgrades => _purchasedUpgrades;
        private readonly HashSet<string> _purchasedUpgrades = new();

        public bool HasUpgrade(string upgradeId)
        {
            return !string.IsNullOrWhiteSpace(upgradeId) && _purchasedUpgrades.Contains(upgradeId);
        }

        public void MarkPurchased(string upgradeId)
        {
            if (string.IsNullOrWhiteSpace(upgradeId))
                return;

            _purchasedUpgrades.Add(upgradeId);
        }
    }
}
