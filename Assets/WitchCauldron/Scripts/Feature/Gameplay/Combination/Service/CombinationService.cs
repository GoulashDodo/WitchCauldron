using System.Linq;
using WitchCauldron.Scripts.Feature.Gameplay.Combination.ScriptableObjects;
using WitchCauldron.Scripts.Feature.Gameplay.Items.Item.Settings;

namespace WitchCauldron.Scripts.Feature.Gameplay.Combination.Service
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