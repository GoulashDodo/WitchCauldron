using Feature.Gameplay.Level;
using Zenject;

namespace Feature.Gameplay._root
{
    public class GameBootstrap : IInitializable
    {

        private G _game;

        public GameBootstrap(G game)
        {
            _game = game;
            
        }
        
        
        public void Initialize()
        {
            _game.StartGameplay();
        }

    }
}