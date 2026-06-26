using UnityEngine;

namespace Gameplay.Items.Usable.Commands.Preview
{
    public interface IPlacementRadiusPreviewSource
    {
        float PreviewRadius { get; }
        Transform PreviewOrigin { get; }
    }
}
