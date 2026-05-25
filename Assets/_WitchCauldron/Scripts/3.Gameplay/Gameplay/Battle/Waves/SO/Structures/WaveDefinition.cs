using Gameplay.Battle.Waves.Enums;
using UnityEngine;

namespace Gameplay.Battle.Waves.SO.Structures
{
    [System.Serializable]
    public sealed class WaveDefinition
    {
        [field: SerializeField] public WaveType Type { get; private set; }
        [field: SerializeField, Min(0f)] public float StartTime { get; private set; }
        [field: SerializeField, Min(0.1f)] public float SpawnInterval { get; private set; } = 1f;
        [field: SerializeField] public SpawnPositionMode SpawnPositionMode { get; private set; }
        [field: SerializeField] public Vector3 SpecificSpawnPosition { get; private set; }
        [field: SerializeField] public EnemySpawnDefinition[] Enemies { get; private set; }

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

        public float Duration => TotalEnemyCount <= 0 ? 0f : (TotalEnemyCount - 1) * SpawnInterval;
    }
}