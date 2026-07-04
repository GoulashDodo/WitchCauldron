using R3;
using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Enemies.Core
{
    
    [RequireComponent(typeof(Animator))]
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] private bool _hasDeathAnimation = true;
        
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Death = Animator.StringToHash("Death");
        private static readonly int AttackSpeedMultiplier = Animator.StringToHash("AttackSpeedMultiplier");

        private readonly CompositeDisposable _disposables = new();

        private Enemy _enemy;
        private EnemyAttack _attack;
        private Animator _animator;
        private bool _hasAttackSpeedMultiplier;
        private bool _isDeathStarted;

        
        
        private void Awake()
        {
            _enemy = GetComponentInParent<Enemy>();
            _attack = GetComponentInParent<EnemyAttack>();
            _animator = GetComponent<Animator>();
            _hasAttackSpeedMultiplier = HasAnimatorParameter(AttackSpeedMultiplier);
        }

        private void Start()
        {
            _enemy.Events.AttackPerformed
                .Subscribe(_ => PlayAttack())
                .AddTo(_disposables);

            _enemy.Events.Died
                .Subscribe(_ => OnDied())
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void PlayAttack()
        {
            if (_isDeathStarted)
                return;

            if (_hasAttackSpeedMultiplier)
                _animator.SetFloat(AttackSpeedMultiplier, Mathf.Max(_enemy.Settings.AttackSpeed, 0.01f));
            
            _animator.SetTrigger(Attack);
        }
        
        private void OnDied()
        {
            if (_isDeathStarted)
                return;

            _isDeathStarted = true;

            if (!_hasDeathAnimation)
            {
                _enemy.CompleteDeath();
                return;
            }

            _animator.SetTrigger(Death);
        }

        public void OnDeathAnimationFinished()
        {
            _enemy.CompleteDeath();
        }

        public void OnAttackHit()
        {
            _attack.ApplyCurrentAttackHit();
        }

        private bool HasAnimatorParameter(int parameterHash)
        {
            if (_animator.runtimeAnimatorController == null)
                return false;

            foreach (var parameter in _animator.parameters)
            {
                if (parameter.nameHash == parameterHash)
                    return true;
            }

            return false;
        }
        
        
    }
}
