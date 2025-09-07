namespace WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces
{
    public interface ICommand<in TCommandParameter> where TCommandParameter : ICommandParameter
    {
        bool Handle(TCommandParameter commandParameter);
    }
}