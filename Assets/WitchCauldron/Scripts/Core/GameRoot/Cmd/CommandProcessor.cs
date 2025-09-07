using System;
using System.Collections.Generic;
using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;

namespace WitchCauldron.Scripts.Core.GameRoot.Cmd
{
    public class CommandProcessor : ICommandProcessor
    {

        private readonly Dictionary<Type, object> _commands = new();
        
        public void RegisterCommand<TCommandParameter>(ICommand<TCommandParameter> command) where TCommandParameter : ICommandParameter
        {
            _commands[typeof(TCommandParameter)] = command;
        }

        public bool Process<TCommandParameter>(TCommandParameter commandParameter) where TCommandParameter : ICommandParameter
        {
            if (_commands.TryGetValue(typeof(TCommandParameter), out var command))
            {
                var typedParameter = (ICommand<TCommandParameter>)command;
                var commandSucceeded = typedParameter.Handle(commandParameter);
                
                return commandSucceeded;
            }
            
            return false;
        }
    }
}
