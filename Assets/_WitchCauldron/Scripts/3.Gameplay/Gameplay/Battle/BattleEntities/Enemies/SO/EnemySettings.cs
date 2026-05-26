using Gameplay.Battle.BattleEntities.Enemies.Core;
using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Enemies.SO
{
    [CreateAssetMenu(fileName = "Enemy Settings", menuName = "Game/Enemies/Enemy Settings")]
    public class EnemySettings : ScriptableObject
    {
     
        
        [field: Header("Identity & Prefab")]
        [field: SerializeField] public string TypeId {get; private set;}
        [field: SerializeField] public Enemy EnemyPf { get; private set; }


        [field: Space(10)] [field: Header("Reward & Cost")]
        [field: SerializeField, Min(1)] public int PointPrice { get; private set; } = 1;
        
        
        
        [field: Space(10)] [field: Header("Core Attributes")]
        [field: SerializeField] public float MaxHealth {get; private set;}
        [field: SerializeField] public float MaxSpeed {get; private set;}


        [field: Space(10)]
        [field: Header("Attack")]
        [field: SerializeField] public float Damage { get; private set; } = 1;
        [field: SerializeField] public float AttackDistance { get; private set; } = 1;
        [field: SerializeField] public float AttackSpeed { get; private set; } = 1;
        
        
        
        [field: Space(10)]
        [field: Header("Loot")]
        [field: SerializeField] public EnemyLootDefinition[] LootDefinitions { get; private set; } 
        
    }
}
