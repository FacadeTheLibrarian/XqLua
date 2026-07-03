using UnityEngine;

namespace XqLua.Sample.BadSample {
    public class Target : MonoBehaviour {
        [SerializeField] private ParticleSystem _hitParticle = default;
        public void Hit() {
            _hitParticle.Play();
        }
    }
}
