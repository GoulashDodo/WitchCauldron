using Gameplay.Battle.Effects.Base;
using UnityEngine;

namespace Gameplay.Battle.Effects.Slow
{
    
    [CreateAssetMenu(
        fileName = "SlowEffectData",
        menuName = "Game/Gameplay/Effects/Slow")]
    public class SlowEffectData : EffectData<SlowEffectRuntime>
    {
        [field: SerializeField] public float SpeedMultiplier { get; set; } = 1f; 
        
        
    }
}