using Gameplay.Items.Usable.Commands.Handler;
using UnityEngine;
using Zenject;

namespace Gameplay.Items.Usable.Commands.Spawn
{
    public class SpawnCommandHandler : UseCommandHandler<SpawnCommandParameters>
    {
        private readonly IInstantiator _instantiator;

        public SpawnCommandHandler(IInstantiator instantiator)
        {
            _instantiator = instantiator;
        }

        public override bool Handle(SpawnCommandParameters p, Vector2 pos, UseCommandContext context = null)
        {
            _instantiator.InstantiatePrefab(p.Prefab, pos, Quaternion.identity, null);
            return true;
        }
    }
}
