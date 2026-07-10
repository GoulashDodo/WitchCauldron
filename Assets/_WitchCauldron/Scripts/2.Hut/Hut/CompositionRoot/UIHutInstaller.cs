using Hut.UI;
using UnityEngine;
using Zenject;

namespace Hut.CompositionRoot
{
    public class UIHutInstaller : MonoInstaller
    {
        [SerializeField] private UIHutRootBinder _hutRootBinderPf;
        
        public override void InstallBindings()
        {
            Container.Bind<UIHutRootBinder>()
                .FromComponentInNewPrefab(_hutRootBinderPf)
                .AsSingle()
                .NonLazy();
        }
    }
}