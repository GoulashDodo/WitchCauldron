using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Enemies.SO
{
    [CreateAssetMenu(fileName = "Enemy Settings", menuName = "Game/Enemies/Enemy Settings")]
    public class EnemySettings : ScriptableObject
    {
        
        [field: SerializeField] public string TypeId {get; private set;}
        [field: SerializeField, Min(1)] public int PointPrice { get; private set; } = 1;
        
        
        [field: Space(10)]
        [field: SerializeField] public float MaxHealth {get; private set;}
        [field: SerializeField] public float MaxSpeed {get; private set;}

        [field: SerializeField] public Enemy EnemyPf { get; private set; }
    }
}
