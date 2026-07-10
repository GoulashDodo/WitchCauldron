using Gameplay.Battle.Effects.Base;
using Gameplay.Battle.HealthSystem.Structs;
using UnityEngine;

namespace Gameplay.Battle.Effects.Damage
{
    
    [CreateAssetMenu(
        fileName = "DamageEffectData",
        menuName = "Game/Gameplay/Effects/Damage")]
    public class DamageEffectData : EffectData<DamageEffectRuntime>
    {

        [field: SerializeField] public float Damage { get;  private set; } = 1f;
        [field: SerializeField] public DamageType DamageType { get; private set; } = DamageType.Physical;
        
        [field: SerializeField] public float Periodicity { get; private set; } = 1f;


    }
}