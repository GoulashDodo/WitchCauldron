using Feature.MainMenu.UI;
using UnityEngine;
using Zenject;

namespace Core.GameRoot._root.CompositionRoot.MainMenu
{
    public class UIMainMenuRootInstaller : MonoInstaller
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
