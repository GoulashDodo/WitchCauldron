using Gameplay.Battle.BattleEntities.Enemies.Core;
using Gameplay.Battle.Effects;
using UnityEngine;

namespace Gameplay.UI.Enemies
{
    public class UIEnemyStatusView : MonoBehaviour
    {
        [SerializeField] private UIEnemyStatusEffects _effects;
        [SerializeField] private UIEnemyStatusHealthBar _healthBar;

        
        public RectTransform RectTransform { get; private set; }

        private void Awake()
        {
            RectTransform = (RectTransform)transform;
        }
        
        public void Initialize(Enemy enemy)
        {
            _healthBar.Initialize(enemy.Health);

            if (enemy.TryGetComponent(out EffectReceiver effectReceiver))
            {
                _effects.Initialize(effectReceiver);
            }
            
        }
        
    }
}