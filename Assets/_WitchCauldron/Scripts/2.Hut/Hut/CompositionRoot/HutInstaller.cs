using Hut.SelectedItems;
using Hut.Shop;
using UnityEngine;
using Zenject;

namespace Hut.CompositionRoot
{
    public class HutInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SelectedItemsRuntime>().AsSingle().NonLazy();
            Container.Bind<SelectedFamiliarRuntime>().AsSingle().NonLazy();
            Container.Bind<ShopService>().AsSingle().NonLazy();
        }
    }
}
