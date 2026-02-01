using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace _WitchCauldron.Scripts.Core.GameRoot._Root.CompositionRoot.Game
{
    public class GameEntryPoint
    {
        private static GameEntryPoint _instance;
        

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutoStartGame()
        {
            _ = ProjectContext.Instance;
            
            
            _instance = new GameEntryPoint();
            _instance.RunGame();
        }
        
        private void RunGame()
        {

            SceneManager.LoadScene(Data.Scenes.Boot);

        }
        
    }
}
