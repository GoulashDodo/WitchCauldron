using System;
using UnityEngine;

namespace Hut.Shop.SO
{
    [Serializable]
    public struct ShopUpgradeUnlockCondition
    {
        [field: SerializeField] public ShopUpgradeUnlockConditionType Type { get; private set; }
        [field: SerializeField] public string TargetId { get; private set; }
        [field: SerializeField] public int Amount { get; private set; }
    }
}
