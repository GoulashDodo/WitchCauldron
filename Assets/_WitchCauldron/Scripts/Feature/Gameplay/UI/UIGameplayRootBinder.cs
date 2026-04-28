using _WitchCauldron.Scripts.Core.GameRoot.State.Providers;
using _WitchCauldron.Scripts.Core.GameRoot.View;
using _WitchCauldron.Scripts.Feature.Gameplay.Battle.Model;
using UnityEngine;
using Zenject;

namespace _WitchCauldron.Scripts.Feature.Gameplay.UI
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
