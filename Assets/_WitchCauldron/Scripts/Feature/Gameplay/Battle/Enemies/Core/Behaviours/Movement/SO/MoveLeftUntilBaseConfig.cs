using Core.GameRoot.Data;
using Feature.Gameplay.Battle.Enemies.Core.Behaviours.Movement.Strategy;
using UnityEngine;

namespace Feature.Gameplay.Battle.Enemies.Core.Behaviours.Movement.SO
{
    [CreateAssetMenu(fileName = "Enemy MoveLeftUntilBase", menuName = "Game/Enemies/MoveSettings/MoveLeftUntilBase")]

    public class MoveLeftUntilBaseConfig : MoveBehaviourConfig
    {
        

        public override IMoveBehaviour CreateBehaviour()
        {
            return new MoveLeftUntilRaycastHitBehaviour();
        }

        public override void ConfigureContext(MoveContext context)
        {
            context.StopLayerMask = LayerMask.GetMask(Layers.Base);
        }

        
    }
}