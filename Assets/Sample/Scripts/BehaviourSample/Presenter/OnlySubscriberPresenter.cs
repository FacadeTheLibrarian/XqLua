using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.BehaviourSample {
    public class OnlySubscriberPresenter : MonoBehaviour {
        [SerializeField] private PublisherButton _publisher = default;
        [SerializeField] private Arrow _arrow = default;
        [SerializeField] private Subscriber _subscriber = default;

        private Disposables _disposables = default;

        public void Start() {
            _disposables = new Disposables();
            _publisher.OnButtonClicked.Subscribe(_ => _subscriber.OnPublisherInvoked()).AddTo(_disposables);
            _publisher.OnButtonClicked.Subscribe(_ => _arrow.OnPublisherInvoked()).AddTo(_disposables);
        }

        public void OnDestroy() {
            _disposables.Dispose();
        }
    }
}
