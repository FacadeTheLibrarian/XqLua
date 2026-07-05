using System;
using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.GameSample {
    public class EnemyHealthPointPresenter : MonoBehaviour, IDisposable {
        [SerializeField] private EnemyHealthPointView _view = default;
        [SerializeField] private HealthPoint _model = default;

        private Disposables _disposables = default;

        public EnemyHealthPointPresenter Initialize() {
            _disposables = new Disposables();
            _view = _view.Initialize(_model).AddTo(_disposables);
            return this;
        }

        public void Dispose() {
            _disposables.Dispose();
        }
    }
}
