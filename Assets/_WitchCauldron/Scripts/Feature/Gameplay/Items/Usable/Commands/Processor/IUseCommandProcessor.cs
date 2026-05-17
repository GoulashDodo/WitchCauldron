using Feature.Gameplay.Items.Usable.Commands.Handler;
using UnityEngine;

namespace Feature.Gameplay.Items.Usable.Commands.Processor
{
    public interface IUseCommandProcessor
    {
        void RegisterHandler(IUseCommandHandler handler);
        bool Process(UseCommandParameters command, Vector2 position);
    }
}