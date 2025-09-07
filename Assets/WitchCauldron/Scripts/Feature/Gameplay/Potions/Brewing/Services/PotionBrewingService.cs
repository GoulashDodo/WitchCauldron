using System.Collections.Generic;
using R3;
using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Services
{
    public class PotionBrewingService
    {

        private readonly Queue<PotionIngredient> _ingredients = new();
        
        
        public PotionBrewingService(ReceiptService receiptService)
        {

            receiptService.IngredientsSelected.Subscribe(SetIngredientsQueue);

        }

        private void SetIngredientsQueue(List<PotionIngredient> ingredients)
        {
            _ingredients.Clear();
            
            foreach (var ingredient in ingredients)
            {
                _ingredients.Enqueue(ingredient);
            }
        }

        public void TryDequeueIngredient(PotionIngredient ingredient)
        {
            if (ingredient == _ingredients.Peek())
            {
                _ingredients.Dequeue();
            }
            
        }
        
        
    }
}