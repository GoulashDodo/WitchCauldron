using UnityEngine;

namespace Hut.SO
{
    [CreateAssetMenu(fileName = "GameMacroSettings", menuName = "Game/Hut/Settings/GameMacroSettings")]
    public class GameMacroSettings : ScriptableObject
    {
        [field: SerializeField] public int MinimumSelectedItemsCount { get; private set; } = 1;
        [field: SerializeField] public int InitialSelectedItemsCount { get; private set; }
    }
}
