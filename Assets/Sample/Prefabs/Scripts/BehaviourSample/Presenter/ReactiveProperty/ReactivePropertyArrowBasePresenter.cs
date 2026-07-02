using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.BehaviourSample {
    public class ReactivePropertyArrowBasePresenter : MonoBehaviour {
        [SerializeField] private ReactivePropertyButton _model = default;
        [SerializeField] private ArrowBase _view = default;

        private Disposables _disposables = default;

        public void Start() {
            _disposables = new Disposables();
            _model.Count.Subscribe(_ => _view.OnPublisherInvoked()).AddTo(_disposables);
        }

        public void OnDestroy() {
            _disposables.Dispose();
        }
    }
}