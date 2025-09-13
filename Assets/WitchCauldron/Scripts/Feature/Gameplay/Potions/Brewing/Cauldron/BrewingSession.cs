using System.Collections.Generic;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Cauldron
{
    public class BrewingSession
    {

        private readonly Queue<BrewingIngredient> _ingredients;
        
        public BrewingSession(BrewingReceipt receipt)
        {
            
            
            var ingredients = new Queue<BrewingIngredient>();
            foreach (var part in receipt.Parts)
            {
                for (int i = 0; i < part.Quantity; i++)
                {
                    ingredients.Enqueue(part.Ingredient);
                }
            }
            
            _ingredients = ingredients;
        }

        public bool TryAddIngredient(BrewingIngredient ingredient)
        {
            if (ingredient != _ingredients.Peek()) return false;
            _ingredients.Dequeue();
            return true;

        }
        
    }
}