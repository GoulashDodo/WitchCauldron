using _WitchCauldron.Scripts.Feature.Gameplay.Waves.Service;
using UnityEngine;
using Zenject;

namespace _WitchCauldron.Scripts.Feature.Gameplay._Root
{
    public class GameBootstrap : IInitializable
    {
        
        private readonly WaveService _waveService;


        public GameBootstrap(WaveService waveService)
        {
            _waveService = waveService;
        }
        
        
        public void Initialize()
        {
            Debug.Log("Starting game");    
            
            _waveService.StartLevel();
        }

    }
}