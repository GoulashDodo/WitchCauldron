using Feature.Gameplay.Items.Usable.Commands.Handler;
using UnityEngine;

namespace Feature.Gameplay.Items.Usable.Commands.Spawn
{
    public class SpawnCommandHandler : UseCommandHandler<SpawnCommandParameters>
    {
        public override bool Handle(SpawnCommandParameters p, Vector2 pos)
        {
            Object.Instantiate(p.Prefab, pos, Quaternion.identity);
            return true;
        }
    }
}