using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Core.Audio
{
    public sealed class AudioService : IInitializable, ITickable, IDisposable
    {
        private const string LibraryResourcePath = "Audio/AudioLibrary";
        private const string MasterVolumeKey = "MasterVolume";
        private const string MusicVolumeKey = "MusicVolume";
        private const string SfxVolumeKey = "SfxVolume";
        private const string UiVolumeKey = "UiVolume";
        private const string AmbientVolumeKey = "AmbientVolume";

        private readonly Dictionary<AudioId, float> _lastPlayedTimes = new();
        private readonly Queue<AudioSource> _availableOneShotSources = new();
        private readonly List<AudioSource> _activeOneShotSources = new();

        private AudioLibrary _library;
        private GameObject _root;
        private AudioSource _musicSource;
        private AudioSource _ambientSource;

        private float _masterVolume = 1f;
        private float _musicVolume = 1f;
        private float _sfxVolume = 1f;
        private float _uiVolume = 1f;
        private float _ambientVolume = 1f;

        public static AudioService Current { get; private set; }

        public void Initialize()
        {
            Current = this;
            _library = Resources.Load<AudioLibrary>(LibraryResourcePath);

            _root = new GameObject("[AUDIO]");
            UnityEngine.Object.DontDestroyOnLoad(_root);

            _musicSource = CreateLoopSource("Music");
            _ambientSource = CreateLoopSource("Ambient");

            LoadVolumes();
        }

        public void Tick()
        {
            for (var i = _activeOneShotSources.Count - 1; i >= 0; i--)
            {
                var source = _activeOneShotSources[i];
                if (source != null && source.isPlaying)
                    continue;

                RecycleOneShotSource(source);
                _activeOneShotSources.RemoveAt(i);
            }
        }

        public void Dispose()
        {
            SaveVolumes();

            if (Current == this)
                Current = null;

            if (_root != null)
                UnityEngine.Object.Destroy(_root);
        }

        public void PlaySfx(AudioId id)
        {
            PlayOneShot(id, null, AudioCategory.Sfx);
        }

        public void PlaySfx(AudioId id, Vector3 position)
        {
            PlayOneShot(id, position, AudioCategory.Sfx);
        }

        public void PlaySfx(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f, float spatialBlend = 0f)
        {
            PlayClip(clip, position, AudioCategory.Sfx, volume, pitch, spatialBlend);
        }

        public void PlayUi(AudioId id)
        {
            PlayOneShot(id, null, AudioCategory.Ui);
        }

        public void PlayMusic(AudioId id, bool fade = true)
        {
            PlayLoop(id, _musicSource, AudioCategory.Music);
        }

        public void PlayAmbient(AudioId id, bool fade = true)
        {
            PlayLoop(id, _ambientSource, AudioCategory.Ambient);
        }

        public void StopMusic(bool fade = true)
        {
            StopLoop(_musicSource);
        }

        public void StopAmbient(bool fade = true)
        {
            StopLoop(_ambientSource);
        }

        public void SetMasterVolume(float value)
        {
            _masterVolume = ClampVolume(value);
            ApplyLoopVolumes();
            SaveVolumes();
        }

        public void SetMusicVolume(float value)
        {
            _musicVolume = ClampVolume(value);
            ApplyLoopVolumes();
            SaveVolumes();
        }

        public void SetSfxVolume(float value)
        {
            _sfxVolume = ClampVolume(value);
            SaveVolumes();
        }

        public void SetUiVolume(float value)
        {
            _uiVolume = ClampVolume(value);
            SaveVolumes();
        }

        public void SetAmbientVolume(float value)
        {
            _ambientVolume = ClampVolume(value);
            ApplyLoopVolumes();
            SaveVolumes();
        }

        private void PlayOneShot(AudioId id, Vector3? position, AudioCategory expectedCategory)
        {
            var cue = GetPlayableCue(id);
            if (cue == null)
                return;

            if (cue.Category != expectedCategory)
                return;

            if (IsOnCooldown(cue))
                return;

            var clip = cue.GetRandomClip();
            if (clip == null)
                return;

            _lastPlayedTimes[id] = Time.unscaledTime;

            var source = GetOneShotSource();
            source.transform.position = position ?? _root.transform.position;
            source.clip = clip;
            source.volume = cue.Volume * GetCategoryVolume(cue.Category);
            source.pitch = cue.GetRandomPitch();
            source.spatialBlend = cue.SpatialBlend;
            source.loop = false;
            source.Play();

            _activeOneShotSources.Add(source);
        }

        private void PlayClip(
            AudioClip clip,
            Vector3 position,
            AudioCategory category,
            float volume,
            float pitch,
            float spatialBlend)
        {
            if (clip == null)
                return;

            var source = GetOneShotSource();
            source.transform.position = position;
            source.clip = clip;
            source.volume = Mathf.Clamp01(volume) * GetCategoryVolume(category);
            source.pitch = Mathf.Max(0.01f, pitch);
            source.spatialBlend = Mathf.Clamp01(spatialBlend);
            source.loop = false;
            source.Play();

            _activeOneShotSources.Add(source);
        }

        private void PlayLoop(AudioId id, AudioSource source, AudioCategory category)
        {
            var cue = GetPlayableCue(id);
            if (cue == null || source == null)
                return;

            var clip = cue.GetRandomClip();
            if (clip == null)
                return;

            if (source.clip == clip && source.isPlaying)
            {
                source.volume = cue.Volume * GetCategoryVolume(category);
                return;
            }

            source.Stop();
            source.clip = clip;
            source.volume = cue.Volume * GetCategoryVolume(category);
            source.pitch = cue.GetRandomPitch();
            source.spatialBlend = cue.SpatialBlend;
            source.loop = true;
            source.Play();
        }

        private void StopLoop(AudioSource source)
        {
            if (source == null)
                return;

            source.Stop();
            source.clip = null;
        }

        private AudioCue GetPlayableCue(AudioId id)
        {
            if (_library == null)
                return null;

            return _library.GetCue(id);
        }

        private bool IsOnCooldown(AudioCue cue)
        {
            if (cue.Cooldown <= 0f)
                return false;

            return _lastPlayedTimes.TryGetValue(cue.Id, out var lastPlayedTime) &&
                   Time.unscaledTime - lastPlayedTime < cue.Cooldown;
        }

        private AudioSource CreateLoopSource(string sourceName)
        {
            var child = new GameObject(sourceName);
            child.transform.SetParent(_root.transform, false);

            var source = child.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = true;
            return source;
        }

        private AudioSource GetOneShotSource()
        {
            if (_availableOneShotSources.Count > 0)
            {
                var pooled = _availableOneShotSources.Dequeue();
                pooled.gameObject.SetActive(true);
                return pooled;
            }

            var child = new GameObject("OneShot");
            child.transform.SetParent(_root.transform, false);

            var source = child.AddComponent<AudioSource>();
            source.playOnAwake = false;
            return source;
        }

        private void RecycleOneShotSource(AudioSource source)
        {
            if (source == null)
                return;

            source.Stop();
            source.clip = null;
            source.gameObject.SetActive(false);
            _availableOneShotSources.Enqueue(source);
        }

        private float GetCategoryVolume(AudioCategory category)
        {
            return _masterVolume * (category switch
            {
                AudioCategory.Music => _musicVolume,
                AudioCategory.Ui => _uiVolume,
                AudioCategory.Ambient => _ambientVolume,
                AudioCategory.Sfx => _sfxVolume,
                _ => 1f
            });
        }

        private void ApplyLoopVolumes()
        {
            ApplyLoopVolume(_musicSource, AudioCategory.Music);
            ApplyLoopVolume(_ambientSource, AudioCategory.Ambient);
        }

        private void ApplyLoopVolume(AudioSource source, AudioCategory category)
        {
            if (source == null || source.clip == null)
                return;

            source.volume = GetCategoryVolume(category);
        }

        private void LoadVolumes()
        {
            _masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
            _musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
            _sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
            _uiVolume = PlayerPrefs.GetFloat(UiVolumeKey, 1f);
            _ambientVolume = PlayerPrefs.GetFloat(AmbientVolumeKey, 1f);
        }

        private void SaveVolumes()
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, _masterVolume);
            PlayerPrefs.SetFloat(MusicVolumeKey, _musicVolume);
            PlayerPrefs.SetFloat(SfxVolumeKey, _sfxVolume);
            PlayerPrefs.SetFloat(UiVolumeKey, _uiVolume);
            PlayerPrefs.SetFloat(AmbientVolumeKey, _ambientVolume);
            PlayerPrefs.Save();
        }

        private static float ClampVolume(float value)
        {
            return Mathf.Clamp01(value);
        }
    }
}
