using Core.Audio;
using Zenject;

namespace Hut.CompositionRoot
{
    public sealed class HutAudioBootstrap : IInitializable
    {
        private readonly AudioService _audioService;

        public HutAudioBootstrap(AudioService audioService)
        {
            _audioService = audioService;
        }

        public void Initialize()
        {
            _audioService.PlayMusic(AudioId.Music_Hut);
            _audioService.PlayAmbient(AudioId.Ambient_Swamp);
        }
    }
}
