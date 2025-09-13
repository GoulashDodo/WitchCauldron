using UnityEngine;
using WitchCauldron.Scripts.Common.Utilits.Debugging;
using WitchCauldron.Scripts.Core.GameRoot.Root.CompositionRoot.Abstract;
using WitchCauldron.Scripts.Core.GameRoot.Root.CompositionRoot.Gameplay.Registrations;
using WitchCauldron.Scripts.Core.GameRoot.View;
using WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Services;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Services;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Spawners;
using WitchCauldron.Scripts.Feature.Gameplay.UI;
using Zenject;

namespace WitchCauldron.Scripts.Core.GameRoot.Root.CompositionRoot.Gameplay.Root
{
    public class GameplayEntryPoint : SceneEntryPoint
    {
        
        [SerializeField] private UIGameplayRootBinder _sceneRootBinderPrefab;
        
        [SerializeField] private PotionReceiptList _receipts;
        
        public override void Run(DiContainer sceneContainer)
        {
            var uiRoot = sceneContainer.Resolve<UIRootView>();
            var sceneRootBinder = Instantiate(_sceneRootBinderPrefab);
            uiRoot.AttachSceneUI(sceneRootBinder.gameObject);
            
            sceneRootBinder.InitializeUI(sceneContainer);
            
            
            GameplayRegistrations.Register(sceneContainer, _receipts);
            
            
#if UNITY_EDITOR
            InitializeDebugInvokes(sceneContainer);
#endif
            
            
        }
        
        private void InitializeDebugInvokes(DiContainer sceneContainer)
        {
            var debugReceiptService = Object.FindFirstObjectByType<DebugReceiptServiceInvoker>();
            
            debugReceiptService.Initialize(sceneContainer.Resolve<ReceiptService>());
            
            var itemSpawner = FindFirstObjectByType<ItemSpawner>();
            
            itemSpawner.Initialize(sceneContainer.Resolve<DraggableItemService>());
            
        }
        

    }
}
