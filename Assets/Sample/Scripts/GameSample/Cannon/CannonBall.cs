using System;
using System.Collections;
using UnityEngine;

namespace XqLua.Sample.GameSample {
    public class CannonBall : MonoBehaviour, IDisposable {

        [SerializeField] private Renderer _renderer = default;
        [SerializeField] private ParticleSystem _trail = default;

        [SerializeField] private int _damage = 10;

        public CannonBall Initialize() {
            _renderer.enabled = false;
            return this;
        }

        public void Dispose() {

        }

        public IEnumerator Fire(Transform from, Target to, float secondToTarget) {
            this.transform.position = from.position;
            Vector3 difference = to.transform.position - from.position;
            float elapsedTime = 0.0f;

            _renderer.enabled = true;
            _trail.Play();
            while (elapsedTime < secondToTarget) {
                float unscaledDeltaTime = Time.unscaledDeltaTime;
                elapsedTime += unscaledDeltaTime;
                this.transform.position += (difference * unscaledDeltaTime) / secondToTarget;
                yield return null;
            }

            to.Hit(_damage);
            _trail.Stop();
            _renderer.enabled = false;
        }
    }
}