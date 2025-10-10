using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Settings
{
    [CreateAssetMenu(fileName = "PotionsSettings", menuName = "Game/Potions/Potion Settings", order = 0)]
    public class AllPotionsSettings : ScriptableObject
    {
        [field: SerializeField] public PotionSettings[] PotionSettings { get; private set; }
    }
}