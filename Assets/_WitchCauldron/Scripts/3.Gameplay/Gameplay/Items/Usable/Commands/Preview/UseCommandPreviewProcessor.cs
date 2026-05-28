using System;
using System.Collections.Generic;
using Gameplay.Items.SO;
using Gameplay.Items.Usable.Commands.Spawn;
using UnityEngine;

namespace Gameplay.Items.Usable.Commands.Preview
{
    public class UseCommandPreviewProcessor : IUseCommandPreviewProcessor
    {
        private readonly Dictionary<Type, IUseCommandPreviewProvider> _providers = new();

        public UseCommandPreviewProcessor()
        {
            RegisterProvider(new SpawnCommandPreviewProvider());
        }

        private void RegisterProvider(IUseCommandPreviewProvider provider)
        {
            _providers[provider.ParametersType] = provider;
        }

        public GameObject CreatePreview(UseCommandParameters command, Vector2 position, ItemSettings itemSettings)
        {
            if (command == null)
                return null;

            return _providers.TryGetValue(command.GetType(), out var provider)
                ? provider.CreatePreview(command, position, itemSettings)
                : null;
        }

        public void UpdatePreview(GameObject preview, UseCommandParameters command, Vector2 position, ItemSettings itemSettings)
        {
            if (preview == null || command == null)
                return;

            if (_providers.TryGetValue(command.GetType(), out var provider))
                provider.UpdatePreview(preview, command, position, itemSettings);
        }
    }
}
