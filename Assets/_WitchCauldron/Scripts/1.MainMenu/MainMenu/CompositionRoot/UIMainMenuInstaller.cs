using MainMenu.UI;
using UnityEngine;
using Zenject;

namespace MainMenu.CompositionRoot
{
    public class UIMainMenuInstaller : MonoInstaller
    {

        [SerializeField] private UIMainMenuRootBinder _mainMenuRootBinderPrefab;
        
        public override void InstallBindings()
        {
            Container.Bind<UIMainMenuRootBinder>()
                .FromComponentInNewPrefab(_mainMenuRootBinderPrefab)
                .AsSingle()
                .NonLazy();
        }
    }
}
