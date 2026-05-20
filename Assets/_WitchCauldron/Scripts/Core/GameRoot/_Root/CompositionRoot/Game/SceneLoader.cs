using System.Collections;
using Common.Utils;
using Core.GameRoot.Data;
using Feature.Gameplay._root;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Core.GameRoot._root.CompositionRoot.Game
{
    public class SceneLoader
    {

        private readonly Coroutines _coroutines;
        
        private readonly Subject<Unit> _onSceneLoadingStarted = new();
        private readonly Subject<Unit> _onSceneLoadingEnded = new();
        private readonly SceneParametersPayload _sceneParametersPayload;

        public Observable<Unit> OnSceneLoadingStarted => _onSceneLoadingStarted;
        public Observable<Unit> OnSceneLoadingEnded => _onSceneLoadingEnded;        
        
        
        public SceneLoader(SceneParametersPayload parametersPayload)
        {
            _sceneParametersPayload = parametersPayload;
            
            
            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);
            
        }

        public void LoadScene(string sceneName)
        {
            switch (sceneName)
            {
                case Scenes.Gameplay:
                    
                    //TODO: Change default level selection
                    var entryParams = new GameplayEntryParameters("level_default");
                    
                    _coroutines.StartCoroutine(LoadAndStartGameplay(entryParams));
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
        
        private IEnumerator LoadAndStartGameplay(GameplayEntryParameters gameplayEntryParameters)
        {

            _onSceneLoadingStarted.OnNext(Unit.Default);
            
            yield return LoadSceneAsync(Scenes.Boot);
            yield return LoadSceneAsync(Scenes.Gameplay);
            

            var sceneEntryPoint = Object.FindFirstObjectByType<SceneContext>();
            _sceneParametersPayload.SetGameplayEntryParameters(gameplayEntryParameters);
            
            sceneEntryPoint.Run();
            
    
            _onSceneLoadingEnded.OnNext(Unit.Default);

        }
        
        private static IEnumerator LoadSceneAsync(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }

        
    }
}