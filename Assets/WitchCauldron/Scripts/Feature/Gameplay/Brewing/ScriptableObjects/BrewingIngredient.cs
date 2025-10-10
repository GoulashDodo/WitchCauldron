    using UnityEngine;
    using WitchCauldron.Scripts.Feature.Gameplay.Potions.Settings;

    namespace WitchCauldron.Scripts.Feature.Gameplay.Brewing.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Brewing Ingredient", menuName = "Game/Brewing/Ingredient")]
    public class BrewingIngredient : ScriptableObject
    {
        
        [field: SerializeField] public string TypeId { get; private set; }
        
        
        
        [field: Space(10)]
        [field: SerializeField] public string TitleLid {get; private set; }
        [field: SerializeField] public string DescriptionLid { get; private set; }
    }
}