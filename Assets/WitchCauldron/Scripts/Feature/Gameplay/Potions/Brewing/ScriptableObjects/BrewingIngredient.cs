using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Brewing Ingredient", menuName = "Game/Potions/Brewing/Ingredient")]
    public class BrewingIngredient : ScriptableObject
    {
        [field: SerializeField] public string Name {get; private set; }
        [field: SerializeField] public string Description { get; private set; }
    }
}