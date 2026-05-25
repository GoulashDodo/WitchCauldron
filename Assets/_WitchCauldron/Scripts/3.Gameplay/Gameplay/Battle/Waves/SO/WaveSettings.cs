using Gameplay.Battle.Waves.SO.Structures;
using UnityEngine;

namespace Gameplay.Battle.Waves.SO
{
    [CreateAssetMenu(fileName = "Wave Settings", menuName = "Game/Gameplay/Waves/Wave Settings")]
    public class WaveSettings : ScriptableObject
    {
        [field: SerializeField, Min(0f)] public float StartDelay { get; private set; } = 2f;
        [field: SerializeField] public WaveDefinition[] Waves { get; private set; }
    }
    
    
}
