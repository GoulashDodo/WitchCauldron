using Hut.SelectedItems;
using UnityEngine;
using Zenject;

namespace Hut.CompositionRoot
{
    public class HutInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SelectedItemsRuntime>().AsSingle().NonLazy();
        }
    }
}