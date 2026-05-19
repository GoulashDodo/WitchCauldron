using Feature.Gameplay._root;

namespace Core.GameRoot._root
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