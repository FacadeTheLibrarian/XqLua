using System;
using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.GameSample {
    public class HealthPoint : MonoBehaviour, IDisposable {
        public IReactiveProperty<int> Hp => _hp;
        public IPublisher<Empty> OnDefeated => _onDefeated;
        public IPublisher<Empty> OnRecovered => _onRecovered;
        public IPublisher<Empty> OnDangerZone => _onDangerZone;

        [SerializeField] private ReactiveProperty<int> _hp = default;
        private Publisher<Empty> _onDefeated = default;
        private Publisher<Empty> _onRecovered = default;
        private Publisher<Empty> _onDangerZone = default;

        private int _maxHp = 0;
        private int _dangerZone = 0;
        private bool _isAlerted = false;

        private Disposables _disposables = default;
        public HealthPoint Initialize(int maxHp, float dangerRatio) {
            _disposables = new Disposables();
            _maxHp = maxHp;
            _dangerZone = (int)((float)maxHp * dangerRatio);
            _hp = new ReactiveProperty<int>(maxHp).AddTo(_disposables);
            _onDefeated = new Publisher<Empty>().AddTo(_disposables);
            _onRecovered = new Publisher<Empty>().AddTo(_disposables);
            _onDangerZone = new Publisher<Empty>().AddTo(_disposables);
            return this;
        }
        public void Dispose() {
            _disposables.Dispose();
        }

        public void TakeDamage(int amount) {
            int remain = _hp.Value - amount;
            bool shouldAlert = remain < _dangerZone;
            if (!_isAlerted && shouldAlert) {
                _onDangerZone.Invoke(Empty.Default);
            }
            if (remain > 0) {
                _hp.Value = remain;
            }
            else {
                _hp.Value = 0;
                _isAlerted = false;
                _onDefeated.Invoke(Empty.Default);
            }
        }

        public bool TryHealOne() {
            if (_hp.Value >= _maxHp) {
                _onRecovered.Invoke(Empty.Default);
                return false;
            }
            else {
                _hp.Value++;
                return true;
            }
        }
    }
}