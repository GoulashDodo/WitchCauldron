using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Enemies.SO
{
    [CreateAssetMenu(fileName = "All Enemy Settings", menuName = "Game/Enemies/All Enemy Settings")]

    public class AllEnemySettings : ScriptableObject
    {
        [field: SerializeField] public EnemySettings[] AllSettings { get;  private set; }
        
    }
}