using Gameplay.Items.SO;
using UnityEngine;

namespace Gameplay.Items.Combination.ScriptableObjects
{
    [System.Serializable]
    public class CombinationRule
    {
        [SerializeField] private string _recipeId;
        [SerializeField] private ItemSettings _itemA;
        [SerializeField] private ItemSettings _itemB;
        [SerializeField] private ItemSettings _result;

        public string RecipeId => string.IsNullOrWhiteSpace(_recipeId) ? GenerateRecipeId(_itemA, _itemB, _result) : _recipeId;
        public ItemSettings ItemA => _itemA;
        public ItemSettings ItemB => _itemB;
        public ItemSettings Result => _result;

        public bool Matches(ItemSettings first, ItemSettings second)
        {
            return (first == _itemA && second == _itemB)
                   || (first == _itemB && second == _itemA);
        }

        public static string GenerateRecipeId(ItemSettings itemA, ItemSettings itemB, ItemSettings result)
        {
            var itemAId = itemA != null ? itemA.TypeId : string.Empty;
            var itemBId = itemB != null ? itemB.TypeId : string.Empty;
            var resultId = result != null ? result.TypeId : string.Empty;

            if (string.CompareOrdinal(itemAId, itemBId) > 0)
                (itemAId, itemBId) = (itemBId, itemAId);

            return $"{itemAId}+{itemBId}->{resultId}";
        }
    }
}
