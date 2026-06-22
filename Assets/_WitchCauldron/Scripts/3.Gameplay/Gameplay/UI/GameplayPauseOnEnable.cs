using System;
using Gameplay._root;
using UnityEngine;
using Zenject;

namespace Gameplay.UI
{
    public sealed class GameplayPauseOnEnable : MonoBehaviour
    {
        private GameplayPauseService _pauseService;
        private IDisposable _pauseHandle;

        [Inject]
        public void Construct(GameplayPauseService pauseService)
        {
            _pauseService = pauseService;
            RefreshPause();
        }

        private void OnEnable()
        {
            RefreshPause();
        }

        private void OnDisable()
        {
            ReleasePause();
        }

        private void OnDestroy()
        {
            ReleasePause();
        }

        private void RefreshPause()
        {
            if (!isActiveAndEnabled || _pauseService == null || _pauseHandle != null)
                return;

            _pauseHandle = _pauseService.RequestPause();
        }

        private void ReleasePause()
        {
            _pauseHandle?.Dispose();
            _pauseHandle = null;
        }
    }
}
