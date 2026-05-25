using System;
using Core.Data;
using Core.Run;
using Core.SceneManagement;
using Gameplay.Level;
using R3;
using Zenject;

namespace Gameplay._root
{
    public class GameplayRunFlowController : IInitializable, IDisposable
    {
        private readonly G _game;
        private readonly RunState _runState;
        private readonly SceneLoader _sceneLoader;
        private readonly CompositeDisposable _disposables = new();

        public GameplayRunFlowController(G game, RunState runState, SceneLoader sceneLoader)
        {
            _game = game;
            _runState = runState;
            _sceneLoader = sceneLoader;
        }

        public void Initialize()
        {
            _game.GameWon
                .Subscribe(_ => CompleteLevelAndOpenHut())
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void CompleteLevelAndOpenHut()
        {
            _runState.TrySetNextLevel();
            _sceneLoader.LoadScene(Scenes.Hut);
        }
    }
}
