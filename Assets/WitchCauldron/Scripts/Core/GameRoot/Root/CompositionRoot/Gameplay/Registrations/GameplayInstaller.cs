using UnityEngine;
using WitchCauldron.Scripts.Core.GameRoot.View;
using WitchCauldron.Scripts.Feature.Gameplay.Clickable;
using WitchCauldron.Scripts.Feature.Gameplay.Combination.ScriptableObjects;
using WitchCauldron.Scripts.Feature.Gameplay.Combination.Service;
using WitchCauldron.Scripts.Feature.Gameplay.Items.Services;
using WitchCauldron.Scripts.Feature.Gameplay.Items.Settings;
using WitchCauldron.Scripts.Feature.Gameplay.Items.Usable.Commands.Processor;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Services;
using WitchCauldron.Scripts.Feature.Gameplay.UI;
using Zenject;

namespace WitchCauldron.Scripts.Core.GameRoot.Root.CompositionRoot.Gameplay.Registrations
{
    public sealed class GameplayInstaller : MonoInstaller
    {
        [Header("Configs")]
        [SerializeField] private UIGameplayRootBinder _sceneRootBinderPrefab;
        [SerializeField] private AllItemSettings _allItemSettings;
        [SerializeField] private CombinationRuleList  _combinationRuleList;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_allItemSettings);
            Container.BindInstance(_combinationRuleList);
    
            
            Container.Bind<IUseCommandProcessor>().To<UseCommandProcessor>().AsSingle();
            
            Container.Bind<PotionService>().AsSingle().NonLazy();
            Container.Bind<CombinationService>().AsSingle().NonLazy();
            Container.Bind<ItemService>().AsSingle();

            Container.Bind<MouseClickHandler>()
                .FromMethod(_ => new MouseClickHandler(Container.Resolve<GameInput>()))
                .AsSingle()     
                .NonLazy();

            
       
            
            
            InstallSceneUI();
            
        }

        private void InstallSceneUI()
        {
            Container.Bind<UIGameplayRootBinder>()
                .FromComponentInNewPrefab(_sceneRootBinderPrefab)
                .AsSingle()
                .OnInstantiated<UIGameplayRootBinder>((ctx, binder) =>
                {
                    var uiRoot = ctx.Container.Resolve<UIRootView>();
                    uiRoot.AttachSceneUI(binder.gameObject);

                    binder.InitializeUI(ctx.Container);
                })
                .NonLazy();
        }
     
    }
}
