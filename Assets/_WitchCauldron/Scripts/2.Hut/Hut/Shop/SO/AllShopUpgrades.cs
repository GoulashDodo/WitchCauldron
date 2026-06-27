using UnityEngine;

namespace Hut.Shop.SO
{
    [CreateAssetMenu(fileName = "AllShopUpgrades", menuName = "Game/Hut/Shop/All Shop Upgrades", order = 1)]
    public class AllShopUpgrades : ScriptableObject
    {
        [field: SerializeField] public ShopUpgradeDefinition[] Upgrades { get; private set; }
    }
}
