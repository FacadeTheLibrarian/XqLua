using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.BehaviourSample {
    public class ArrowBasePresenter : MonoBehaviour {
        [SerializeField] private PublisherButton _model = default;
        [SerializeField] private ArrowBase _view = default;

        private Disposables _disposables = default;

        public void Start() {
            _disposables = new Disposables();
            _model.OnButtonClicked.Subscribe(_ => _view.OnPublisherInvoked()).AddTo(_disposables);
        }

        public void OnDestroy() {
            _disposables.Dispose();
        }
    }
}