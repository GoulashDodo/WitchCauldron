using Gameplay.Items.MonoBehaviours.View;
using Gameplay.Items.Services;
using UnityEngine;
using Zenject;

namespace Gameplay.Battle.BattleEntities.Friendly.Spawners
{
    public class ItemDropSpawner : MonoBehaviour
    {
        [SerializeField] private string _itemToSpawnTypeId;
        [SerializeField, Min(0.01f)] private float _spawnInterval = 5f;
        [SerializeField] private bool _spawnOnStart;
        [SerializeField] private Vector2 _dropXOffsetRange = new(-0.35f, 0.35f);
        [SerializeField] private Vector2 _dropYOffsetRange = new(0.1f, 0.45f);

        private ItemService _itemService;
        private float _nextSpawnTime;

        [Inject]
        public void Initialize(ItemService itemService)
        {
            _itemService = itemService;
        }

        private void OnEnable()
        {
            _nextSpawnTime = Time.time + (_spawnOnStart ? 0f : GetSpawnInterval());
        }

        private void Update()
        {
            if (Time.time < _nextSpawnTime)
                return;

            SpawnItem();
            _nextSpawnTime = Time.time + GetSpawnInterval();
        }

        private void SpawnItem()
        {
            if (_itemService == null)
            {
                Debug.LogWarning($"{nameof(ItemDropSpawner)} on '{name}' was not injected.");
                return;
            }

            var startPosition = transform.position;
            if (!_itemService.TrySpawnDraggableItem(_itemToSpawnTypeId, startPosition, out var item))
                return;

            var dropFx = item.GetComponent<LootDropFx>();
            if (dropFx == null)
                dropFx = item.gameObject.AddComponent<LootDropFx>();

            dropFx.Play(startPosition, GetDropPosition(startPosition));
        }

        private Vector3 GetDropPosition(Vector3 origin)
        {
            return origin + new Vector3(
                Random.Range(_dropXOffsetRange.x, _dropXOffsetRange.y),
                Random.Range(_dropYOffsetRange.x, _dropYOffsetRange.y),
                0f);
        }

        private float GetSpawnInterval()
        {
            return Mathf.Max(0.01f, _spawnInterval);
        }
    }
}
