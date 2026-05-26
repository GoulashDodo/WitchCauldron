using Core.Data;
using Gameplay.Battle.BattleEntities.Enemies.Services;
using Gameplay.Battle.BattleEntities.Enemies.SO;
using Gameplay.Battle.HealthSystem;
using Gameplay.Battle.HealthSystem.Core;
using Gameplay.Battle.HealthSystem.Structs;
using R3;
using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Enemies.Core
{
    
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour, IDamageable
    {
        
        private readonly CompositeDisposable _disposables = new();
        
        
        private Rigidbody2D _rigidbody;
        private Health _health;
        private LayerMask _baseLayerMask;
        private float _nextAttackTime;


        public EnemySettings Settings { get; private set; }
        public EnemyEvents Events { get; private set; }

        private EnemyService _enemyService;
    

        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }
        
        #region CREATION
        
        public void Construct(EnemyService enemyService, EnemySettings enemySettings)
        {
            _enemyService = enemyService;

            Settings = enemySettings;
            

            Events = new EnemyEvents();
            _health = new Health(Settings.MaxHealth);
            _baseLayerMask = LayerMask.GetMask(Layers.Base);

            _health.Damaged
                .Subscribe(damageInfo => Events.RaiseDamaged(damageInfo))
                .AddTo(_disposables);

            _health.Died
                .Subscribe(deathInfo =>
                {
                    Events.RaiseDied(deathInfo);
                    Die(deathInfo);
                })
                .AddTo(_disposables);


            Events.RaiseSpawned(this);
            
            _enemyService.RegisterEnemy(this);
        }
        
        #endregion
        
        private void FixedUpdate()
        {
            if (Settings == null)
                return;

            var target = FindBaseInAttackRange();

            if (target != null)
            {
                TryAttack(target);
                return;
            }

            MoveLeft(Time.fixedDeltaTime);
        }
        
        
        public void TakeDamage(BattleDamage battleDamage)
        {
            Debug.Log($"Taking damage {battleDamage.Amount}");
            _health.TakeDamage(battleDamage);
        }

        private IDamageable FindBaseInAttackRange()
        {
            var hit = Physics2D.Raycast(
                origin: transform.position,
                direction: Vector2.left,
                distance: Settings.AttackDistance,
                layerMask: _baseLayerMask);

            return hit.collider != null
                ? hit.collider.GetComponentInParent<IDamageable>()
                : null;
        }

        private void TryAttack(IDamageable target)
        {
            if (Time.time < _nextAttackTime)
                return;

            target.TakeDamage(new BattleDamage(Settings.Damage, DamageType.Physical));
            _nextAttackTime = Time.time + GetAttackCooldown();
        }

        private float GetAttackCooldown()
        {
            return 1f / Mathf.Max(Settings.AttackSpeed, 0.01f);
        }

        private void MoveLeft(float deltaTime)
        {
            var nextPosition = _rigidbody.position + Vector2.left * (Settings.MaxSpeed * deltaTime);
            _rigidbody.MovePosition(nextPosition);
        }

        private void Die(DeathInfo deathInfo)
        {
            _enemyService.UnregisterEnemy(this);
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _disposables.Dispose();
            _health?.Dispose();
        }

        private void OnDrawGizmosSelected()
        {
            if (Settings != null)
            {
                Gizmos.DrawLine(transform.position, new Vector3(transform.position.x - Settings.AttackDistance, transform.position.y));
            }
        }
        
    }
}
