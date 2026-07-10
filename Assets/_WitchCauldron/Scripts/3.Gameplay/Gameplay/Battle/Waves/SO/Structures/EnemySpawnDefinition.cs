using UnityEngine;

namespace Gameplay.Battle.Waves.SO.Structures
{
    [System.Serializable]
    public sealed class EnemySpawnDefinition
    {
        [field: SerializeField]
        public string EnemyTypeId { get; private set; }

        [field: SerializeField, Min(0)]
        [field: Tooltip("Used only when the wave spawn mode is Manual Count.")]
        public int Count { get; private set; } = 1;

        [field: SerializeField, Min(1)]
        public int Weight { get; private set; } = 1;

        [field: SerializeField, Min(0)]
        [field: Tooltip("Used only when the wave spawn mode is Point Budget.")]
        public int MinCount { get; private set; }

        [field: SerializeField, Min(0)]
        [field: Tooltip("Used only when the wave spawn mode is Point Budget. 0 means no limit.")]
        public int MaxCount { get; private set; }
    }
}
