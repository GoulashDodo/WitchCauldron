using Feature.Gameplay.Battle.Enemies.SO;
using Feature.Gameplay.Combination.ScriptableObjects;
using Feature.Gameplay.Items.SO;
using Feature.Gameplay.Level.SO;
using UnityEngine;

namespace Feature.Gameplay._root.SO
{
    [CreateAssetMenu(fileName = "Gameplay Settings", menuName = "Game/Settings/Gameplay Settings")]
    public class GameplaySettings : ScriptableObject
    {
        [field: SerializeField] public AllEnemySettings AllEnemiesSettings  { get; private set; }
        [field: SerializeField] public AllItemSettings AllItemsSettings { get; private set; }
        [field: SerializeField] public AllLevelSettings AllLevelSettings  { get; private set; }
        [field: SerializeField] public CombinationRuleList CombinationRuleList  { get; private set; }

    }
}