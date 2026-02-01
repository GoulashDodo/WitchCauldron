using _WitchCauldron.Scripts.Core.GameRoot.State.Root;
using R3;

namespace _WitchCauldron.Scripts.Core.GameRoot.State.Providers
{
    public interface IGameStateProvider
    {
        public GameState GameState { get; }

        public Observable<GameState> LoadGameState();
        public Observable<bool> SaveGameState();
        public Observable<bool> ResetGameState();
        
    }
}