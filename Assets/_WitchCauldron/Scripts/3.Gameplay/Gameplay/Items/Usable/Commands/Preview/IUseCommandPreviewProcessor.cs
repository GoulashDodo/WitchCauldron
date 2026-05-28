using Gameplay.Items.SO;
using UnityEngine;

namespace Gameplay.Items.Usable.Commands.Preview
{
    public interface IUseCommandPreviewProcessor
    {
        GameObject CreatePreview(UseCommandParameters command, Vector2 position, ItemSettings itemSettings);
        void UpdatePreview(GameObject preview, UseCommandParameters command, Vector2 position, ItemSettings itemSettings);
    }
}
