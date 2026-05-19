using Core.GameRoot.Input.Clickable;
using Core.GameRoot.View;
using UnityEngine;
using Zenject;

namespace Core.GameRoot._root.CompositionRoot.Game.Registrations
{
    public class GlobalInstaller : MonoInstaller
    {
        [Header("UI Root")]
        [SerializeField] private UIRootView _uiRootPrefab;

        public override void InstallBindings()
        {
            Container.Bind<SceneParametersPayload>().AsSingle();
            
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

            
            Container.BindInterfacesAndSelfTo<MouseClickHandler>()
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