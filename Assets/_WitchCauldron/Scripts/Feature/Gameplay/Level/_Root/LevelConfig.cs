using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Level._Root
{
    [CreateAssetMenu(fileName = "New Level Config", menuName = "Game/Gameplay/Level/Level Config", order = 0)]
    public class LevelConfig : ScriptableObject
    {
        [field:SerializeField] public float BaseHealth { get; private set; }
    }
}