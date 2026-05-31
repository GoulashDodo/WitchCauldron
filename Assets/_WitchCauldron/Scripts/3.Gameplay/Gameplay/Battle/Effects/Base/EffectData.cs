using UnityEngine;

namespace Gameplay.Battle.Effects.Base
{
    public abstract class EffectData : ScriptableObject
    {
        [field: SerializeField] public string EffectName { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public bool IsPermanent { get; private set; }

        public abstract EffectRuntime CreateRuntime();
    }
    
    public abstract class EffectData<TEffectRuntime>: EffectData where TEffectRuntime : EffectRuntime, new()
    {
        public override EffectRuntime CreateRuntime()
        {
            return new TEffectRuntime();
        }
    }
}