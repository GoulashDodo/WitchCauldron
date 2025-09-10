using UnityEngine;
using WitchCauldron.Scripts.Core.GameRoot.Cmd;
using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;
using WitchCauldron.Scripts.Core.GameRoot.State.Providers;
using WitchCauldron.Scripts.Core.GameRoot.View;
using Zenject;

namespace WitchCauldron.Scripts.Core.GameRoot.Root.EntryPoints.GameEntryPoint.Registrations
{
    public static class GlobalRegistrations
    {

        public static void Register(DiContainer rootContainer)
        {
            rootContainer.Bind<ICommandProcessor>().To<CommandProcessor>().AsSingle();
            
            rootContainer.Bind<IGameStateProvider>().To<PlayerPrefsGameStateProvider>().AsSingle();
            
            
            rootContainer.Bind<SceneLoader>().AsSingle();

            var prefabUIRoot = Resources.Load<UIRootView>("__UI__");
            var uiRoot = Object.Instantiate(prefabUIRoot);
            uiRoot.Initialize(rootContainer.Resolve<SceneLoader>());
            Object.DontDestroyOnLoad(uiRoot.gameObject);
            rootContainer.BindInstance(uiRoot);
            
            
        }

        
    }
}