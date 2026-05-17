using UnityEngine;

namespace Feature.Gameplay.Battle.Enemies.Core.Behaviours.Movement.SO
{
    public abstract class MoveBehaviourConfig : ScriptableObject
    {
        public abstract IMoveBehaviour CreateBehaviour();

        
        public virtual void ConfigureContext(MoveContext context) { }
    }
}