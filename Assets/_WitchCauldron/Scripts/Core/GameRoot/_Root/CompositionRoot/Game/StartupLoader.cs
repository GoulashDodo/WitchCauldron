using Common.Utils;
using Zenject;

namespace Core.GameRoot._root.CompositionRoot.Game
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
            _sceneLoader.LoadScene(EditorStartSceneCache.RequestedSceneName);
        }
    }
}