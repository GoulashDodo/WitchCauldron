using UnityEngine;

namespace Feature.Gameplay.Battle.Enemies.Core.Behaviours.Movement
{
    public class MoveContext
    {
        public Transform Transform { get; }
        public Rigidbody2D Rigidbody { get; }
        
        public float Speed { get; set; }
        public float StopDistance { get; set; }
        public LayerMask StopLayerMask { get; set; }



        public MoveContext(Transform transform, Rigidbody2D rigidbody)
        {
            Transform = transform;
            Rigidbody = rigidbody;
        }
}
}