using WitchCauldron.Scripts.Core.GameRoot.Cmd;
using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;
using Zenject;

namespace WitchCauldron.Scripts.Core.GameRoot.Root
{
    public static class GlobalRegistrations
    {

        public static void Register(DiContainer rootContainer)
        {
            rootContainer.Bind<ICommandProcessor>().To<CommandProcessor>().AsSingle();
        }
    }
}