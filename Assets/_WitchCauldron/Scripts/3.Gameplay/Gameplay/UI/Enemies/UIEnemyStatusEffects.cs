using System.Collections.Generic;
using Gameplay.Battle.Effects;
using Gameplay.Battle.Effects.Base;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Enemies
{
    public class UIEnemyStatusEffects :  MonoBehaviour
    {
        [SerializeField] private Image[] _iconSlots;

        private readonly Dictionary<EffectRuntime, Image> _activeIcons = new();
        private readonly Queue<Image> _freeIcons = new();
        
        private readonly CompositeDisposable _disposables = new();

        public void Initialize(EffectReceiver effectReceiver)
        {
            if (effectReceiver == null)
                return;
            
            
            effectReceiver.EffectAdded.Subscribe(AddIcon).AddTo(_disposables);
            effectReceiver.EffectRemoved.Subscribe(RemoveIcon).AddTo(_disposables);
        }
        
        
        private void Awake()
        {
            foreach (var icon in _iconSlots)
            {
                icon.gameObject.SetActive(false);
                _freeIcons.Enqueue(icon);
            }
        }
        private void AddIcon(EffectRuntime effect)
        {
            if (effect == null || effect.EffectData == null || effect.EffectData.EffectIcon == null)
                return;

            if (_freeIcons.Count == 0)
                return;

            var icon = _freeIcons.Dequeue();
            icon.sprite = effect.EffectData.EffectIcon;
            icon.gameObject.SetActive(true);

            _activeIcons.Add(effect, icon);
        }
        private void RemoveIcon(EffectRuntime effect)
        {
            if (!_activeIcons.Remove(effect, out var icon))
                return;

            icon.sprite = null;
            icon.gameObject.SetActive(false);
            _freeIcons.Enqueue(icon);
        }
        
        private void OnDestroy()
        {
            _disposables.Dispose();
        }
        
    }
}