using _WitchCauldron.Scripts.Core.GameRoot.State.Providers;
using UnityEngine;
using Zenject;

namespace _WitchCauldron.Scripts.Feature.Gameplay.UI
{
    public class UIGameplayRootBinder : MonoBehaviour
    {
        


        public void InitializeUI(DiContainer container)
        {
            
            //REFACTOR THIS
            var gameState =  container.Resolve<IGameStateProvider>().GameState;
            
            
        }
        
        
    }
}
