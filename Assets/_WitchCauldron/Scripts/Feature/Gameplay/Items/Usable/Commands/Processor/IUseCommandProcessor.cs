using _WitchCauldron.Scripts.Feature.Gameplay.Items.Usable.Commands.Handler;
using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Items.Usable.Commands.Processor
{
    public interface IUseCommandProcessor
    {
        void RegisterHandler(IUseCommandHandler handler);
        bool Process(UseCommandParameters command, Vector2 position);
    }
}