using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Items.Item.Settings;

namespace WitchCauldron.Scripts.Feature.Gameplay.Combination.ScriptableObjects
{
    [System.Serializable]
    public class CombinationRule
    {
        [SerializeField] private ItemSettings _itemA;
        [SerializeField] private ItemSettings _itemB;
        [SerializeField] private ItemSettings _result;

        public ItemSettings ItemA => _itemA;
        public ItemSettings ItemB => _itemB;
        public ItemSettings Result => _result;

        public bool Matches(ItemSettings first, ItemSettings second)
        {
            return (first == _itemA && second == _itemB)
                   || (first == _itemB && second == _itemA);
        }
    }
}