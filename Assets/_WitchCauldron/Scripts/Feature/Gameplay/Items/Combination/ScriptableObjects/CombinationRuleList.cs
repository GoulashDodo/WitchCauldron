using System.Collections.Generic;
using UnityEngine;

namespace Feature.Gameplay.Combination.ScriptableObjects
{
    [CreateAssetMenu(fileName = "CombinationRuleList", menuName = "Game/Items/Combination Rules", order = 1)]
    public class CombinationRuleList : ScriptableObject
    {
        [SerializeField] private List<CombinationRule> _rules;
        public IReadOnlyList<CombinationRule> Rules => _rules;
    }
}