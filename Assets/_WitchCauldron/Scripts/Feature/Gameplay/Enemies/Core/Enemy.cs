using System;
using _WitchCauldron.Scripts.Core.GameRoot.Data;
using _WitchCauldron.Scripts.Feature.Gameplay.Enemies.Core.Behaviours.Movement;
using _WitchCauldron.Scripts.Feature.Gameplay.Enemies.Core.Behaviours.Movement.SO;
using _WitchCauldron.Scripts.Feature.Gameplay.Enemies.Core.Behaviours.Movement.Strategy;
using _WitchCauldron.Scripts.Feature.Gameplay.Enemies.Services;
using _WitchCauldron.Scripts.Feature.Gameplay.Enemies.SO;
using _WitchCauldron.Scripts.Feature.Gameplay.HealthSystem;
using _WitchCauldron.Scripts.Feature.Gameplay.HealthSystem.Structs;
using R3;
using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Enemies.Core
{
    
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour, IDamageable
    {
        
        private readonly CompositeDisposable _disposables = new();
        
        
        private Rigidbody2D _rigidbody;


        public EnemySettings Settings { get; private set; }
        public EnemyEvents Events { get; private set; }
        
        private EnemyContext _context;
        
        
        
        private EnemyService _enemyService;
        private MovementController _movementController;    
    

        
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


            _context = new EnemyContext(
                gameObject: gameObject,
                transform: transform,
                rigidbody2D: _rigidbody,
                settings: Settings,
                events : Events
            );

            _context.InitializeCore();
            
            
            _context.Health.Damaged
                .Subscribe(damageInfo => Events.RaiseDamaged(damageInfo))
                .AddTo(_disposables);

            _context.Health.Died
                .Subscribe(deathInfo =>
                {
                    Events.RaiseDied(deathInfo);
                    Die(deathInfo);
                })
                .AddTo(_disposables);



            SetupMovement();
            
            Events.RaiseSpawned(this);
            
            _enemyService.RegisterEnemy(this);
            
            
        }
        
        private void SetupMovement()
        {
            var moveContext = new MoveContext(transform, _rigidbody)
            {
                Speed = Settings.MaxSpeed,
                StopDistance = Settings.AttackDistance
            };

            var moveConfig = Settings.MoveConfig;

            moveConfig.ConfigureContext(moveContext);
            var behaviour = moveConfig.CreateBehaviour();

            _movementController = new MovementController(moveContext);
            _movementController.SetBehaviour(behaviour);
        
        }
        
        #endregion
        
        private void FixedUpdate()
        {
            _movementController?.Tick(Time.fixedDeltaTime);
            
        }
        
        
        public void TakeDamage(float damage)
        {
            Debug.Log($"Taking damage {damage}");
            _context.Health.TakeDamage(damage);
        }

        private void Die(DeathInfo deathInfo)
        {
            _enemyService.UnregisterEnemy(this);
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _disposables.Dispose();
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