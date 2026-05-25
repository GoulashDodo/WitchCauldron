using Core.View;
using UnityEngine;
using Zenject;

namespace MainMenu.UI
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