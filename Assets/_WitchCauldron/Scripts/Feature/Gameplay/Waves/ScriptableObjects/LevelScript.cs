using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Waves.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Level Script", menuName = "Game/Level/Level Script", order = 0)]
    public class LevelScript : ScriptableObject
    {
        [field: SerializeField] public float LevelDuration { get; private set; }
    }
}