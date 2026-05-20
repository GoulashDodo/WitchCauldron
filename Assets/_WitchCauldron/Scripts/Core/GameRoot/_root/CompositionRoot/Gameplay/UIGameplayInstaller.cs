using Feature.Gameplay.UI;
using UnityEngine;
using Zenject;

namespace Core.GameRoot._root.CompositionRoot.Gameplay
{
    public class UIGameplayInstaller : MonoInstaller
    {
        [SerializeField] private UIGameplayRootBinder _sceneRootBinderPrefab;



        public override void InstallBindings()
        {
            Container.Bind<UIGameplayRootBinder>()
                .FromComponentInNewPrefab(_sceneRootBinderPrefab)
                .AsSingle()
                .NonLazy();
        }
    }
}