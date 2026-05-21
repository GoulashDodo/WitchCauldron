using UnityEngine;

namespace Feature.Gameplay.Battle.Enemies.SO
{
    [System.Serializable]
    public class EnemyLootDefinition
    {
        [field: SerializeField] public string DropItemTypeId {get; private set;}
        [field: SerializeField] public float ChanceToDropItem {get; private set;} = 1;
    }
}