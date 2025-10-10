using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Enemies
{
    [CreateAssetMenu(fileName = "Enemy Settings", menuName = "Game/Enemies/Enemy Settings")]
    public class EnemySettings : ScriptableObject
    {
        [field: SerializeField] public int PointPrice {get; private set;}
        
        [field: SerializeField] public int MaxHealth {get; private set;}
        [field: SerializeField] public int MaxSpeed {get; private set;}
    }
}
