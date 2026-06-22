using System;
using UnityEngine;

namespace Gameplay._root
{
    public sealed class GameplayPauseService : IDisposable
    {
        private int _pauseRequests;
        private float _previousTimeScale = 1f;

        public bool IsPaused => _pauseRequests > 0;

        public IDisposable RequestPause()
        {
            _pauseRequests++;

            if (_pauseRequests == 1)
            {
                _previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }

            return new PauseHandle(this);
        }

        public void Dispose()
        {
            _pauseRequests = 0;
            Time.timeScale = 1f;
        }

        private void ReleasePause()
        {
            if (_pauseRequests <= 0)
                return;

            _pauseRequests--;

            if (_pauseRequests == 0)
                Time.timeScale = _previousTimeScale;
        }

        private sealed class PauseHandle : IDisposable
        {
            private GameplayPauseService _service;

            public PauseHandle(GameplayPauseService service)
            {
                _service = service;
            }

            public void Dispose()
            {
                _service?.ReleasePause();
                _service = null;
            }
        }
    }
}
