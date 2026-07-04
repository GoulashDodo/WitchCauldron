namespace Core.Audio
{
    internal static class AudioCueFactory
    {
        public static AudioCue Create(AudioId id)
        {
            return id switch
            {
                AudioId.UI_Click => new AudioCue(id, AudioCategory.Ui),
                AudioId.UI_Back => new AudioCue(id, AudioCategory.Ui),
                AudioId.UI_OpenPanel => new AudioCue(id, AudioCategory.Ui),
                AudioId.UI_ClosePanel => new AudioCue(id, AudioCategory.Ui),
                AudioId.Music_Hut => new AudioCue(id, AudioCategory.Music, true),
                AudioId.Music_Gameplay => new AudioCue(id, AudioCategory.Music, true),
                AudioId.Ambient_Swamp => new AudioCue(id, AudioCategory.Ambient, true),
                AudioId.Enemy_Hit => new AudioCue(id, AudioCategory.Sfx, false, 0.08f, 1f),
                AudioId.Fire_Burn => new AudioCue(id, AudioCategory.Sfx, false, 0.2f, 1f),
                AudioId.Slime_Slow => new AudioCue(id, AudioCategory.Sfx, false, 0.2f, 1f),
                _ => new AudioCue(id, AudioCategory.Sfx)
            };
        }
    }
}
