using UnityEngine;
using WitchCauldron.Scripts.Core.GameRoot.View;
using WitchCauldron.Scripts.Feature.Gameplay.Brewing.ScriptableObjects;
using WitchCauldron.Scripts.Feature.Gameplay.Brewing.Services;
using WitchCauldron.Scripts.Feature.Gameplay.Clickable;
using WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Services;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Services;
using WitchCauldron.Scripts.Feature.Gameplay.UI;
using Zenject;

namespace WitchCauldron.Scripts.Core.GameRoot.Root.CompositionRoot.Gameplay.Registrations
{
    public sealed class GameplayInstaller : MonoInstaller
    {
        [Header("Configs")]
        [SerializeField] private UIGameplayRootBinder _sceneRootBinderPrefab;
        [SerializeField] private PotionReceiptList _receiptList;

        public override void InstallBindings()
        {
            Container.BindInstance(_receiptList).AsSingle();


            Container.Bind<ReceiptService>().AsSingle();
            Container.Bind<BrewingService>().AsSingle();
            Container.Bind<CauldronService>().AsSingle();
            Container.Bind<PotionService>().AsSingle().NonLazy();
            Container.Bind<DraggableItemService>().AsSingle();

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
