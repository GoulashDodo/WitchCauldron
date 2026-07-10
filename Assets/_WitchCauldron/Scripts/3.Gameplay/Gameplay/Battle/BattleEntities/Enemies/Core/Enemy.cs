using Core.Audio;
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
        
        
        private Health _health;


        public EnemySettings Settings { get; private set; }
        public EnemyEvents Events { get; } = new();
        public IHealth Health => _health;
        
        public bool IsDead { get; private set; }
        
        public bool IsInitialized => Settings != null;

        private EnemyService _enemyService;
        private AudioService _audioService;
    

        
        #region CREATION
        
        public void Construct(EnemyService enemyService, EnemySettings enemySettings, AudioService audioService)
        {
            _enemyService = enemyService;
            _audioService = audioService;

            Settings = enemySettings;
            

            _health = new Health(Settings.MaxHealth);

            _health.Damaged
                .Subscribe(damageInfo => Events.RaiseDamaged(damageInfo))
                .AddTo(_disposables);

            _health.Died
                .Subscribe(Die)
                .AddTo(_disposables);


            Events.RaiseSpawned(this);
            
            _enemyService.RegisterEnemy(this);
        }
        
        #endregion
        
        
        public void TakeDamage(BattleDamage battleDamage)
        {
            if (IsDead)
                return;

            Debug.Log($"Taking damage {battleDamage.Amount}");
            _audioService?.PlaySfx(AudioId.Enemy_Hit, transform.position);
            _health.TakeDamage(battleDamage);
        }

        
        private void Die(DeathInfo deathInfo)
        {
            if (IsDead)
                return;

            IsDead = true;

            _audioService?.PlaySfx(AudioId.Enemy_Death, transform.position);
            Events.RaiseDied(deathInfo);
            _enemyService.UnregisterEnemy(this);
        }

        public void CompleteDeath()
        {
            if (!IsDead)
                return;

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
