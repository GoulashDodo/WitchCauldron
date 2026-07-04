using System.Collections.Generic;
using Core.Audio;
using Gameplay.Battle.Effects.Base;
using Gameplay.Battle.Effects.Damage;
using Gameplay.Battle.Effects.Slow;
using Gameplay.Battle.HealthSystem.Structs;
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
            if (_isDisposed || effectData == null)
                return null;

            if (TryGetActiveEffectOfType(effectData.GetType(), out var existingEffect))
                return existingEffect;

            var runtime = effectData.CreateRuntime();

            runtime.Initialize(effectData);
            runtime.ApplyTo(gameObject);

            _effects.Add(runtime);
            if (!_isDisposed)
                _effectAdded.OnNext(runtime);

            PlayEffectAudio(runtime);
            
            return runtime;
        }

        private void PlayEffectAudio(EffectRuntime runtime)
        {
            if (runtime == null)
                return;

            var audioService = AudioService.Current;
            if (audioService == null)
                return;

            if (runtime.EffectData is SlowEffectData)
            {
                audioService.PlaySfx(AudioId.Slime_Slow, transform.position);
                return;
            }

            if (runtime.EffectData is DamageEffectData damageEffectData &&
                damageEffectData.DamageType == DamageType.Fire)
            {
                audioService.PlaySfx(AudioId.Fire_Burn, transform.position);
            }
        }

        private bool TryGetActiveEffectOfType(System.Type effectDataType, out EffectRuntime effect)
        {
            for (var i = 0; i < _effects.Count; i++)
            {
                var activeEffect = _effects[i];
                if (activeEffect?.EffectData == null)
                    continue;

                if (activeEffect.EffectData.GetType() != effectDataType)
                    continue;

                effect = activeEffect;
                return true;
            }

            effect = null;
            return false;
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
