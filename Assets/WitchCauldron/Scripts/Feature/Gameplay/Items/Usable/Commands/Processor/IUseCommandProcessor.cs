using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Items.Usable.Commands.Processor
{
    public interface IUseCommandProcessor
    {
        void RegisterHandler(IUseCommandHandler handler);
        bool Process(UseCommandParameters command, Vector2 position);
    }
}