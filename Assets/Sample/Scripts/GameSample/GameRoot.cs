
using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.GameSample {
    public class GameRoot : MonoBehaviour {
        [SerializeField] private Cannon _cannon = default;
        [SerializeField] private Magazine _magazine = default;
        [SerializeField] private Spotter _spotter = default;

        [SerializeField] private TargetManager _targetManager = default;

        [SerializeField] private AudioAlbum _audioAlbum = default;
        [SerializeField] private AudioSourceService _audioSourceService = default;

        [SerializeField] private CannonPresenter _cannonPresenter = default;
        [SerializeField] private MagazinePresenter _magazinePresenter = default;

        [SerializeField] private EnemyHealthPointPresenter _enemyHpPresenter = default;

        private Disposables _disposables = default;
        public void Awake() {
            _disposables = new Disposables();

            _targetManager = _targetManager.Initialize().AddTo(_disposables);

            _magazine = _magazine.Initialize().AddTo(_disposables);
            _spotter = _spotter.Initialize(_targetManager.Targets).AddTo(_disposables);

            _cannon = _cannon.Initialize(_magazine, _spotter).AddTo(_disposables);

            _cannonPresenter = _cannonPresenter.Initialize().AddTo(_disposables);
            _magazinePresenter = _magazinePresenter.Initialize(_magazine).AddTo(_disposables);

            _enemyHpPresenter = _enemyHpPresenter.Initialize().AddTo(_disposables);

            _audioSourceService = _audioSourceService.Initialize().AddTo(_disposables);

            AudioController audioController = new AudioController(_cannon, _audioSourceService, _audioAlbum).AddTo(_disposables);
        }

        public void OnDestroy() {
            _disposables.Dispose();
        }
    }
}
