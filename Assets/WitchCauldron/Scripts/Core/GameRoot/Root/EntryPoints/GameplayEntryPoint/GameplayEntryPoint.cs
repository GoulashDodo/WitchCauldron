using UnityEngine;
using WitchCauldron.Scripts.Core.GameRoot.Root.EntryPoints.GameplayEntryPoint.Registrations;
using WitchCauldron.Scripts.Core.GameRoot.View;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;
using WitchCauldron.Scripts.Feature.Gameplay.UI;
using Zenject;

namespace WitchCauldron.Scripts.Core.GameRoot.Root.EntryPoints.GameplayEntryPoint
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
            
        }

    }
}
