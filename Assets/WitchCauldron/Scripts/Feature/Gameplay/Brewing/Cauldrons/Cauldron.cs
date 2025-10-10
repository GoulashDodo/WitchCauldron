using System.Linq;
using R3;
using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Brewing.Cauldrons.Session;
using WitchCauldron.Scripts.Feature.Gameplay.Brewing.ScriptableObjects;
using WitchCauldron.Scripts.Feature.Gameplay.Brewing.Services;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Settings;
using Zenject;

namespace WitchCauldron.Scripts.Feature.Gameplay.Brewing.Cauldrons
{
    public class Cauldron : MonoBehaviour
    {

        private CauldronService _cauldronService;
        private BrewingService _brewingService;
        
        private readonly ReactiveProperty<BrewingSession> _currentBrewingSession = new ();
        
        public Observable<PotionSettings> BrewingSessionFinished => _currentBrewingSession.CurrentValue.BrewingSessionFinished;  

        
        //TODO: REMOVE, DEBUG PURPOSE ONLY
        private void Start()
        {
            _currentBrewingSession.Skip(1).Subscribe(session =>
            {
                Debug.Log($"{name}: {session.BrewingReceipt.TypeId}\n" +
                          string.Join("\n", session.Ingredients.Select(i => $"Ingredient: {i.TypeId}")));

                BrewingSessionFinished.Subscribe(_ =>
                {
                    Debug.Log($"{name}: brewing session finished.");
                });
                
            });
            
            
        }


        [Inject]
        public void Initialize(CauldronService cauldronService, BrewingService brewingService)
        {
            _cauldronService = cauldronService;
            _brewingService = brewingService;

            
            //Todo: Change the selection of brewing session
            _currentBrewingSession.Value = _brewingService.CreateBrewingSession();
            
            
            _cauldronService.RegisterCauldron(this);
        }


        public void TryAddItem(BrewingIngredient ingredient)
        {
            if (_currentBrewingSession.Value == null || _currentBrewingSession.Value.Ingredients.Count == 0)        
            {
                Debug.LogWarning($" {name}: There is no brewing session available");
                return;
            } 
            
            var result = _brewingService.TryDequeueIngredient(_currentBrewingSession.Value,  ingredient);
            
        }

        
    }
}
