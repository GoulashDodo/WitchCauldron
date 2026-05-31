using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Enemies.SO
{
    [CreateAssetMenu(fileName = "All Enemy Settings", menuName = "Game/Gameplay/Enemies/All Enemy Settings")]

    public class AllEnemySettings : ScriptableObject
    {
        [field: SerializeField] public EnemySettings[] AllSettings { get;  private set; }
        
    }
}