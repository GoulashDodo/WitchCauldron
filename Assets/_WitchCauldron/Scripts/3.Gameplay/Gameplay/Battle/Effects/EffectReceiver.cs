using System.Collections.Generic;
using Gameplay.Battle.Effects.Base;
using UnityEngine;

namespace Gameplay.Battle.Effects
{
    public class EffectReceiver : MonoBehaviour
    {
        
        private readonly List<EffectRuntime> _effects = new();


        public void AddEffect(EffectData effectData)
        {
            var runtime = effectData.CreateRuntime();

            runtime.Initialize(effectData);
            runtime.ApplyTo(gameObject);

            _effects.Add(runtime);;
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
            }
        }
        
    }
}