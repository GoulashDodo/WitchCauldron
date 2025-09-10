using R3;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Model
{
    public class Cauldron
    {
        private readonly ReactiveProperty<BrewingSession> _brewingSession = new ReactiveProperty<BrewingSession>(); 
        
        public Observable<BrewingSession> BrewingSession => _brewingSession;
        
        public void SetBrewingSession(BrewingSession brewingSession)
        {
            _brewingSession.Value = brewingSession;
        }

        public void TryAddIngredient(BrewingIngredient ingredient)
        {
            _brewingSession.Value.TryAddIngredient(ingredient);
        }
        
    }
}