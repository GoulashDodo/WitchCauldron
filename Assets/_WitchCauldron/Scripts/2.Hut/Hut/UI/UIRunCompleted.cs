using Core.Run;
using UnityEngine;

namespace Hut.UI
{
    public class UIRunCompleted : MonoBehaviour
    {
        
        
        public void Initialize(RunState runState)
        {
            gameObject.SetActive(runState.IsCompleted);
        }
    }
}
