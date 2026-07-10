using Hut.Shop.SO;
using UnityEngine;

namespace Hut.SO
{
    [CreateAssetMenu(fileName = "Hut Settings", menuName = "Game/Settings/Hut Settings")]
    public class HutSettings : ScriptableObject
    {
        [field: SerializeField] public GameMacroSettings MacroSettings { get; private set; }
        [field: SerializeField] public AllShopUpgrades ShopUpgrades { get; private set; }
    }
}
