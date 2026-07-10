using System;
using Gameplay.Items.SO;
using UnityEngine;

namespace Gameplay.Items.Usable.Commands.Preview
{
    public interface IUseCommandPreviewProvider
    {
        Type ParametersType { get; }

        GameObject CreatePreview(UseCommandParameters parameters, Vector2 position, ItemSettings itemSettings);
        void UpdatePreview(GameObject preview, UseCommandParameters parameters, Vector2 position, ItemSettings itemSettings);
    }

    public interface IUseCommandPreviewProvider<in TParameters>
        : IUseCommandPreviewProvider
        where TParameters : UseCommandParameters
    {
        GameObject CreatePreview(TParameters parameters, Vector2 position, ItemSettings itemSettings);
        void UpdatePreview(GameObject preview, TParameters parameters, Vector2 position, ItemSettings itemSettings);
    }
}
