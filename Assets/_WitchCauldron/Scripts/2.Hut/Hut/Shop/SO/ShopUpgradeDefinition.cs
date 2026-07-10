using UnityEngine;

namespace Hut.Shop.SO
{
    [CreateAssetMenu(fileName = "ShopUpgrade", menuName = "Game/Hut/Shop/Upgrade Definition", order = 0)]
    public class ShopUpgradeDefinition : ScriptableObject
    {
        [field: SerializeField] public string UpgradeId { get; private set; }
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: TextArea]
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public int Price { get; private set; }
        [field: SerializeField] public ShopUpgradeCategory Category { get; private set; }
        [field: SerializeField] public bool HideUntilUnlocked { get; private set; }
        [field: SerializeField] public ShopUpgradeEffect[] Effects { get; private set; }
        [field: SerializeField] public ShopUpgradeUnlockCondition[] UnlockConditions { get; private set; }
    }
}
