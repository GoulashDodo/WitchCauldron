using System.Collections.Generic;
using Gameplay.Items.Usable.Commands;
using Gameplay.Items.Usable.Commands.Preview;
using R3;
using UnityEngine;

namespace Gameplay.Items.MonoBehaviours.View
{
    [RequireComponent(typeof(UsableItem))]
    public class UsableItemPreviewFx : UnityEngine.MonoBehaviour
    {
        [SerializeField] private bool _showOnlyWhenItemCanBeUsed = true;
        [SerializeField] private bool _hideItemRenderersWhenPreviewVisible = true;

        private readonly List<PreviewInstance> _previews = new();
        private readonly CompositeDisposable _disposables = new();

        private UsableItem _item;
        private SpriteRenderer[] _itemRenderers;
        private bool[] _itemRendererEnabledStates;
        private IUseCommandPreviewProcessor _previewProcessor;
        private bool _isPreviewActive;

        public void Initialize(IUseCommandPreviewProcessor previewProcessor)
        {
            _previewProcessor = previewProcessor;

            if (_item != null && _item.IsDragging)
                ShowPreviews();
        }

        private void Awake()
        {
            _item = GetComponent<UsableItem>();
            _itemRenderers = GetComponentsInChildren<SpriteRenderer>();
            _itemRendererEnabledStates = new bool[_itemRenderers.Length];
            CacheItemRendererStates();
        }

        private void OnEnable()
        {
            _item.PickedUp.Subscribe(OnPickedUp).AddTo(_disposables);
            _item.Dropped.Subscribe(OnDropped).AddTo(_disposables);
        }

        private void OnDisable()
        {
            HidePreviews();
            _disposables.Clear();
        }

        private void Update()
        {
            if (!_isPreviewActive)
                return;

            var position = (Vector2)transform.position;
            var visible = !_showOnlyWhenItemCanBeUsed || _item.CanUseAtCurrentPosition();

            foreach (var preview in _previews)
            {
                if (preview.GameObject == null)
                    continue;

                preview.GameObject.SetActive(visible);
                _previewProcessor.UpdatePreview(preview.GameObject, preview.Command, position, _item.Settings);
            }

            SetItemRenderersVisible(!visible || !_hideItemRenderersWhenPreviewVisible);
        }

        private void OnPickedUp(Unit _)
        {
            ShowPreviews();
        }

        private void OnDropped(Unit _)
        {
            HidePreviews();
        }

        private void ShowPreviews()
        {
            HidePreviews();
            CacheItemRendererStates();

            if (_previewProcessor == null || _item.Settings?.OnUseCommands == null)
                return;

            var position = (Vector2)transform.position;

            foreach (var command in _item.Settings.OnUseCommands)
            {
                var preview = _previewProcessor.CreatePreview(command, position, _item.Settings);
                if (preview == null)
                    continue;

                _previews.Add(new PreviewInstance(command, preview));
            }

            _isPreviewActive = _previews.Count > 0;
        }

        private void HidePreviews()
        {
            foreach (var preview in _previews)
            {
                if (preview.GameObject != null)
                    Destroy(preview.GameObject);
            }

            _previews.Clear();
            _isPreviewActive = false;
            RestoreItemRenderers();
        }

        private void CacheItemRendererStates()
        {
            for (var i = 0; i < _itemRenderers.Length; i++)
                _itemRendererEnabledStates[i] = _itemRenderers[i] != null && _itemRenderers[i].enabled;
        }

        private void SetItemRenderersVisible(bool visible)
        {
            for (var i = 0; i < _itemRenderers.Length; i++)
            {
                var itemRenderer = _itemRenderers[i];
                if (itemRenderer == null)
                    continue;

                itemRenderer.enabled = visible && _itemRendererEnabledStates[i];
            }
        }

        private void RestoreItemRenderers()
        {
            for (var i = 0; i < _itemRenderers.Length; i++)
            {
                var itemRenderer = _itemRenderers[i];
                if (itemRenderer == null)
                    continue;

                itemRenderer.enabled = _itemRendererEnabledStates[i];
            }
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
