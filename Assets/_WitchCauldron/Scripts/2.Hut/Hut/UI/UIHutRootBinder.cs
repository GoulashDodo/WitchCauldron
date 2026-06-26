using Core.Run;
using Core.SceneManagement;
using Core.UI;
using Hut.SelectedItems;
using Hut.SelectFamiliars;
using Hut.UI.UISelectItems;
using UnityEngine;
using Zenject;

namespace Hut.UI
{
    public class UIHutRootBinder : MonoBehaviour
    {
        
        [SerializeField] private UINextLevel _nextLevel;
        [SerializeField] private UIRunCompleted _runCompleted;   
        [SerializeField] private UISelectItemParent _selectItemParent;
        [SerializeField] private UIShowSelectItemsButton _showSelectItemsButton;
        
        [Inject]
        public void Construct(
            UIRootView view,
            RunState runState,
            SceneLoader sceneLoader,
            SelectedItemsRuntime selectedItemsRuntime)
        {

            view.AttachSceneUI(gameObject);
            
            _nextLevel.Initialize(runState, sceneLoader, selectedItemsRuntime);
            _showSelectItemsButton.Initialize(runState);
            _runCompleted.Initialize(runState);
            _selectItemParent.Initialize(selectedItemsRuntime, runState);

        }
        
    }
}
