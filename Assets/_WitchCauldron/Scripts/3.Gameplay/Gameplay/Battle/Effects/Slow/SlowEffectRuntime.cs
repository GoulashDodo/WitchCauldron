using Gameplay.Battle.BattleEntities.Enemies.Core;
using Gameplay.Battle.Effects.Base;

namespace Gameplay.Battle.Effects.Slow
{
    public class SlowEffectRuntime : EffectRuntime<SlowEffectData>
    {
        private EnemyMotor _motor;
        
        //TODO: prob change this, to avoid conflict between same effects
        private float _prevMultiplayer;
        
        protected override void OnApply()
        {
            if (Target.TryGetComponent(out _motor))
            {
                _prevMultiplayer = _motor.SpeedMultiplier;
                _motor.SetSpeedMultiplier(Data.SpeedMultiplier);
            }
        }

        protected override void OnRemove()
        {
            if (_motor != null)
            {
                _motor.SetSpeedMultiplier(_prevMultiplayer);
            }
        }
    }
}