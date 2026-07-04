using UnityEngine;

namespace Gameplay.Items.Usable.Commands.Preview
{
    public interface IPlacementBoxPreviewSource
    {
        Vector2 PreviewSize { get; }
        Vector2 PreviewOffset { get; }
        Transform PreviewOrigin { get; }
    }
}
