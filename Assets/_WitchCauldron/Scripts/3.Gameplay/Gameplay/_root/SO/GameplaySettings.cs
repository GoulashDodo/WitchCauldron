using Gameplay.Battle.BattleEntities.Enemies.SO;
using Gameplay.Battle.Familiars.SO;
using Gameplay.Items.Combination.ScriptableObjects;
using Gameplay.Items.SO;
using Gameplay.Level.SO;
using Gameplay.UI.Enemies;
using UnityEngine;

namespace Gameplay._root.SO
{
    [CreateAssetMenu(fileName = "Gameplay Settings", menuName = "Game/Settings/Gameplay Settings")]
    public class GameplaySettings : ScriptableObject
    {
        [field: SerializeField, Min(0f)] public float VictoryDelay { get; private set; }
        [field: SerializeField, Min(0f)] public float SpawnedObjectMinDistance { get; private set; } = 0.65f;

        [field: SerializeField] public AllEnemySettings AllEnemiesSettings  { get; private set; }
        [field: SerializeField] public AllItemSettings AllItemsSettings { get; private set; }
        [field: SerializeField] public AllLevelSettings AllLevelSettings  { get; private set; }
        
        [field: SerializeField] public AllFamiliarsData AllFamiliarsData { get; private set; }
        
        [field: SerializeField] public CombinationRuleList CombinationRuleList  { get; private set; }
        [field: SerializeField] public AllDamageTypeTextSettings DamageTypeTextSettings { get; private set; }

    }
}
