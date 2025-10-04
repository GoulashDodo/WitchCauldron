using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Brewing.Cauldrons;

namespace WitchCauldron.Scripts.Feature.Gameplay.Brewing.Services
{
    public class CauldronService
    {
        
        private readonly BrewingService _brewingService;

        public CauldronService(BrewingService brewingService)
        {
            _brewingService = brewingService;
        }
        
        public Cauldron CreateCauldron(Cauldron cauldronPf, Vector3 position)
        {

            var cauldron = Object.Instantiate(cauldronPf,  position, Quaternion.identity);
            cauldron.Initialize(this, _brewingService);
            
            return cauldron;
        }
        
    }
}