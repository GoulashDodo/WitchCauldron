using R3;
using UnityEngine;

namespace Gameplay.Items.Spawners
{
    
    [RequireComponent(typeof(ItemSpawner))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemSpawnerFx : MonoBehaviour
    {

        private readonly CompositeDisposable _disposables = new();
        
        private ItemSpawner _spawner;
        
        private SpriteRenderer _spriteRenderer;

        [SerializeField, Range(0f, 1f)] private float _cooldownAlpha = 0.4f;
        
        private void Awake()
        {
            _spawner = GetComponent<ItemSpawner>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _spawner.ItemSpawned
                .Subscribe(_ => SetCooldownView())
                .AddTo(_disposables);

            _spawner.CooldownRestored
                .Subscribe(_ => SetReadyView())
                .AddTo(_disposables);

            if (_spawner.CanSpawn)
                SetReadyView();
            else
                SetCooldownView();
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void SetReadyView()
        {
            SetAlpha(1f);
        }

        private void SetCooldownView()
        {
            SetAlpha(_cooldownAlpha);
        }

        private void SetAlpha(float alpha)
        {
            var color = _spriteRenderer.color;
            color.a = alpha;
            _spriteRenderer.color = color;
        }
        
        
    }
}
