using Core.Audio;
using Core.Run;
using Core.SceneManagement;
using Core.UI;
using Gameplay._root.SO;
using Hut.SelectedItems;
using Hut.SO;
using Hut.Shop;
using Hut.UI.UIAlmanac;
using Hut.UI.UIShop;
using Hut.UI.UISelectItems;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Hut.UI
{
    public class UIHutRootBinder : MonoBehaviour
    {
        
        [SerializeField] private UINextLevel _nextLevel;
        [SerializeField] private UIRunCompleted _runCompleted;   
        [SerializeField] private UISelectItemParent _selectItemParent;
        [SerializeField] private UIShowSelectItemsButton _showSelectItemsButton;
        [SerializeField] private UIWalletView _walletView;
        [SerializeField] private UIShopParent _shopParent;
        [SerializeField] private UIAlmanacRoot _almanacRoot;
        [SerializeField] private Button _showAlmanacButton;

        private void OnDestroy()
        {
            if (_showAlmanacButton != null && _almanacRoot != null)
                _showAlmanacButton.onClick.RemoveListener(_almanacRoot.Show);
        }
        
        [Inject]
        public void Construct(
            UIRootView view,
            RunState runState,
            SceneLoader sceneLoader,
            SelectedItemsRuntime selectedItemsRuntime,
            ShopService shopService,
            HutSettings hutSettings,
            GameplaySettings gameplaySettings,
            AudioService audioService)
        {

            view.AttachSceneUI(gameObject);
            
            _nextLevel.Initialize(runState, sceneLoader, selectedItemsRuntime);
            _showSelectItemsButton.Initialize(runState);
            _runCompleted.Initialize(runState);
            _selectItemParent.Initialize(selectedItemsRuntime, runState, audioService);

         

            if (_walletView != null)
                _walletView.Initialize(runState);

            if (_shopParent != null)
                _shopParent.Initialize(hutSettings.ShopUpgrades, shopService, runState);

            if (_almanacRoot != null)
                _almanacRoot.Initialize(gameplaySettings, runState);

            if (_showAlmanacButton != null && _almanacRoot != null)
                _showAlmanacButton.onClick.AddListener(_almanacRoot.Show);
        }
        
    }
}
