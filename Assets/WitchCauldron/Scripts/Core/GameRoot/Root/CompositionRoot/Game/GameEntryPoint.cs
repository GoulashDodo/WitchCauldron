using UnityEngine;
using UnityEngine.SceneManagement;
using WitchCauldron.Scripts.Core.GameRoot.Root.CompositionRoot.Game.Registrations;
using Zenject;

namespace WitchCauldron.Scripts.Core.GameRoot.Root.CompositionRoot.Game
{
    public class GameEntryPoint
    {
        private static GameEntryPoint _instance;
        private readonly DiContainer _rootContainer = new();


        private readonly SceneLoader _sceneLoader;

        private GameEntryPoint()
        {
            GlobalRegistrations.Register(_rootContainer);
            
            _sceneLoader = _rootContainer.Resolve<SceneLoader>();
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutoStartGame()
        {
        
            _instance = new GameEntryPoint();


            _instance.RunGame();

        }
        
        private void RunGame()
        {

#if UNITY_EDITOR

            var sceneName = SceneManager.GetActiveScene().name;

            switch (sceneName)
            {
                case Data.Scenes.Gameplay:
                    _sceneLoader.LoadScene(Data.Scenes.Gameplay);
                    return;
                case Data.Scenes.MainMenu:
                    _sceneLoader.LoadScene(Data.Scenes.MainMenu);
                    return;
            }

            if (sceneName != Data.Scenes.Boot)
            {
                return;
            }

#endif

            _sceneLoader.LoadScene(Data.Scenes.MainMenu);
            
        }
        
    }
}
