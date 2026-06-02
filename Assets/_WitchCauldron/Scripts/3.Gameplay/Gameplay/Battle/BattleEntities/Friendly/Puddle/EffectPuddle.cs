using System.Collections.Generic;
using Gameplay.Battle.Effects;
using Gameplay.Battle.Effects.Base;
using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Friendly.Puddle
{
    public class EffectPuddle : MonoBehaviour
    {
        [SerializeField] private EffectData _effectData;

        private readonly Dictionary<EffectReceiver, EffectRuntime> _activeEffects = new();

        private void OnTriggerEnter2D(Collider2D other)
        {
            var effectReceiver = other.GetComponentInParent<EffectReceiver>();
            if (effectReceiver == null)
                return;

            if (_activeEffects.ContainsKey(effectReceiver))
                return;

            var runtime = effectReceiver.AddEffect(_effectData);
            if (runtime == null)
                return;
            
            _activeEffects.Add(effectReceiver, runtime);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var effectReceiver = other.GetComponentInParent<EffectReceiver>();
            if (effectReceiver == null)
                return;

            if (!_activeEffects.TryGetValue(effectReceiver, out var runtime))
                return;

            effectReceiver.RemoveEffect(runtime);
            _activeEffects.Remove(effectReceiver);
        }

        private void OnDisable()
        {
            foreach (var (receiver, runtime) in _activeEffects)
            {
                if (receiver != null)
                    receiver.RemoveEffect(runtime);
            }
            
            _activeEffects.Clear();
        }
        
    }
}
