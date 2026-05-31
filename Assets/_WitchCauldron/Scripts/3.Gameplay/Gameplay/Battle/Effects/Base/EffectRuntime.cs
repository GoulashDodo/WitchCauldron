using UnityEngine;

namespace Gameplay.Battle.Effects.Base
{
    public abstract class EffectRuntime
    {

        private float _timeLeft;
        
        protected GameObject Target { get; private set; }
        protected EffectData Data { get; private set; }

        public bool IsExpired => !Data.IsPermanent && _timeLeft <= 0;
        
        public void Initialize(EffectData data)
        {
            Data = data;
            _timeLeft = data.Duration;
        }
        
        public void ApplyTo(GameObject target)
        {
            Target = target;
            OnApply();
        }

        public void Tick(float deltaTime)
        {
            if (!Data.IsPermanent)
                _timeLeft -= deltaTime;

            OnTick(deltaTime);
        }

        public void Remove()
        {
            OnRemove();
        }

        
        

        protected abstract void OnApply();
        protected virtual void OnTick(float deltaTime) { }
        protected abstract void OnRemove();
        
    }

    public abstract class EffectRuntime<TData> : EffectRuntime where TData : EffectData
    {
        protected new TData Data => (TData)base.Data;
    }
    
}
