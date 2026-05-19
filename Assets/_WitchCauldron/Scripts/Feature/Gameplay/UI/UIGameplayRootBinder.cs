using Core.GameRoot.View;
using Feature.Gameplay.Battle.Base.Interfaces;
using Feature.Gameplay.Battle.HealthSystem.Core;
using UnityEngine;
using Zenject;

namespace Feature.Gameplay.UI
{
    public class UIGameplayRootBinder : MonoBehaviour
    {

        
        
        [SerializeField] private UIBaseHealth _uiBaseHealth;
        
        
    
        
        [Inject]
        public void Construct(UIRootView view, IBaseHealthProvider baseHealthProvider)
        {
            view.AttachSceneUI(gameObject);
            
            InitializeUI(baseHealthProvider);
            
        }


        private void InitializeUI(IBaseHealthProvider baseHealthProvider)
        {
            
            _uiBaseHealth.Initialize(baseHealthProvider);
            
            
        }
        
        
    }
}
