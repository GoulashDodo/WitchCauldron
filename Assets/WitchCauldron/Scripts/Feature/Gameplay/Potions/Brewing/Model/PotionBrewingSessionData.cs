using System.Collections.Generic;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Model
{
    public class PotionBrewingSessionData
    {
        public BrewingReceipt PotionReceipt { get; set; }
        
        public Queue<BrewingIngredient> CurrentPotionIngredientsQueue {get; set;}

        
    }
}