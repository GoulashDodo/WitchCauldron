using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Potion Ingredient", menuName = "Game/Potions/Brewing/Ingredient")]
    public class PotionIngredient : ScriptableObject
    {
        [field: SerializeField] public string Name {get; private set; }
        [field: SerializeField] public string Description { get; private set; }
    }
}