using _WitchCauldron.Scripts.Core.GameRoot.View;
using _WitchCauldron.Scripts.Feature.Gameplay.UI;
using UnityEngine;
using Zenject;

namespace _WitchCauldron.Scripts.Core.GameRoot._Root.CompositionRoot.Gameplay.Registrations
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