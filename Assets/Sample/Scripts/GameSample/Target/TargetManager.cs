
using System;
using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.GameSample {
    public class TargetManager : MonoBehaviour, IDisposable {

        public Target[] Targets => _targets;

        [SerializeField] private Target[] _targets = default;
        [SerializeField] private ParticleApplier _applier = default;

        private Disposables _disposables = default;

        public TargetManager Initialize() {
            _disposables = new Disposables();
            _applier = _applier.Initialize().AddTo(_disposables);

            for (int i = 0; i < _targets.Length; i++) {
                _targets[i] = _targets[i].Initialize(i).AddTo(_disposables);
                _targets[i].OnTargetEvent.Subscribe(OnTargetAction).AddTo(_disposables);
            }
            return this;
        }

        public void Dispose() {
            _disposables.Dispose();
        }

        private void OnTargetAction(ITargetEvents message) {
            switch (message) {
                case (TargetHit hit):
                    _applier.Play(0, hit.Position);
                    break;
                default:
                    break;
            }
        }
    }
}
