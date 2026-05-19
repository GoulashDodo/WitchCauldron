using System.Linq;
using Feature.Gameplay._root.SO;
using Feature.Gameplay.Items.Combination.ScriptableObjects;
using Feature.Gameplay.Items.SO;

namespace Feature.Gameplay.Items.Combination.Service
{
    public class CombinationService
    {
        private readonly CombinationRuleList _ruleList;

        public CombinationService(GameplaySettings settings)
        {
            _ruleList = settings.CombinationRuleList;
        }

        public ItemSettings TryCombine(ItemSettings first, ItemSettings second)
        {
            var rule = _ruleList.Rules.FirstOrDefault(r => r.Matches(first, second));
            return rule?.Result;
        }
    }
}