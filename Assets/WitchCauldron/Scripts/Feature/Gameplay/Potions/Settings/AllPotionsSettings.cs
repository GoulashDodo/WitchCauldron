using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Settings
{
    [CreateAssetMenu(fileName = "PotionsSettings", menuName = "Game/Potions/All Potions Settings", order = 0)]
    public class AllPotionsSettings : ScriptableObject
    {
        [field: SerializeField] public PotionSettings[] PotionSettings { get; private set; }
    }
}