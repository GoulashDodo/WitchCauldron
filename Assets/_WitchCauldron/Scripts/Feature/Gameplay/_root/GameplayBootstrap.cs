using Feature.Gameplay.Level;
using Zenject;

namespace Feature.Gameplay._root
{
    public class GameplayBootstrap : IInitializable
    {

        private G _game;

        public GameplayBootstrap(G game)
        {
            _game = game;
            
        }
        
        
        public void Initialize()
        {
            _game.StartGameplay();
        }

    }
}