using System;
using UnityEngine;

namespace XqLua.Sample.GameSample {
    public class Muzzle : MonoBehaviour, IDisposable {
        [SerializeField] private ParticleSystem _muzzleFlash = default;
        public Muzzle Initialize() {
            return this;
        }
        public void Dispose() {

        }
        public void Fire() {
            _muzzleFlash.Play();
        }
    }
}