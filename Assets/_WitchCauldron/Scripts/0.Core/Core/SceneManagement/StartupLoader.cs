using Core.Data;
using Core.Utils;
using Zenject;

namespace Core.SceneManagement
{
    public class StartupLoader : IInitializable
    {
        
        private readonly SceneLoader _sceneLoader;

        public StartupLoader(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        
        public void Initialize()
        {
            
#if UNITY_EDITOR
            _sceneLoader.LoadScene(EditorStartSceneCache.RequestedSceneName);
            return;
#endif
            
            _sceneLoader.LoadScene(Scenes.MainMenu);
        }
    }
}