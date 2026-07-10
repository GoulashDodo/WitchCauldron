using System;
using System.Collections.Generic;
using Gameplay.Items.Usable.Commands.Handler;
using UnityEngine;

namespace Gameplay.Items.Usable.Commands.Processor
{
    public class UseCommandProcessor : IUseCommandProcessor
    {
        private readonly Dictionary<Type, IUseCommandHandler> _handlers = new();

        public UseCommandProcessor(List<IUseCommandHandler> handlers)
        {
            foreach (var handler in handlers)
                RegisterHandler(handler);
        }

        public void RegisterHandler(IUseCommandHandler handler)
        {
            Debug.Log($"Registering {handler.ParametersType.Name}");
            _handlers[handler.ParametersType] = handler;
        }

        public bool Process(UseCommandParameters command, Vector2 position, UseCommandContext context = null)
        {
            if (command == null)
            {
                Debug.LogError("UseCommandProcessor.Process: command is null");
                return false;
            }

            var t = command.GetType();
            if (_handlers.TryGetValue(t, out var handler))
                return handler.Handle(command, position, context);


            Debug.LogError($"Handler for {t.Name} not found");
            return false;
        }
    }
}
