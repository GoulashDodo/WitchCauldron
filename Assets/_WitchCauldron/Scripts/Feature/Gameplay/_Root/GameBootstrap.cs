using Feature.Gameplay.Battle.Waves.Service;
using UnityEngine;
using Zenject;

namespace Feature.Gameplay._root
{
    public class GameBootstrap : IInitializable
    {

        //private readonly GameplayEntryParameters _gameplayEntryParameters;
        private readonly WaveService _waveService;

        public GameBootstrap(WaveService waveService)
        {
            //_gameplayEntryParameters = gameplayEntryParameters;
            _waveService = waveService;
        }
        
        
        public void Initialize()
        {
            Debug.Log("Starting game");    
            //Debug.Log(_gameplayEntryParameters.LevelId);
            _waveService.StartLevel();
        }

    }
}