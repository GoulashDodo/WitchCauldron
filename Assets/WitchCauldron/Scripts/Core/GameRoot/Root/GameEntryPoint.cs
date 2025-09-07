using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using WitchCauldron.Scripts.Common.Utilits;
using WitchCauldron.Scripts.Core.GameRoot.Data;
using Zenject;

namespace WitchCauldron.Scripts.Core.GameRoot.Root
{
    public class GameEntryPoint
    {
        private static GameEntryPoint _instance;
        private readonly Coroutines _coroutines;


        private readonly DiContainer _rootContainer = new();


        private GameEntryPoint()
        {
            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);
            
            GlobalRegistrations.Register(_rootContainer);
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
                case Scenes.Gameplay:
                    _coroutines.StartCoroutine(LoadAndStartGameplay());
                    return;
                case Scenes.MainMenu:
                    _coroutines.StartCoroutine(LoadAndStartMainMenu());
                    return;
            }

            if (sceneName != Scenes.Boot)
            {
                return;
            }

#endif
            _coroutines.StartCoroutine(LoadAndStartMainMenu());

        }
        
        
        
        private IEnumerator LoadAndStartMainMenu()
        {
            
            yield return LoadSceneAsync(Scenes.Boot);
            yield return LoadSceneAsync(Scenes.MainMenu);

        
            //Simulating loading
            yield return new WaitForSeconds(0.5f);
            

        }
        
        private IEnumerator LoadAndStartGameplay()
        {

            yield return LoadSceneAsync(Scenes.Boot);
            yield return LoadSceneAsync(Scenes.Gameplay);

        
            //Simulating loading
            yield return new WaitForSeconds(0.5f);
            
            

        }

        
        private static IEnumerator LoadSceneAsync(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }
        
        
        
    }
}
