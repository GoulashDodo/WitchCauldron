using System.Collections.Generic;
using R3;
using WitchCauldron.Scripts.Feature.Gameplay.Brewing.ScriptableObjects;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Settings;

namespace WitchCauldron.Scripts.Feature.Gameplay.Brewing.Cauldrons.Session
{
    public class BrewingSession
    {

        private readonly BrewingReceipt _brewingReceipt;
        private readonly Queue<BrewingIngredient> _ingredients; 
        private readonly Subject<PotionSettings> _brewingSessionFinished = new();
        
        
        public BrewingReceipt BrewingReceipt =>  _brewingReceipt;
        public IReadOnlyCollection<BrewingIngredient> Ingredients => _ingredients;
        public Observable<PotionSettings> BrewingSessionFinished => _brewingSessionFinished;
        
        
        public BrewingSession(BrewingReceipt receipt)
        {
            _brewingReceipt = receipt;
            
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
            if (_ingredients.Count == 0) return false;
            
            if (ingredient != _ingredients.Peek()) return false;
            _ingredients.Dequeue();

            if (_ingredients.Count == 0)
            {
                _brewingSessionFinished.OnNext(_brewingReceipt.ResultPotionSettings);
            }
            
            return true;

        }
        
    }
}