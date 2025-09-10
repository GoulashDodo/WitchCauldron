using System.Collections.Generic;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Model
{
    public class BrewingSession
    {
        public BrewingReceipt Receipt { get; }

        public Queue<BrewingIngredient> Ingredients { get; }
        
        public BrewingSession(BrewingReceipt receipt)
        {
            Receipt = receipt;
            
            
            var ingredients = new Queue<BrewingIngredient>();
            foreach (var part in receipt.Parts)
            {
                for (int i = 0; i < part.Quantity; i++)
                {
                    ingredients.Enqueue(part.Ingredient);
                }
            }
            
            Ingredients = ingredients;
        }

        public bool TryAddIngredient(BrewingIngredient ingredient)
        {
            if (ingredient != Ingredients.Peek()) return false;
            
            
            Ingredients.Dequeue();
            return true;

        }
        
    }
}