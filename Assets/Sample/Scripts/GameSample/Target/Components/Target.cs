using System;
using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.GameSample {
    public class Target : MonoBehaviour, IDisposable {
        public IPublisher<ITargetEvents> OnTargetEvent => _onTargetEvent;

        private Publisher<ITargetEvents> _onTargetEvent = default;
        private int _id = default;

        private Disposables _disposables = default;

        public Target Initialize(int id) {
            _disposables = new Disposables();
            _id = id;

            _onTargetEvent = new Publisher<ITargetEvents>().AddTo(_disposables);

            return this;
        }

        public void Dispose() {
            _disposables.Dispose();
        }

        public void Hit(int damage) {
            TargetHit hit = new TargetHit(_id, this.transform.position, damage);
            _onTargetEvent.Invoke(hit);
        }
    }
}