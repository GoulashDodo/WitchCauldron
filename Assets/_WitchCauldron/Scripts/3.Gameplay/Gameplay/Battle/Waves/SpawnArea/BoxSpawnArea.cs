using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Battle.Waves.SpawnArea
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class BoxSpawnArea : MonoBehaviour,  ISpawnArea
    {
        private BoxCollider2D _box;
        public Vector3 CenterPosition => _box != null ? _box.bounds.center : transform.position;

        private void Awake()
        {
            if (!TryGetComponent(out _box))
                throw new InvalidOperationException("BoxSpawnArea requires a BoxCollider2D.");
        }
        
        
        public Vector3 GetRandomPosition()
        {
            var b = _box.bounds; 
            var x = Random.Range(b.min.x, b.max.x);
            var y = Random.Range(b.min.y, b.max.y);
            return new Vector3(x, y, transform.position.z);
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!TryGetComponent(out BoxCollider2D box)) return;
            
            Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
            Gizmos.DrawCube(box.bounds.center, box.bounds.size);
            Gizmos.color = new Color(0f, 1f, 0f, 1f);
            Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
        }
        
    }
}
