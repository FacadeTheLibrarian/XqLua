using System;
using UnityEngine;

namespace XqLua.Sample.GameSample {
    public class AudioSourceService : MonoBehaviour, IDisposable {
        [SerializeField] private AudioSource[] _audioSources = default;
        private int _playingIndex = 0;

        public AudioSourceService Initialize() {
            foreach (AudioSource audioSource in _audioSources) {
                audioSource.playOnAwake = false;
                audioSource.loop = false;
            }
            return this;
        }

        public void Dispose() {
            
        }

        public void PlayOneShot(AudioClip clip) {
            _audioSources[_playingIndex].PlayOneShot(clip);
            _playingIndex = (_playingIndex + 1) % _audioSources.Length;
        }
    }
}