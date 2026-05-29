using UnityEngine;

namespace Hut.SO
{
    [CreateAssetMenu(fileName = "Hut Settings", menuName = "Game/Settings/Hut Settings")]
    public class HutSettings : ScriptableObject
    {
        [field: SerializeField] public GameMacroSettings MacroSettings { get; private set; }
    }
}