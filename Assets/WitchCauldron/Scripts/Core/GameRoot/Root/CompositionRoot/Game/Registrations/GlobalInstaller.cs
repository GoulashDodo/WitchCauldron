using UnityEngine;
using WitchCauldron.Scripts.Core.GameRoot.Cmd;
using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;
using WitchCauldron.Scripts.Core.GameRoot.State.Providers;
using WitchCauldron.Scripts.Core.GameRoot.View;
using Zenject;

namespace WitchCauldron.Scripts.Core.GameRoot.Root.CompositionRoot.Game.Registrations
{
    public sealed class GlobalInstaller : MonoInstaller
    {
        [Header("UI Root")]
        [SerializeField] private UIRootView _uiRootPrefab;

        public override void InstallBindings()
        {
            Container.Bind<ICommandProcessor>().To<CommandProcessor>().AsSingle();
            Container.Bind<IGameStateProvider>().To<PlayerPrefsGameStateProvider>().AsSingle();
            Container.Bind<SceneLoader>().AsSingle();

            Container.Bind<GameInput>()
                .FromMethod(_ =>
                {
                    var input = new GameInput();
                    input.Enable();
                    input.Gameplay.Enable();
                    return input;
                })
                .AsSingle()
                .NonLazy();

            InstallUiRoot();
            
            Container.BindInterfacesTo<StartupLoader>()
                .AsSingle()
                .NonLazy();
        }

        private void InstallUiRoot()
        {
            Container.Bind<UIRootView>()
                .FromComponentInNewPrefab(_uiRootPrefab)
                .AsSingle()
                .OnInstantiated<UIRootView>((ctx, ui) =>
                {
                    var sceneLoader = ctx.Container.Resolve<SceneLoader>();
                    ui.Initialize(sceneLoader);

                })
                .NonLazy();
        }
    }
}