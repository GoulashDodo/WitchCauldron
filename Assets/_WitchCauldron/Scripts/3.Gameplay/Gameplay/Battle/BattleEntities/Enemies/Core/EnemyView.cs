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

        private readonly CompositeDisposable _disposables = new();

        private Enemy _enemy;
        private Animator _animator;

        
        
        private void Awake()
        {
            _enemy = GetComponentInParent<Enemy>();
            _animator = GetComponent<Animator>();
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
            _animator.SetTrigger(Attack);
        }
        
        private void OnDied()
        {
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
        
        
    }
}
