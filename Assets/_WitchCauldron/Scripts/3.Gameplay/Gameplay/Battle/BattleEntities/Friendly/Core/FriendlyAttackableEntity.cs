using Core.Data;
using Gameplay.Battle.HealthSystem;
using Gameplay.Battle.HealthSystem.Core;
using Gameplay.Battle.HealthSystem.Structs;
using R3;
using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Friendly.Core
{
    public class FriendlyAttackableEntity : MonoBehaviour, IDamageable, IEnemyAttackTarget
    {
        [SerializeField] private float _maxHealth = 10f;

        private readonly CompositeDisposable _disposables = new();

        private Health _health;

        public IHealth Health => _health;
        public IDamageable Damageable => this;
        public bool IsDead { get; private set; }

        private void Awake()
        {
            _health = new Health(Mathf.Max(1f, _maxHealth));
            _health.Died
                .Subscribe(_ => Die())
                .AddTo(_disposables);

            EnsureEnemyAttackRaycastTarget();
        }

        public void TakeDamage(BattleDamage battleDamage)
        {
            if (IsDead)
                return;

            _health.TakeDamage(battleDamage);
        }

        private void Die()
        {
            if (IsDead)
                return;

            IsDead = true;
            Destroy(gameObject);
        }

        private void EnsureEnemyAttackRaycastTarget()
        {
            if (!TryGetComponent<Collider2D>(out _))
            {
                var attackCollider = gameObject.AddComponent<BoxCollider2D>();
                attackCollider.isTrigger = true;
            }

            var baseLayer = LayerMask.NameToLayer(Layers.Base);
            if (baseLayer >= 0 && gameObject.layer == 0)
            {
                gameObject.layer = baseLayer;
            }
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
            _health?.Dispose();
        }
    }
}
