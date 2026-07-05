using System;
using System.Collections;
using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.GameSample {
    public class Target : MonoBehaviour, IDisposable {
        public IPublisher<ITargetEvents> OnTargetEvent => _onTargetEvent;

        private Publisher<ITargetEvents> _onTargetEvent = default;

        [SerializeField] private HealthPoint _healthPoint = default;
        [SerializeField] private int _maxHp = 100;
        [SerializeField] private float _dangerZoneRatio = 0.325f;

        [SerializeField] private float _defeatedWaitSecond = 1.0f;
        [SerializeField] private float _secondToHeal = 1.0f;

        private int _id = default;
        private bool _isRecovering = false;

        private Disposables _disposables = default;

        public Target Initialize(int id) {
            _disposables = new Disposables();
            _id = id;

            _onTargetEvent = new Publisher<ITargetEvents>().AddTo(_disposables);
            _healthPoint = _healthPoint.Initialize(_maxHp, _dangerZoneRatio).AddTo(_disposables);

            _healthPoint.OnDefeated.Subscribe(_ => OnDefeated()).AddTo(_disposables);

            return this;
        }

        public void Dispose() {
            _disposables.Dispose();
        }

        public void Hit(int damage) {
            if (_isRecovering) {
                return;
            }
            TargetHit hit = new TargetHit(_id, this.transform.position, damage);
            _onTargetEvent.Invoke(hit);

            _healthPoint.TakeDamage(damage);
        }

        private void OnDefeated() {
            _isRecovering = true;
            IEnumerator coroutineSource = Recover();
            StartCoroutine(coroutineSource);
        }

        private IEnumerator Recover() {
            yield return new WaitForSeconds(_defeatedWaitSecond);

            float interval = _secondToHeal / (float)_maxHp;
            while (_healthPoint.TryHealOne()) {
                yield return new WaitForSeconds(interval);
            }
            _isRecovering = false;
        }
    }
}