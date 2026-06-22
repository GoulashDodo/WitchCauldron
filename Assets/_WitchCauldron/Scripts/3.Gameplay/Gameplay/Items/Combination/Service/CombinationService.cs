using System.Linq;
using Core.Run;
using Gameplay._root.SO;
using Gameplay.Items.Combination.ScriptableObjects;
using Gameplay.Items.SO;

namespace Gameplay.Items.Combination.Service
{
    public class CombinationService
    {
        private readonly CombinationRuleList _ruleList;
        private readonly RunState _runState;

        public CombinationService(GameplaySettings settings, RunState runState)
        {
            _ruleList = settings.CombinationRuleList;
            _runState = runState;
        }

        public ItemSettings TryCombine(ItemSettings first, ItemSettings second)
        {
            var rule = _ruleList.Rules.FirstOrDefault(r =>
                _runState.UnlockedRecipes.HasRecipe(r.RecipeId) &&
                r.Matches(first, second));

            return rule?.Result;
        }
    }
}
