using System.Collections.Generic;
using Gameplay._root.SO;
using UnityEngine;
using Zenject;

namespace Gameplay.Battle.Familiars.Service
{
    public class FamiliarService
    {
        private readonly Dictionary<string, GameObject> _familiarsPfs = new();
        private readonly IInstantiator _instantiator;
        
        
        public FamiliarService(GameplaySettings gameplaySettings, IInstantiator instantiator)
        {
            _instantiator = instantiator;
            var familiarsData = gameplaySettings?.AllFamiliarsData?.AllData;

            if (familiarsData == null)
            {
                Debug.LogWarning("No Familiars data loaded");
                return;   
            }
            
            foreach (var data in familiarsData)
            {
                if (data == null || string.IsNullOrWhiteSpace(data.FamiliarTypeId) || data.FamiliarPrefab == null)
                    continue;

                _familiarsPfs[data.FamiliarTypeId] = data.FamiliarPrefab;
            }
        }


        public void SpawnFamiliar(string familiarTypeId, Vector3 position)
        {
            if (string.IsNullOrWhiteSpace(familiarTypeId))
            {
                Debug.Log("[FamiliarService]: No familiar selected.");
                return;
            }

            if (!_familiarsPfs.TryGetValue(familiarTypeId, out var familiar))
            {
                Debug.LogWarning("Familiar not found: " + familiarTypeId);
                return;
            }

            Debug.Log("[FamiliarService]: Spawning familiar: " + familiarTypeId);
            _instantiator.InstantiatePrefab(familiar, position, Quaternion.identity, null);
        }
    }
}
