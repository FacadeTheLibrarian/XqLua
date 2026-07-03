using System;
using UnityEngine;
using XqLua;

namespace XqLua.Sample.GameSample {
    public class HealthPoint : MonoBehaviour, IDisposable {
        private ReactiveProperty<int> _hp = default;

        public HealthPoint Initialize() {
            _hp = new ReactiveProperty<int>();
            return this;
        }
        public void Dispose() {
            _hp.Dispose();
        }

        public void TakeDamage(int amount) {
            int remain = Mathf.Max(0, _hp.Value - amount);
            _hp.Value = remain;
        }
    }
}