using System;
using System.Collections.Generic;
using Core.Input.Clickable;
using Gameplay.Battle.BattleEntities.Friendly.Core;
using Gameplay.Items.Services;
using Gameplay.Items.SO;
using Gameplay.Items.Usable.Commands;
using Gameplay.Items.Usable.Commands.Preview;
using Gameplay.Items.Usable.Commands.Spawn;
using R3;
using UnityEngine;

namespace Gameplay.Items.MonoBehaviours
{
    [RequireComponent(typeof(Collider2D))]
    public class ItemSpawnGhost : MonoBehaviour, ILeftButtonReleasable, IDisposable
    {
        private readonly Collider2D[] _overlapBuffer = new Collider2D[8];
        private readonly List<PreviewInstance> _previews = new();

        private ItemSettings _settings;
        private ItemService _itemService;
        private IUseCommandPreviewProcessor _previewProcessor;
        private IDisposable _positionSubscription;
        private Action<bool> _completed;
        private Collider2D _collider;
        private SpriteRenderer[] _renderers;
        private bool[] _rendererEnabledStates;
        private bool _isCompleted;
        private bool _showingSpawnBlockedRadii;

        public void Initialize(
            ItemSettings settings,
            ItemService itemService,
            Observable<Vector2> mouseWorldPosition,
            IUseCommandPreviewProcessor previewProcessor,
            Action<bool> completed)
        {
            _settings = settings;
            _itemService = itemService;
            _previewProcessor = previewProcessor;
            _completed = completed;
            _collider = GetComponent<Collider2D>();
            _renderers = GetComponentsInChildren<SpriteRenderer>();
            _rendererEnabledStates = new bool[_renderers.Length];
            CacheRendererStates();
            ShowPreviews();

            _positionSubscription = mouseWorldPosition.Subscribe(position =>
            {
                if (!_isCompleted)
                {
                    transform.position = position;
                    UpdatePreviews();
                }
            });
        }

        public void OnLeftButtonReleased(Vector3 mousePosition)
        {
            if (_isCompleted)
                return;

            transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);

            var accepted = TryPlace();
            Complete(accepted);
        }

        private bool TryPlace()
        {
            if (_settings == null || _itemService == null)
                return false;

            if (ItemPlacementQuery.IsInCombineZone(_collider, transform, _overlapBuffer))
            {
                var spawnedItem = _itemService.SpawnDraggableItem(_settings.TypeId, transform.position);
                TryCombineSpawnedItem(spawnedItem);
                return true;
            }

            if (!ItemPlacementQuery.CanUseOnBattleground(_collider, transform, _overlapBuffer))
                return false;

            if (!_itemService.CanUseItemAt(_settings, transform.position))
                return false;

            var item = _itemService.SpawnDraggableItem(_settings.TypeId, transform.position);
            if (item is UsableItem usableItem && _itemService.TryUseItem(usableItem, transform.position))
                return true;

            item.Dispose();
            Destroy(item.gameObject);
            return false;
        }

        private void TryCombineSpawnedItem(DraggableItem item)
        {
            if (item is not CombinableItem combinableItem)
                return;

            var itemCollider = item.GetComponent<Collider2D>();
            var best = ItemPlacementQuery.FindBestOverlappedCombinableItem(
                itemCollider,
                item.transform,
                _overlapBuffer,
                out var count);

            if (best != null)
                _itemService.TryCombineItems(combinableItem, best);

            Array.Clear(_overlapBuffer, 0, count);
        }

        private void ShowPreviews()
        {
            HidePreviews();

            if (_previewProcessor == null || _settings?.OnUseCommands == null)
                return;

            var position = (Vector2)transform.position;

            foreach (var command in _settings.OnUseCommands)
            {
                var preview = _previewProcessor.CreatePreview(command, position, _settings);
                if (preview == null)
                    continue;

                _previews.Add(new PreviewInstance(command, preview));
            }

            UpdatePreviews();

            if (_previews.Count > 0 && HasSpawnCommand())
                ShowSpawnBlockedRadii();
        }

        private void UpdatePreviews()
        {
            if (_previews.Count == 0 || _previewProcessor == null)
                return;

            var inCombineZone = ItemPlacementQuery.IsInCombineZone(_collider, transform, _overlapBuffer);
            var canUseOnBattleground = ItemPlacementQuery.CanUseOnBattleground(_collider, transform, _overlapBuffer);
            var position = (Vector2)transform.position;
            var canUseHere = canUseOnBattleground &&
                             _itemService != null &&
                             _itemService.CanUseItemAt(_settings, position);
            var visible = canUseHere && !inCombineZone;
            var showGhostSprite = !visible && (!canUseOnBattleground || inCombineZone);

            foreach (var preview in _previews)
            {
                if (preview.GameObject == null)
                    continue;

                preview.GameObject.SetActive(visible);
                _previewProcessor.UpdatePreview(preview.GameObject, preview.Command, position, _settings);
            }

            SetGhostRenderersVisible(showGhostSprite);
        }

        private void HidePreviews()
        {
            foreach (var preview in _previews)
            {
                if (preview.GameObject != null)
                    Destroy(preview.GameObject);
            }

            _previews.Clear();
            HideSpawnBlockedRadii();
            RestoreRenderers();
        }

        private bool HasSpawnCommand()
        {
            if (_settings?.OnUseCommands == null)
                return false;

            foreach (var command in _settings.OnUseCommands)
            {
                if (command is SpawnCommandParameters)
                    return true;
            }

            return false;
        }

        private void ShowSpawnBlockedRadii()
        {
            if (_showingSpawnBlockedRadii)
                return;

            _showingSpawnBlockedRadii = true;
            FriendlyAttackableEntity.BeginSpawnPlacementPreview();
        }

        private void HideSpawnBlockedRadii()
        {
            if (!_showingSpawnBlockedRadii)
                return;

            _showingSpawnBlockedRadii = false;
            FriendlyAttackableEntity.EndSpawnPlacementPreview();
        }

        private void CacheRendererStates()
        {
            for (var i = 0; i < _renderers.Length; i++)
                _rendererEnabledStates[i] = _renderers[i] != null && _renderers[i].enabled;
        }

        private void SetGhostRenderersVisible(bool visible)
        {
            for (var i = 0; i < _renderers.Length; i++)
            {
                var itemRenderer = _renderers[i];
                if (itemRenderer == null)
                    continue;

                itemRenderer.enabled = visible && _rendererEnabledStates[i];
            }
        }

        private void RestoreRenderers()
        {
            for (var i = 0; i < _renderers.Length; i++)
            {
                var itemRenderer = _renderers[i];
                if (itemRenderer == null)
                    continue;

                itemRenderer.enabled = _rendererEnabledStates[i];
            }
        }

        private void Complete(bool accepted)
        {
            _isCompleted = true;
            _positionSubscription?.Dispose();
            HidePreviews();
            _completed?.Invoke(accepted);
            Destroy(gameObject);
        }

        public void Dispose()
        {
            _positionSubscription?.Dispose();
        }

        private void OnDestroy()
        {
            HidePreviews();
            Dispose();
        }

        private readonly struct PreviewInstance
        {
            public PreviewInstance(UseCommandParameters command, GameObject gameObject)
            {
                Command = command;
                GameObject = gameObject;
            }

            public UseCommandParameters Command { get; }
            public GameObject GameObject { get; }
        }
    }
}
