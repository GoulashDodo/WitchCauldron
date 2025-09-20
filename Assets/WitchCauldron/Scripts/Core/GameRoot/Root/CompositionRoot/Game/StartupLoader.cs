using WitchCauldron.Scripts.Common.Utils;
using Zenject;

namespace WitchCauldron.Scripts.Core.GameRoot.Root.CompositionRoot.Game
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