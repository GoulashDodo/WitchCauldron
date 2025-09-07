using System.Collections;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using WitchCauldron.Scripts.Common.Utilits;
using WitchCauldron.Scripts.Core.GameRoot.Data;

namespace WitchCauldron.Scripts.Core.GameRoot.Root
{
    public class SceneLoader
    {
        
        private readonly Coroutines _coroutines;
        
        private readonly Subject<Unit> _onSceneLoadingStarted = new Subject<Unit>();
        private readonly Subject<Unit> _onSceneLoadingEnded = new Subject<Unit>();

        public Observable<Unit> OnSceneLoadingStarted => _onSceneLoadingStarted;
        public Observable<Unit> OnSceneLoadingEnded => _onSceneLoadingEnded;        
        
        
        private SceneLoader()
        {
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