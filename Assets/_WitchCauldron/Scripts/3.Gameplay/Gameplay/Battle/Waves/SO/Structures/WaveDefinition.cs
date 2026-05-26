using Gameplay.Battle.Waves.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.Battle.Waves.SO.Structures
{
    [System.Serializable]
    public sealed class WaveDefinition
    {
        [field: SerializeField, BoxGroup("Timing")]
        public WaveType Type { get; private set; }

        [field: SerializeField, BoxGroup("Timing")]
        public WaveStartMode StartMode { get; private set; } = WaveStartMode.Timeline;

        [field: SerializeField, Min(0f), BoxGroup("Timing")]
        [field: LabelText("Start Time / Delay")]
        [field: InfoBox("Timeline: seconds after level wave start. After Previous Cleared: seconds after previous wave is fully cleared.")]
        public float StartTime { get; private set; }

        [field: SerializeField, Min(0.1f), BoxGroup("Timing")]
        public float SpawnInterval { get; private set; } = 1f;

        [field: SerializeField, BoxGroup("Spawn Generation")]
        public WaveSpawnMode SpawnMode { get; private set; } = WaveSpawnMode.ManualCount;

        [field: SerializeField, Min(0), BoxGroup("Spawn Generation")]
        [field: ShowIf(nameof(IsPointBudgetMode))]
        public int PointBudget { get; private set; }

        [field: SerializeField, BoxGroup("Position")]
        public SpawnPositionMode SpawnPositionMode { get; private set; }

        [field: SerializeField, BoxGroup("Position")]
        [field: ShowIf(nameof(UsesSpecificSpawnPosition))]
        public Vector3 SpecificSpawnPosition { get; private set; }

        [field: SerializeField, BoxGroup("Enemies")]
        [field: TableList]
        public EnemySpawnDefinition[] Enemies { get; private set; }

        private bool IsPointBudgetMode => SpawnMode == WaveSpawnMode.PointBudget;
        private bool UsesSpecificSpawnPosition => SpawnPositionMode == SpawnPositionMode.SpecificPosition;

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
