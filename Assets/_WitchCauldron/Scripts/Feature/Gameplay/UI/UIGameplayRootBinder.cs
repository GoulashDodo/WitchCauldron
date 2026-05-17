using Core.GameRoot.View;
using Feature.Gameplay.Battle.Model;
using UnityEngine;
using Zenject;

namespace Feature.Gameplay.UI
{
    public class UIGameplayRootBinder : MonoBehaviour
    {

        
        
        [SerializeField] private UIBaseHealth _uiBaseHealth;
        
        
    
        
        [Inject]
        public void Construct(UIRootView view, Base baseInstance)
        {
            view.AttachSceneUI(gameObject);
            
            InitializeUI(baseInstance);
            
        }
        

        public void InitializeUI(Base baseInstance)
        {
            
            _uiBaseHealth.Initialize(baseInstance);
            
            
        }
        
        
    }
}
