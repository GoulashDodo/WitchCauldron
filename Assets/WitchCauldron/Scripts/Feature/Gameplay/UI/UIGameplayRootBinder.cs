using UnityEngine;
using WitchCauldron.Scripts.Core.GameRoot.State.Providers;
using Zenject;

namespace WitchCauldron.Scripts.Feature.Gameplay.UI
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
