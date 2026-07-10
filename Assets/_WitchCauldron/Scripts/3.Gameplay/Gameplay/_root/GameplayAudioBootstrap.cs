using Core.Audio;
using Zenject;

namespace Gameplay._root
{
    public sealed class GameplayAudioBootstrap : IInitializable
    {
        private readonly AudioService _audioService;

        public GameplayAudioBootstrap(AudioService audioService)
        {
            _audioService = audioService;
        }

        public void Initialize()
        {
            _audioService.PlayMusic(AudioId.Music_Gameplay);
            _audioService.PlayAmbient(AudioId.Ambient_Swamp);
        }
    }
}
