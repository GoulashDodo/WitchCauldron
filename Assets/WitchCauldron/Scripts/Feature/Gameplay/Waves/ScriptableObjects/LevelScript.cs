using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Waves.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Level Script", menuName = "Game/Level/Level Script", order = 0)]
    public class LevelScript : ScriptableObject
    {
        public float LevelDuration { get; private set; }
    }
}