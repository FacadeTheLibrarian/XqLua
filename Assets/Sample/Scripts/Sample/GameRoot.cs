
using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.GameSample {
    public class GameRoot : MonoBehaviour {
        [SerializeField] private Cannon _cannon = default;
        [SerializeField] private Magazine _magazine = default;
        [SerializeField] private Spotter _spotter = default;

        [SerializeField] private TargetManager _targetManager = default;

        [SerializeField] private CannonPresenter _cannonPresenter = default;
        [SerializeField] private MagazinePresenter _magazinePresenter = default;

        private Disposables _disposables = default;
        public void Awake() {
            _disposables = new Disposables();

            _targetManager = _targetManager.Initialize().AddTo(_disposables);

            _magazine = _magazine.Initialize().AddTo(_disposables);
            _spotter = _spotter.Initialize(_targetManager.Targets).AddTo(_disposables);

            _cannon = _cannon.Initialize(_magazine, _spotter).AddTo(_disposables);

            _cannonPresenter = _cannonPresenter.Initialize().AddTo(_disposables);
            _magazinePresenter = _magazinePresenter.Initialize(_magazine).AddTo(_disposables);
        }

        public void OnDestroy() {
            _disposables.Dispose();
        }
    }
}
