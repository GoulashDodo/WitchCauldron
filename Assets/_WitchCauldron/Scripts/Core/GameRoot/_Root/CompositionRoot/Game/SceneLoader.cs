using System.Collections;
using _WitchCauldron.Scripts.Common.Utils;
using _WitchCauldron.Scripts.Core.GameRoot.Data;
using _WitchCauldron.Scripts.Core.GameRoot.State.Providers;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace _WitchCauldron.Scripts.Core.GameRoot._Root.CompositionRoot.Game
{
    public class SceneLoader
    {
        private readonly DiContainer _rootContainer;

        private readonly Coroutines _coroutines;
        
        private readonly Subject<Unit> _onSceneLoadingStarted = new();
        private readonly Subject<Unit> _onSceneLoadingEnded = new();

        public Observable<Unit> OnSceneLoadingStarted => _onSceneLoadingStarted;
        public Observable<Unit> OnSceneLoadingEnded => _onSceneLoadingEnded;        
        
        
        public SceneLoader(DiContainer rootContainer)
        {
            _rootContainer = rootContainer;

            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);
            
        }

        public void LoadScene(string sceneName)
        {
            switch (sceneName)
            {
                case Scenes.Gameplay:
                    _coroutines.StartCoroutine(LoadAndStartGameplay());
                    return;
                case Scenes.MainMenu:
                    _coroutines.StartCoroutine(LoadAndStartMainMenu());
                    return;
            }
        }
        
        private IEnumerator LoadAndStartMainMenu()
        {

            _onSceneLoadingStarted.OnNext(Unit.Default);
            
            yield return LoadSceneAsync(Scenes.Boot);
            yield return LoadSceneAsync(Scenes.MainMenu);

            yield return new WaitForSeconds(0.5f);
            
            
            _onSceneLoadingEnded.OnNext(Unit.Default);

        }
        
        private IEnumerator LoadAndStartGameplay()
        {

            _onSceneLoadingStarted.OnNext(Unit.Default);
            
            yield return LoadSceneAsync(Scenes.Boot);
            yield return LoadSceneAsync(Scenes.Gameplay);

            
            var isGameStateLoaded = false;
            _rootContainer.Resolve<IGameStateProvider>().LoadGameState().Subscribe(_ => isGameStateLoaded = true);
            yield return new WaitUntil(() => isGameStateLoaded);
            

            var sceneEntryPoint = Object.FindFirstObjectByType<SceneContext>();
            if (!sceneEntryPoint)
            {
                Debug.LogError($"{Scenes.Gameplay}: entry point not found!!");
            }
            
            
            sceneEntryPoint.Run();
            
    
            _onSceneLoadingEnded.OnNext(Unit.Default);

        }
        
        private static IEnumerator LoadSceneAsync(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }

        
    }
}