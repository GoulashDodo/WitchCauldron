using System.Collections;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using WitchCauldron.Scripts.Common.Utilits;
using WitchCauldron.Scripts.Core.GameRoot.Data;
using WitchCauldron.Scripts.Core.GameRoot.Root.EntryPoints;
using WitchCauldron.Scripts.Core.GameRoot.State.Providers;
using Zenject;

namespace WitchCauldron.Scripts.Core.GameRoot.Root
{
    public class SceneLoader
    {
        private readonly DiContainer _rootContainer;
        private readonly IGameStateProvider _gameStateProvider;

        private readonly Coroutines _coroutines;
        
        private readonly Subject<Unit> _onSceneLoadingStarted = new();
        private readonly Subject<Unit> _onSceneLoadingEnded = new();

        public Observable<Unit> OnSceneLoadingStarted => _onSceneLoadingStarted;
        public Observable<Unit> OnSceneLoadingEnded => _onSceneLoadingEnded;        
        
        
        public SceneLoader(DiContainer rootContainer,IGameStateProvider gameStateProvider)
        {
            _rootContainer = rootContainer;
            _gameStateProvider = gameStateProvider;

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

        
            //Simulating loading
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
            

            var sceneEntryPoint = Object.FindFirstObjectByType<SceneEntryPoint>();
            if (!sceneEntryPoint)
            {
                Debug.LogError($"{Scenes.Gameplay}: entry point not found!!");
            }
            
            var sceneContainer = new DiContainer(_rootContainer);
            sceneEntryPoint.Run(sceneContainer);
            
        
            //Simulating loading
            yield return new WaitForSeconds(0.5f);
            
            _onSceneLoadingEnded.OnNext(Unit.Default);

        }
        
        private static IEnumerator LoadSceneAsync(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }

        
    }
}