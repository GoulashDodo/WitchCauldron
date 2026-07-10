using UnityEngine;

namespace Gameplay.Items.Combination
{
    [RequireComponent(typeof(Collider2D))]
    public class CombineZone : MonoBehaviour
    {
        private void Reset()
        {
            if (TryGetComponent(out Collider2D zoneCollider))
                zoneCollider.isTrigger = true;
        }
    }
}
