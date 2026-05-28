using Gameplay.Items.SO;
using Gameplay.Items.Usable.Commands.Preview;
using UnityEngine;

namespace Gameplay.Items.Usable.Commands.Spawn
{
    public sealed class SpawnCommandPreviewProvider : UseCommandPreviewProvider<SpawnCommandParameters>
    {
        private const float PreviewAlpha = 0.45f;

        public override GameObject CreatePreview(SpawnCommandParameters parameters, Vector2 position, ItemSettings itemSettings)
        {
            return CommandPreviewVisualFactory.CreateSpriteGhost(parameters.Prefab, position, PreviewAlpha);
        }

        public override void UpdatePreview(
            GameObject preview,
            SpawnCommandParameters parameters,
            Vector2 position,
            ItemSettings itemSettings)
        {
            if (preview != null)
                preview.transform.position = position;
        }
    }
}
