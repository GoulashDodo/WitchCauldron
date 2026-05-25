using System.Linq;
using Gameplay._root.SO;
using Gameplay.Items.Combination.ScriptableObjects;
using Gameplay.Items.SO;

namespace Gameplay.Items.Combination.Service
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