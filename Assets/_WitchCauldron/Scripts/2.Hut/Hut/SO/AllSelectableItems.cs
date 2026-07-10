using Gameplay.Items.SO;
using UnityEngine;

namespace Hut.SO
{
    [CreateAssetMenu(fileName = "AllSelectableItems", menuName = "Game/Hut/Settings/AllSelectableItems", order = 0)]
    public class AllSelectableItems : ScriptableObject
    {
        [field: SerializeField] public ItemSettings[] ItemSettings { get; private set; }  
    }
}