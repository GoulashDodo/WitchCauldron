using System.Collections.Generic;
using System.Linq;
using Core.Run;
using Gameplay.Items.Combination.ScriptableObjects;
using Gameplay.Items.SO;

namespace Gameplay.Items.Knowledge
{
    public class ItemKnowledgeService
    {
        private readonly AllItemSettings _allItemSettings;
        private readonly CombinationRuleList _combinationRuleList;
        private readonly RunState _runState;

        public ItemKnowledgeService(
            AllItemSettings allItemSettings,
            CombinationRuleList combinationRuleList,
            RunState runState)
        {
            _allItemSettings = allItemSettings;
            _combinationRuleList = combinationRuleList;
            _runState = runState;
        }

        public IReadOnlyList<ItemSettings> GetAllItems()
        {
            if (_allItemSettings?.ItemSettings == null)
                return System.Array.Empty<ItemSettings>();

            return _allItemSettings.ItemSettings
                .Where(item => item != null)
                .OrderBy(item => IsAvailable(item) ? 0 : 1)
                .ThenBy(item => item.Tier)
                .ThenBy(GetAvailableAlmanacSortGroup)
                .ThenBy(item => item.TitleLid)
                .ThenBy(item => item.TypeId)
                .ToArray();
        }

        public IReadOnlyList<ItemSettings> GetAllItemsSortedByTier()
        {
            if (_allItemSettings?.ItemSettings == null)
                return System.Array.Empty<ItemSettings>();

            return _allItemSettings.ItemSettings
                .Where(item => item != null)
                .OrderBy(item => item.Tier)
                .ThenBy(item => item.TitleLid)
                .ThenBy(item => item.TypeId)
                .ToArray();
        }

        public bool IsDiscovered(ItemSettings item)
        {
            return item != null && _runState.DiscoveredItems.HasItem(item.TypeId);
        }

        public bool IsAlmanacViewed(ItemSettings item)
        {
            return IsDiscovered(item) && _runState.AlmanacViewedItems.HasItem(item.TypeId);
        }

        public void MarkAlmanacViewed(ItemSettings item)
        {
            if (IsDiscovered(item))
                _runState.AlmanacViewedItems.MarkViewed(item.TypeId);
        }

        public bool IsAvailable(ItemSettings item)
        {
            if (item == null)
                return false;

            if (_runState.UnlockedSelectableItems.HasItem(item.TypeId))
                return true;

            foreach (var rule in GetRules())
            {
                if (rule?.Result == item && _runState.UnlockedRecipes.HasRecipe(rule.RecipeId))
                    return true;
            }

            return false;
        }

        public List<CombinationRule> GetUnlockedRecipesForResult(ItemSettings result)
        {
            var rules = new List<CombinationRule>();

            foreach (var rule in GetRules())
            {
                if (rule?.Result == result && _runState.UnlockedRecipes.HasRecipe(rule.RecipeId))
                    rules.Add(rule);
            }

            return rules;
        }

        public List<CombinationRule> GetAllRecipesForResult(ItemSettings result)
        {
            var rules = new List<CombinationRule>();

            foreach (var rule in GetRules())
            {
                if (rule?.Result == result)
                    rules.Add(rule);
            }

            return rules;
        }

        private IReadOnlyList<CombinationRule> GetRules()
        {
            return _combinationRuleList != null ? _combinationRuleList.Rules : System.Array.Empty<CombinationRule>();
        }

        private int GetAvailableAlmanacSortGroup(ItemSettings item)
        {
            if (IsDiscovered(item))
                return 0;

            if (IsAvailable(item))
                return 1;

            return 0;
        }
    }
}
