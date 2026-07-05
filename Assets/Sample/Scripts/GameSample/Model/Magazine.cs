using System;
using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.GameSample {
    public class Magazine : MonoBehaviour, IDisposable {
        public IReactiveProperty<int> Ammo => _ammo;
        public IPublisher<Empty> OnRunOutOfAmmo => _onRunOutOfAmmo;
        public IPublisher<Empty> OnReloaded => _onReloaded;

        [SerializeField] private ReactiveProperty<int> _ammo = default;
        [SerializeField] private int _capacity = 10;

        private Publisher<Empty> _onRunOutOfAmmo = default;
        private Publisher<Empty> _onReloaded = default;

        private Disposables _disposables = default;

        public Magazine Initialize() {
            _disposables = new Disposables();
            _onRunOutOfAmmo = new Publisher<Empty>().AddTo(_disposables);
            _onReloaded = new Publisher<Empty>().AddTo(_disposables);
            _ammo = new ReactiveProperty<int>(_capacity).AddTo(_disposables);
            return this;
        }
        public void Dispose() {
            _disposables.Dispose();
        }

        public bool TryConsumeAmmo() {
            if (_ammo.Value <= 0) {
                return false;
            }

            _ammo.Value--;
            if (_ammo.Value <= 0) {
                _onRunOutOfAmmo.Invoke(Empty.Default);
            }
            return true;
        }

        public bool TryReloadOne() {
            if (_ammo.Value >= _capacity) {
                return false;
            }
            _ammo.Value++;

            if (_ammo.Value >= _capacity) {
                _onReloaded.Invoke(Empty.Default);
            }
            return true;
        }
    }
}