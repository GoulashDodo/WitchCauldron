using Core.Run;
using Core.SceneManagement;
using Core.UI;
using UnityEngine;
using Zenject;

namespace Hut.UI
{
    public class UIHutRootBinder : MonoBehaviour
    {
        
        [SerializeField] private UINextLevel _nextLevel;
        

        [Inject]
        public void Construct(UIRootView view, RunState runState, SceneLoader sceneLoader)
        {

            view.AttachSceneUI(gameObject);
            
            _nextLevel.Initialize(runState, sceneLoader);

        }
        
    }
}
