using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Items.Settings
{
    [CreateAssetMenu(fileName = "All Item Settings", menuName = "Game/Items/All Item Settings", order = 0)]
    public class AllItemSettings : ScriptableObject
    {
        [field: SerializeField] public ItemSettings[] ItemSettings { get; private set; }
    }
}