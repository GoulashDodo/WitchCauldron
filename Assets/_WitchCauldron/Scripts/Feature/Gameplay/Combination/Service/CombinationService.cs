using System.Linq;
using _WitchCauldron.Scripts.Feature.Gameplay.Combination.ScriptableObjects;
using _WitchCauldron.Scripts.Feature.Gameplay.Items.Settings;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Combination.Service
{
    public class CombinationService
    {
        private readonly CombinationRuleList _ruleList;

        public CombinationService(CombinationRuleList ruleList)
        {
            _ruleList = ruleList;
        }

        public ItemSettings TryCombine(ItemSettings first, ItemSettings second)
        {
            var rule = _ruleList.Rules.FirstOrDefault(r => r.Matches(first, second));
            return rule?.Result;
        }
    }
}