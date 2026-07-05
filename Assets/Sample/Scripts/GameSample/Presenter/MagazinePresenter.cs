using System;
using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.GameSample {
    public class MagazinePresenter : MonoBehaviour, IDisposable {

        [SerializeField] private MagazineView _magazineView = default;
        [SerializeField] private ReloadWarningView _reloadView = default;

        private Disposables _disposables = default;

        public MagazinePresenter Initialize(Magazine model) {
            _disposables = new Disposables();
            _magazineView = _magazineView.Initialize().AddTo(_disposables);
            _reloadView = _reloadView.Initialize().AddTo(_disposables);

            model.Ammo.Subscribe(_magazineView.OnAmmoCountChanged).AddTo(_disposables);
            model.OnRunOutOfAmmo.Subscribe(_ => _reloadView.OnReloadRequired()).AddTo(_disposables);
            model.OnReloaded.Subscribe(_ => _reloadView.OnReloaded()).AddTo(_disposables);

            return this;
        }
        public void Dispose() {
            _disposables.Dispose();
        }
    }
}
