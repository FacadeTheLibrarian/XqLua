using System;
using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.GameSample {
    public class CannonPresenter : MonoBehaviour, IDisposable {
        [SerializeField] private CannonTriggerView _triggerView = default;
        [SerializeField] private CannonReloadView _reloadView = default;
        [SerializeField] private Cannon _model = default;

        private Disposables _disposables = default;
        public CannonPresenter Initialize() {
            _disposables = new Disposables();

            _triggerView = _triggerView.Initialize().AddTo(_disposables);
            _reloadView = _reloadView.Initialize().AddTo(_disposables);

            _triggerView.OnButtonClicked.Subscribe(_ => _model.Fire()).AddTo(_disposables);
            _reloadView.OnButtonClicked.Subscribe(_ => _model.Reload()).AddTo(_disposables);
            return this;
        }
        public void Dispose() {
            _disposables.Dispose();
        }
    }
}