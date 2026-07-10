using Gameplay._root.SO;
using Gameplay.Items.Usable.Commands.Handler;
using UnityEngine;
using Zenject;

namespace Gameplay.Items.Usable.Commands.Spawn
{
    public class SpawnCommandHandler : UseCommandHandler<SpawnCommandParameters>
    {
        private readonly Collider2D[] _overlapBuffer = new Collider2D[32];

        private readonly IInstantiator _instantiator;
        private readonly GameplaySettings _gameplaySettings;

        public SpawnCommandHandler(IInstantiator instantiator, GameplaySettings gameplaySettings)
        {
            _instantiator = instantiator;
            _gameplaySettings = gameplaySettings;
        }

        public override bool Handle(SpawnCommandParameters p, Vector2 pos, UseCommandContext context = null)
        {
            if (!SpawnPlacementQuery.CanSpawnAt(pos, _gameplaySettings, _overlapBuffer))
                return false;

            // TODO: Play Egg_Spawn here once spawn command data identifies egg-specific spawns safely.
            _instantiator.InstantiatePrefab(p.Prefab, pos, Quaternion.identity, null);
            return true;
        }
    }
}
