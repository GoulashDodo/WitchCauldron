using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Friendly
{
    public class LifetimeController : MonoBehaviour
    {
        [SerializeField] private float _lifetime;

        private void OnEnable()
        {
            Invoke(nameof(Expire), _lifetime);
        }

        private void OnDisable()
        {
            Invoke(nameof(Expire), _lifetime);
        }
        

        private void Expire()
        {
            Destroy(gameObject);
  
            
            
        }
        
    }
}