using Core.GameRoot.View;
using UnityEngine;
using Zenject;

namespace Feature.MainMenu.UI
{
    public class UIMainMenuRootBinder : MonoBehaviour
    {
        
        [Inject]
        public void Construct(UIRootView view)
        {
            view.AttachSceneUI(gameObject);
        }
        
    }
}