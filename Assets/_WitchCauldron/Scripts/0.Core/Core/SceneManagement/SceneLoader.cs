using System.Collections;
using Core.Data;
using Core.Run;
using Core.Utils;
using Gameplay._root;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Core.SceneManagement
{
    public class SceneLoader
    {
        private const float LoadingFadeInTimeout = 0.6f;
        private const float MinimumLoadingDuration = 0.45f;

        private readonly Coroutines _coroutines;
        
        private readonly Subject<Unit> _onSceneLoadingStarted = new();
        private readonly Subject<Unit> _onSceneLoadingEnded = new();
        private readonly SceneParametersPayload _sceneParametersPayload;
        private readonly RunState _runState;
        private bool _isLoadingScreenFadeInCompleted;

        public Observable<Unit> OnSceneLoadingStarted => _onSceneLoadingStarted;
        public Observable<Unit> OnSceneLoadingEnded => _onSceneLoadingEnded;        
        
        
        public SceneLoader(SceneParametersPayload parametersPayload, RunState runState)
        {
            _sceneParametersPayload = parametersPayload;
            _runState = runState;
            
            
            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);
            
        }

        public void LoadScene(string sceneName)
        {
            switch (sceneName)
            {
                
                case Scenes.MainMenu:
                    _coroutines.StartCoroutine(LoadAndStartMainMenu());
                    return;
                
                case Scenes.Hut:
                    _coroutines.StartCoroutine(LoadAndStartHut());
                    return;
                
                case Scenes.Gameplay:
                    if (!_runState.HasCurrentLevel && !_runState.StartNewRun())
                        return;

                    var entryParameters = new GameplayEntryParameters(_runState.CurrentLevelId);
                    
                    LoadGameplay(entryParameters);
                    return;
            }
        }


        public void LoadGameplay(GameplayEntryParameters gameplayEntryParameters)
        {
            _coroutines.StartCoroutine(LoadAndStartGameplay(gameplayEntryParameters));
        }

        public void NotifyLoadingScreenFadeInCompleted()
        {
            _isLoadingScreenFadeInCompleted = true;
        }
        
        private IEnumerator LoadAndStartMainMenu()
        {

            var loadingStartedAt = Time.realtimeSinceStartup;
            _isLoadingScreenFadeInCompleted = false;
            _onSceneLoadingStarted.OnNext(Unit.Default);
            yield return WaitForLoadingScreenFadeIn();
            
            yield return LoadSceneAsync(Scenes.Boot);
            yield return LoadSceneAsync(Scenes.MainMenu);

            var sceneEntryPoint = Object.FindFirstObjectByType<SceneContext>();
            
            sceneEntryPoint.Run();

            yield return WaitForMinimumLoadingTime(loadingStartedAt);
            _onSceneLoadingEnded.OnNext(Unit.Default);

        }
        
        private IEnumerator LoadAndStartHut()
        {

            var loadingStartedAt = Time.realtimeSinceStartup;
            _isLoadingScreenFadeInCompleted = false;
            _onSceneLoadingStarted.OnNext(Unit.Default);
            yield return WaitForLoadingScreenFadeIn();
            
            yield return LoadSceneAsync(Scenes.Boot);
            yield return LoadSceneAsync(Scenes.Hut);
            

            var sceneEntryPoint = Object.FindFirstObjectByType<SceneContext>();
            sceneEntryPoint.Run();

            yield return WaitForMinimumLoadingTime(loadingStartedAt);
            _onSceneLoadingEnded.OnNext(Unit.Default);

        }
        
        private IEnumerator LoadAndStartGameplay(GameplayEntryParameters gameplayEntryParameters)
        {

            var loadingStartedAt = Time.realtimeSinceStartup;
            _isLoadingScreenFadeInCompleted = false;
            _onSceneLoadingStarted.OnNext(Unit.Default);
            yield return WaitForLoadingScreenFadeIn();
            
            yield return LoadSceneAsync(Scenes.Boot);
            yield return LoadSceneAsync(Scenes.Gameplay);
            

            var sceneEntryPoint = Object.FindFirstObjectByType<SceneContext>();
            _sceneParametersPayload.SetGameplayEntryParameters(gameplayEntryParameters);
            
            sceneEntryPoint.Run();

            yield return WaitForMinimumLoadingTime(loadingStartedAt);
            _onSceneLoadingEnded.OnNext(Unit.Default);

        }
        
        private static IEnumerator LoadSceneAsync(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }

        private IEnumerator WaitForLoadingScreenFadeIn()
        {
            var startedAt = Time.realtimeSinceStartup;

            while (!_isLoadingScreenFadeInCompleted &&
                   Time.realtimeSinceStartup - startedAt < LoadingFadeInTimeout)
            {
                yield return null;
            }
        }

        private static IEnumerator WaitForMinimumLoadingTime(float loadingStartedAt)
        {
            var elapsed = Time.realtimeSinceStartup - loadingStartedAt;
            var remaining = MinimumLoadingDuration - elapsed;

            if (remaining > 0f)
                yield return new WaitForSecondsRealtime(remaining);
        }
        
    }
}
