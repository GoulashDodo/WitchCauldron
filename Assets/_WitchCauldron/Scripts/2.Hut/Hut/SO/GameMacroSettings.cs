using UnityEngine;

namespace Hut.SO
{
    [CreateAssetMenu(fileName = "GameMacroSettings", menuName = "Game/Hut/Settings/GameMacroSettings")]
    public class GameMacroSettings : ScriptableObject
    {
        [field: SerializeField] public int MinimumSelectedItemsCount { get; private set; } = 1;
        [field: SerializeField] public int InitialSelectedItemsCount { get; private set; }
        [field: SerializeField] public int InitialMoney { get; private set; }
        [field: SerializeField] public float InitialBaseHealth { get; private set; } = 30f;


        [field: SerializeField] public string[] InitialSelectableItemsIds{ get; private set; }
        [field: SerializeField] public string[] InitialRecipeIds { get; private set; }
        [field: SerializeField] public string[] InitialDiscoveredItemIds { get; private set; }

    }
}
