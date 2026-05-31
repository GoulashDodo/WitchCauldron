using Gameplay.Battle.Effects.Base;
using UnityEngine;

namespace Gameplay.Items.Usable.Commands.Effect
{
    
    [CreateAssetMenu(
        fileName = "Effect Command",
        menuName = "Game/Gameplay/Items/Parameters/Effect",
        order = 2)]
    public class EffectCommandParameters: UseCommandParameters
    {
        [field: SerializeField] public EffectData EffectToApply { get; private set; }
        
        [field: SerializeField] public float Radius { get; private set; }
        [field: SerializeField] public bool IsArea { get; private set; }
        
    }
}