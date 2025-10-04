using R3;
using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Brewing.ScriptableObjects;
using WitchCauldron.Scripts.Feature.Gameplay.Brewing.Services;
using Zenject;

namespace WitchCauldron.Scripts.Feature.Gameplay.Brewing.Cauldrons
{
    public class Cauldron : MonoBehaviour
    {

        private CauldronService _cauldronService;
        private BrewingService _brewingService;
        
        private readonly ReactiveProperty<BrewingSession> _currentBrewingSession = new ();
        public Observable<BrewingSession> BrewingSession => _currentBrewingSession;
            
            
        [Inject]
        public void Initialize(CauldronService cauldronService, BrewingService brewingService)
        {
            _cauldronService = cauldronService;
            _brewingService = brewingService;

            _currentBrewingSession.Value = _brewingService.CreateBrewingSession();
        }


        public void TryAddItem(BrewingIngredient ingredient)
        {
            if (_currentBrewingSession.Value == null)
            {
                Debug.LogError("There is no brewing session available");
                return;
            } 
            
            var result = _brewingService.TryDequeueIngredient(_currentBrewingSession.Value,  ingredient);
            
            Debug.Log($"{ingredient.Name} is being added ${result}");
        }



    }
}
