using UnityEngine;
using Sirenix.OdinInspector;

namespace Gameplay.Battle.Waves.SO.Structures
{
    [System.Serializable]
    public sealed class EnemySpawnDefinition
    {
        [field: SerializeField, LabelText("Enemy Type Id")]
        public string EnemyTypeId { get; private set; }

        [field: SerializeField, Min(0), LabelText("Manual Count")]
        [field: InfoBox("Used only when the wave spawn mode is Manual Count.")]
        public int Count { get; private set; } = 1;

        [field: SerializeField, Min(1), LabelText("Selection Weight")]
        public int Weight { get; private set; } = 1;

        [field: SerializeField, Min(0), LabelText("Min Count")]
        [field: InfoBox("Used only when the wave spawn mode is Point Budget.")]
        public int MinCount { get; private set; }

        [field: SerializeField, Min(0), LabelText("Max Count")]
        [field: InfoBox("Used only when the wave spawn mode is Point Budget. 0 means no limit.")]
        public int MaxCount { get; private set; }
    }
}
