using Feature.Gameplay.Battle.Base.Interfaces;
using Feature.Gameplay.Battle.HealthSystem.Core;
using R3;
using TMPro;
using UnityEngine;

namespace Feature.Gameplay.UI
{
    public class UIBaseHealth : MonoBehaviour
    {
        
        private readonly CompositeDisposable _disposables = new();
        

        [SerializeField] private TextMeshProUGUI _text;
        

        private IHealth _health;
        
        
        public void Initialize(IBaseHealthProvider healthProvider)
        {
            _health = healthProvider.GetBaseHealth();
            _health.CurrentHealth.Subscribe(UpdateText).AddTo(_disposables);
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
