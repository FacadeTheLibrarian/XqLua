using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.BehaviourSample {
    public class DelayPresenter : MonoBehaviour {
        [SerializeField] private PublisherButton _publisher = default;
        [SerializeField] private Arrow _publisherToDelay = default;
        [SerializeField] private WithDelayOperator _withDelay = default;
        [SerializeField] private Arrow _delayToSubscriber = default;
        [SerializeField] private Subscriber _subscriber = default;

        private Disposables _disposables = default;

        public void Start() {
            _disposables = new Disposables();
            _publisher.OnButtonClicked
                .Subscribe(_ => _withDelay.OnPublisherInvoked())
                .AddTo(_disposables);
            _publisher.OnButtonClicked
                .Subscribe(_ => _publisherToDelay.OnPublisherInvoked())
                .AddTo(_disposables);

            _withDelay.DelayedPublisher
                .Subscribe(_ => _delayToSubscriber.OnPublisherInvoked())
                .AddTo(_disposables);
            _withDelay.DelayedPublisher
                .Subscribe(_ => _subscriber.OnPublisherInvoked())
                .AddTo(_disposables);
        }

        public void OnDestroy() {
            _disposables.Dispose();
        }
    }
}
