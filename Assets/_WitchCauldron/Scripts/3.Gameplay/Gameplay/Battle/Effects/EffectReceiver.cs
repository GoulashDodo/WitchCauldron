using System.Collections.Generic;
using Gameplay.Battle.Effects.Base;
using R3;
using UnityEngine;

namespace Gameplay.Battle.Effects
{
    public class EffectReceiver : MonoBehaviour
    {
        
        private readonly List<EffectRuntime> _effects = new();

        private readonly Subject<EffectRuntime> _effectAdded = new();
        private readonly Subject<EffectRuntime> _effectRemoved = new();
        private bool _isDisposed;
        
        public Observable<EffectRuntime> EffectAdded => _effectAdded;
        public Observable<EffectRuntime> EffectRemoved => _effectRemoved;

        public EffectRuntime AddEffect(EffectData effectData)
        {
            if (_isDisposed)
                return null;

            var runtime = effectData.CreateRuntime();

            runtime.Initialize(effectData);
            runtime.ApplyTo(gameObject);

            _effects.Add(runtime);
            if (!_isDisposed)
                _effectAdded.OnNext(runtime);
            
            return runtime;
        }

        public void RemoveEffect(EffectRuntime effect)
        {
            if (effect == null)
                return;

            if (!_effects.Remove(effect))
                return;
            
            effect.Remove();
            if (!_isDisposed)
                _effectRemoved.OnNext(effect);
        }
        
        private void Update()
        {
            ApplyEffects();
        }
        
        private void ApplyEffects()
        {
            for (var i = _effects.Count - 1; i >= 0; i--)
            {
                var effect = _effects[i];
                effect.Tick(Time.deltaTime);

                if (!effect.IsExpired)
                    continue;

                effect.Remove();
                _effects.RemoveAt(i);

                if (!_isDisposed)
                    _effectRemoved.OnNext(effect);
            }
        }
        
        
        private void OnDestroy()
        {
            _isDisposed = true;
            
            _effectAdded.Dispose();
            _effectRemoved.Dispose();
        }
        
    }
}
