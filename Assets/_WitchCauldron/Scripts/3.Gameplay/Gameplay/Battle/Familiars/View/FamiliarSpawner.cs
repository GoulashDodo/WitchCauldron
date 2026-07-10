using System;
using Gameplay._root;
using Gameplay.Battle.Familiars.Service;
using Gameplay.Level;
using R3;
using UnityEngine;
using Zenject;

namespace Gameplay.Battle.Familiars.View
{
    public class FamiliarSpawner : MonoBehaviour
    {
        
        private FamiliarService _familiarService;
        private GameplayEntryParameters _gameplayEntryParameters;
        private G _gameplay;
        
        private readonly CompositeDisposable _disposables = new();
        
        [Inject]
        public void Initialize(FamiliarService familiarService, GameplayEntryParameters gameplayEntryParameters, G gameplay)
        {
            if (familiarService == null || gameplayEntryParameters == null || gameplay == null)
                return;

            _familiarService = familiarService;
            _gameplayEntryParameters = gameplayEntryParameters;
            _gameplay = gameplay;

            Debug.Log("[FamiliarSpawner]: Initialized. Selected familiar: " + _gameplayEntryParameters.SelectedFamiliar);
            
            SubscribeToEvents();
        }

        private void SpawnFamiliar(Unit unit)
        {
            if (_familiarService == null || _gameplayEntryParameters == null)
                return;

            Debug.Log("[FamiliarSpawner]: Game started. Trying to spawn familiar: " + _gameplayEntryParameters.SelectedFamiliar);
            _familiarService.SpawnFamiliar(_gameplayEntryParameters.SelectedFamiliar, transform.position);
        }

        public void SubscribeToEvents()
        {
            _disposables.Clear();
            _gameplay.GameStarted.Subscribe(SpawnFamiliar).AddTo(_disposables);
        }


        private void OnDisable()
        {
            _disposables.Dispose();   
        }
    }
}
