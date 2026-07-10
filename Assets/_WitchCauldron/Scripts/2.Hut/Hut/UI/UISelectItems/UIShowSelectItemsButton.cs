using Core.Run;
using UnityEngine;

namespace Hut.UI.UISelectItems
{
    public class UIShowSelectItemsButton : MonoBehaviour
    {
        private RunState _runState;
        
        public void Initialize(RunState runState)
        {
            _runState = runState;
            RefreshState();
        }
        
        private void RefreshState()
        {
            var canStartNextLevel = _runState.HasCurrentLevel;
            gameObject.SetActive(canStartNextLevel);
        }
        
    }
}
