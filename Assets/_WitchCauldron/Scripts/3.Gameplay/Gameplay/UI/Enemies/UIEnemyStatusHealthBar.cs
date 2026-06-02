using Gameplay.Battle.HealthSystem.Core;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Enemies
{
    public class UIEnemyStatusHealthBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        public void Initialize(IHealth health)
        {
            _slider.minValue = 0f;
            _slider.maxValue = health.MaxHealth;
            
            UpdateView(health.CurrentHealthValue);

            health.CurrentHealth
                .Subscribe(UpdateView)
                .AddTo(_disposables);
            
        }


        private void UpdateView(float currentHealth)
        {
            _slider.value = currentHealth;
        }
        
        private void OnDestroy()
        {
            _disposables.Dispose();
        }
        
    }
}