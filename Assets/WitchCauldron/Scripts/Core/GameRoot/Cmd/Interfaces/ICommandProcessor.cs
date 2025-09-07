namespace WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces
{
    public interface ICommandProcessor
    {
        void RegisterCommand<TCommandParameter>(ICommand<TCommandParameter> command) where TCommandParameter : ICommandParameter;
        
        bool Process<TCommandParameter>(TCommandParameter commandParameter) where TCommandParameter : ICommandParameter;
    }
}