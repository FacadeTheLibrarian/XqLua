using System;
using UnityEngine;

namespace XqLua.Sample.GameSample {
    public class ParticleApplier : MonoBehaviour, IDisposable {
        [SerializeField] private ParticleSystem _explosion = default;

        private Disposables _disposables = default;

        public ParticleApplier Initialize() {
            _disposables = new Disposables();
            return this;
        }

        public void Dispose() {
            _disposables.Dispose();
        }

        public void Play(int index, Vector3 position) {
            _explosion.transform.position = position;
            _explosion.Play();
        }
    }
}
