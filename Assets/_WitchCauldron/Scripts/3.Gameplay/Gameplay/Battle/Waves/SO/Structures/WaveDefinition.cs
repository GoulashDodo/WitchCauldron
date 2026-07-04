using Gameplay.Battle.Waves.Enums;
using UnityEngine;

namespace Gameplay.Battle.Waves.SO.Structures
{
    [System.Serializable]
    public sealed class WaveDefinition
    {
        [field: SerializeField]
        public WaveType Type { get; private set; }

        [field: SerializeField, Min(0f)]
        [field: Tooltip("First wave: seconds after level wave start delay. Next waves: seconds after the previous wave is cleared.")]
        public float StartTime { get; private set; }

        [field: SerializeField, Min(0.1f)]
        public float SpawnInterval { get; private set; } = 1f;

        [field: SerializeField]
        public WaveSpawnMode SpawnMode { get; private set; } = WaveSpawnMode.ManualCount;

        [field: SerializeField, Min(0)]
        public int PointBudget { get; private set; }

        [field: SerializeField]
        public SpawnPositionMode SpawnPositionMode { get; private set; }

        [field: SerializeField]
        public float SpecificSpawnY { get; private set; }

        [field: SerializeField]
        public EnemySpawnDefinition[] Enemies { get; private set; }

        public int TotalEnemyCount
        {
            get
            {
                if (Enemies == null)
                    return 0;

                var count = 0;

                foreach (var enemy in Enemies)
                    count += Mathf.Max(0, enemy.Count);

                return count;
            }
        }

    }
}
