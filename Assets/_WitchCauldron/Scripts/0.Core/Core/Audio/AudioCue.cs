using System;
using UnityEngine;

namespace Core.Audio
{
    [Serializable]
    public sealed class AudioCue
    {
        [SerializeField] private AudioId _id;
        [SerializeField] private AudioCategory _category = AudioCategory.Sfx;
        [SerializeField] private AudioClip[] _clips;
        [SerializeField, Range(0f, 1f)] private float _volume = 1f;
        [SerializeField, Min(0f)] private float _pitchMin = 1f;
        [SerializeField, Min(0f)] private float _pitchMax = 1f;
        [SerializeField, Min(0f)] private float _cooldown;
        [SerializeField, Range(0f, 1f)] private float _spatialBlend;
        [SerializeField] private bool _loop;

        public AudioId Id => _id;
        public AudioCategory Category => _category;
        public AudioClip[] Clips => _clips;
        public float Volume => _volume;
        public float PitchMin => _pitchMin;
        public float PitchMax => _pitchMax;
        public float Cooldown => _cooldown;
        public float SpatialBlend => _spatialBlend;
        public bool Loop => _loop;

        public bool HasClip => _clips != null && _clips.Length > 0;

        public AudioCue() { }

        internal AudioCue(
            AudioId id,
            AudioCategory category,
            bool loop = false,
            float cooldown = 0f,
            float spatialBlend = 0f)
        {
            _id = id;
            _category = category;
            _volume = 1f;
            _pitchMin = 1f;
            _pitchMax = 1f;
            _cooldown = cooldown;
            _spatialBlend = spatialBlend;
            _loop = loop;
        }

        public AudioClip GetRandomClip()
        {
            if (!HasClip)
                return null;

            return _clips[UnityEngine.Random.Range(0, _clips.Length)];
        }

        public float GetRandomPitch()
        {
            var min = Mathf.Min(_pitchMin, _pitchMax);
            var max = Mathf.Max(_pitchMin, _pitchMax);
            return Mathf.Approximately(min, max) ? min : UnityEngine.Random.Range(min, max);
        }
    }
}
