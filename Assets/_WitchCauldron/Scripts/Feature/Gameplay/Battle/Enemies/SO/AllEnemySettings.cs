using UnityEngine;

namespace Feature.Gameplay.Battle.Enemies.SO
{
    [CreateAssetMenu(fileName = "All Enemy Settings", menuName = "Game/Enemies/All Enemy Settings")]

    public class AllEnemySettings : ScriptableObject
    {
        [field: SerializeField] public EnemySettings[] AllSettings { get;  private set; }
        
    }
}