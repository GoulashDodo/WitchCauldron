using UnityEngine;

namespace Feature.Gameplay.Battle.Waves.SO.Structures
{
    [System.Serializable]
    public sealed class EnemySpawnDefinition
    {
        [field: SerializeField] public string EnemyTypeId { get; private set; }
        [field: SerializeField, Min(0)] public int Count { get; private set; } = 1;
        [field: SerializeField, Min(1)] public int Weight { get; private set; } = 1;
    }
}