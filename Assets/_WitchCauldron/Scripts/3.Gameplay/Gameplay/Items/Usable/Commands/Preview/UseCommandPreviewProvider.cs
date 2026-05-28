using System;
using Gameplay.Items.SO;
using UnityEngine;

namespace Gameplay.Items.Usable.Commands.Preview
{
    public abstract class UseCommandPreviewProvider<T> : IUseCommandPreviewProvider<T>
        where T : UseCommandParameters
    {
        public Type ParametersType => typeof(T);

        public GameObject CreatePreview(UseCommandParameters parameters, Vector2 position, ItemSettings itemSettings)
        {
            return CreatePreview((T)parameters, position, itemSettings);
        }

        public void UpdatePreview(
            GameObject preview,
            UseCommandParameters parameters,
            Vector2 position,
            ItemSettings itemSettings)
        {
            UpdatePreview(preview, (T)parameters, position, itemSettings);
        }

        public abstract GameObject CreatePreview(T parameters, Vector2 position, ItemSettings itemSettings);

        public abstract void UpdatePreview(
            GameObject preview,
            T parameters,
            Vector2 position,
            ItemSettings itemSettings);
    }
}
