using System.Collections.Generic;
using Gameplay.Battle.BattleEntities.Enemies.Core;
using Gameplay.Battle.BattleEntities.Enemies.Services;
using R3;
using UnityEngine;
using Zenject;

namespace Gameplay.UI.Enemies
{
    public class UIEnemyStatusController : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _statusLayer;
        [SerializeField] private UIEnemyStatusView _statusViewPrefab;
        [SerializeField] private Camera _worldCamera;
        
        private CompositeDisposable _disposables = new();
        
        private readonly Dictionary<Enemy, UIEnemyStatusView> _views = new();

        [Inject]
        public void Construct(EnemyService enemyService)
        {
            Initialize(enemyService);
        }
        
        public void Initialize(EnemyService enemyService)
        {
            enemyService.EnemySpawned.Subscribe(CreateView).AddTo(_disposables);
            enemyService.EnemyDied.Subscribe(RemoveView).AddTo(_disposables);
        }

        private void CreateView(Enemy enemy)
        {
            if (_views.ContainsKey(enemy))
                return;
            
            var view = Instantiate(_statusViewPrefab, _statusLayer.transform);
            view.Initialize(enemy);
            _views.Add(enemy, view);
        }

        private void RemoveView(Enemy enemy)
        {

            if (!_views.Remove(enemy, out var view))
                return;
            
            Destroy(view.gameObject);
            _views.Remove(enemy);
        }

        private void LateUpdate()
        {
            foreach (var (enemy, view) in _views)
            {
                UpdateViewPosition(enemy, view);
            }
        }

        private void UpdateViewPosition(Enemy enemy, UIEnemyStatusView view)
        {
            var screenPoint = _worldCamera.WorldToScreenPoint(enemy.transform.position);

            if (screenPoint.z <= 0f)
            {
                view.gameObject.SetActive(false);
                return;
            }

            view.gameObject.SetActive(true);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _statusLayer,
                screenPoint,
                _canvas.worldCamera,
                out var localPoint
            );

            view.RectTransform.anchoredPosition = localPoint;        }


        private void OnDestroy()
        {
            _disposables.Dispose();
        }
        
    }
}