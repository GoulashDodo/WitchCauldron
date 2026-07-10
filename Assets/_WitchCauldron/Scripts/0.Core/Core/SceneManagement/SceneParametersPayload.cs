using Gameplay._root;

namespace Core.SceneManagement
{
    public class SceneParametersPayload
    {
        public GameplayEntryParameters  GameplayEntryParameters { get; private set; }


        public void SetGameplayEntryParameters(GameplayEntryParameters gameplayEntryParameters)
        {
            GameplayEntryParameters = gameplayEntryParameters;
        }
        
    }
}   