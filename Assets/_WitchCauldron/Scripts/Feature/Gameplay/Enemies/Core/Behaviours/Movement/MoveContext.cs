using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Enemies.Core.Behaviours.Movement
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