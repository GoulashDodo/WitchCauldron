using _WitchCauldron.Scripts.Feature.Gameplay.Battle.Model;
using _WitchCauldron.Scripts.Feature.Gameplay.HealthSystem;
using R3;
using TMPro;
using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.UI
{
    public class UIBaseHealth : MonoBehaviour
    {
        
        private CompositeDisposable _disposables = new();
        

        [SerializeField] private TextMeshProUGUI _text;
        

        private Base _baseInstance;
        
        
        
        
        public void Initialize(Base baseInstance)
        {
            _baseInstance = baseInstance;
            baseInstance.Health.CurrentHealth.Subscribe(UpdateText).AddTo(_disposables);
        }


        private void OnDestroy()
        {
            _disposables.Dispose();
        }
        
        
        private void UpdateText(float currentHealth)
        {
            _text.text = "Base:" + currentHealth;
        }
        
        
    }
}