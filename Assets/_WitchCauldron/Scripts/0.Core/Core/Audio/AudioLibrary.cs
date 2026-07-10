using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Audio
{
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "Game/Audio/Audio Library", order = 0)]
    public sealed class AudioLibrary : ScriptableObject
    {
        [SerializeField] private AudioCue[] _cues;

        private Dictionary<AudioId, AudioCue> _cueById;

        public AudioCue GetCue(AudioId id)
        {
            EnsureCache();
            return _cueById.TryGetValue(id, out var cue) ? cue : null;
        }

        private void EnsureCache()
        {
            if (_cueById != null)
                return;

            _cueById = new Dictionary<AudioId, AudioCue>();

            if (_cues == null)
                return;

            foreach (var cue in _cues)
            {
                if (cue == null)
                    continue;

                _cueById[cue.Id] = cue;
            }
        }

        private void OnValidate()
        {
            _cueById = null;

            if (_cues == null)
                return;

            var seen = new HashSet<AudioId>();
            foreach (var cue in _cues)
            {
                if (cue == null)
                    continue;

                if (!seen.Add(cue.Id))
                    Debug.LogWarning($"{nameof(AudioLibrary)} '{name}' has duplicate audio id '{cue.Id}'.", this);
            }
        }

        [ContextMenu("Reset To Required Audio Ids")]
        private void ResetToRequiredAudioIds()
        {
            var ids = (AudioId[])Enum.GetValues(typeof(AudioId));
            _cues = new AudioCue[ids.Length];

            for (var i = 0; i < ids.Length; i++)
                _cues[i] = AudioCueFactory.Create(ids[i]);

            _cueById = null;
        }
    }
}
