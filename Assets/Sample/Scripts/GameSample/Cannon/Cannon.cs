using System;
using System.Collections;
using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.GameSample {
    public class Cannon : MonoBehaviour, IDisposable {

        public IPublisher<Empty> OnFire => _onFire;

        [SerializeField] private CannonBall _cannonBall = default;
        [SerializeField] private Muzzle _muzzle = default;

        [SerializeField] private float _secondToTarget = 1.0f;
        [SerializeField] private float _reloadInterval = 0.125f;

        private Publisher<Empty> _onFire = default;

        private Magazine _magazine = default;
        private Spotter _spotter = default;

        private bool _isFiring = false;
        private bool _isReloading = false;

        private Disposables _disposables = default;

        public Cannon Initialize(Magazine magazine, Spotter spotter) {
            _disposables = new Disposables();

            _onFire = new Publisher<Empty>().AddTo(_disposables);

            _cannonBall = _cannonBall.Initialize().AddTo(_disposables);
            _muzzle = _muzzle.Initialize().AddTo(_disposables);
            _magazine = magazine;
            _spotter = spotter;
            return this;
        }

        public void Dispose() {
            _disposables.Dispose();
        }

        public void Fire() {
            if (_isFiring || _isReloading) {
                return;
            }
            if (!_magazine.TryConsumeAmmo()) {
                return;
            }
            if (!_spotter.TrySpot(out Target target)) {
                return;
            }
            _isFiring = true;

            //NOTE: 投げっぱなし
            IEnumerator coroutineSource = FireCore(target, _secondToTarget);
            StartCoroutine(coroutineSource);
        }
        private IEnumerator FireCore(Target target, float secondToTarget) {
            _onFire.Invoke(Empty.Default);
            _muzzle.Fire();
            yield return _cannonBall.Fire(_muzzle.transform, target, secondToTarget);
            _isFiring = false;
        }
        public void Reload() {
            if (_isFiring || _isReloading) {
                return;
            }
            _isReloading = true;

            //NOTE: 投げっぱなし
            IEnumerator coroutineSource = ReloadCore();
            StartCoroutine(coroutineSource);
        }
        public IEnumerator ReloadCore() {
            while (_magazine.TryReloadOne()) {
                yield return new WaitForSeconds(_reloadInterval);
            }
            _isReloading = false;
        }
    }
}