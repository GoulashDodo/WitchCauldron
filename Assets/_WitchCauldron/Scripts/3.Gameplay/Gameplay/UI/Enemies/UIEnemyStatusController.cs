using System.Collections.Generic;
using Gameplay._root.SO;
using Gameplay.Battle.BattleEntities.Enemies.Core;
using Gameplay.Battle.BattleEntities.Enemies.Services;
using Gameplay.Battle.HealthSystem.Structs;
using R3;
using TMPro;
using UnityEngine;
using Zenject;

namespace Gameplay.UI.Enemies
{
    public class UIEnemyStatusController : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _statusLayer;
        [SerializeField] private UIEnemyStatusView _statusViewPrefab;
        [SerializeField] private UIDamageText _damageTextPrefab;
        [SerializeField] private Camera _worldCamera;
        
        private readonly CompositeDisposable _disposables = new();
        
        private readonly Dictionary<Enemy, UIEnemyStatusView> _views = new();
        private readonly Dictionary<Enemy, System.IDisposable> _damageSubscriptions = new();
        private AllDamageTypeTextSettings _damageTypeTextSettings;

        [Inject]
        public void Construct(EnemyService enemyService, GameplaySettings gameplaySettings)
        {
            _damageTypeTextSettings = gameplaySettings.DamageTypeTextSettings;
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

            _damageSubscriptions.Add(enemy, enemy.Events.Damaged.Subscribe(damageInfo => ShowDamageText(enemy, damageInfo)));
        }

        private void RemoveView(Enemy enemy)
        {

            if (!_views.Remove(enemy, out var view))
                return;

            if (_damageSubscriptions.Remove(enemy, out var damageSubscription))
            {
                damageSubscription.Dispose();
            }
            
            Destroy(view.gameObject);
        }

        private void ShowDamageText(Enemy enemy, DamageInfo damageInfo)
        {
            if (!_views.TryGetValue(enemy, out var view))
                return;

            var damageText = CreateDamageText();
            damageText.transform.SetParent(_statusLayer, false);
            ((RectTransform)damageText.transform).anchoredPosition = view.RectTransform.anchoredPosition;

            var settings = _damageTypeTextSettings != null
                ? _damageTypeTextSettings.GetSettings(damageInfo.Type)
                : null;

            damageText.Play(damageInfo, settings);
        }

        private UIDamageText CreateDamageText()
        {
            if (_damageTextPrefab != null)
                return Instantiate(_damageTextPrefab);

            var damageTextObject = new GameObject("UIDamageText", typeof(RectTransform), typeof(CanvasGroup), typeof(TextMeshProUGUI), typeof(UIDamageText));
            var text = damageTextObject.GetComponent<TextMeshProUGUI>();
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;
            text.fontStyle = FontStyles.Bold;

            return damageTextObject.GetComponent<UIDamageText>();
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
            foreach (var damageSubscription in _damageSubscriptions.Values)
            {
                damageSubscription.Dispose();
            }

            _damageSubscriptions.Clear();
            _disposables.Dispose();
        }
        
    }
}
