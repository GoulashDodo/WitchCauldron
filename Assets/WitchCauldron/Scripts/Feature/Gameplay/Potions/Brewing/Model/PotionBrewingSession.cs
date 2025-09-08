using System.Collections.Generic;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Model
{
    public class PotionBrewingSession
    {
        public BrewingReceipt Receipt { get; }
        
        public Queue<BrewingIngredient> Ingredients { get; }
        
        public PotionBrewingSession(PotionBrewingSessionData data)
        {
            Receipt = data.PotionReceipt;
            Ingredients = data.CurrentPotionIngredientsQueue;

        }
    }
}