using UnityEngine;
using WitchCauldron.Scripts.Core.GameRoot.State.Providers;
using WitchCauldron.Scripts.Core.GameRoot.State.Root;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.UI;
using Zenject;

namespace WitchCauldron.Scripts.Feature.Gameplay.UI
{
    public class UIGameplayRootBinder : MonoBehaviour
    {
        
        [SerializeField] private UIReceiptNamePanel _namePanel;


        public void InitializeUI(DiContainer container)
        {
            
            //REFACTOR THIS
            var gameState =  container.Resolve<IGameStateProvider>().GameState;
            
            _namePanel.Initialze(gameState.Cauldron);
            
        }
        
        
    }
}
