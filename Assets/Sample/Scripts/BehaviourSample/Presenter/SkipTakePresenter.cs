using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.BehaviourSample {
    public class SkipTakePresenter : MonoBehaviour {
        [SerializeField] private PublisherButton _publisher = default;
        [SerializeField] private Arrow _publisherToSkip = default;
        [SerializeField] private SkipOperatorButton _skip = default;
        [SerializeField] private Arrow _skipToTake = default;
        [SerializeField] private TakeOperatorButton _take = default;
        [SerializeField] private Arrow _takeToSubscriber = default;
        [SerializeField] private Subscriber _subscriber = default;

        private Publisher<Empty> _onSkip = default;

        private Disposables _disposables = default;

        public void Start() {
            _disposables = new Disposables();
            _onSkip = new Publisher<Empty>().AddTo(_disposables);

            _publisher.OnButtonClicked
                .Subscribe(_ => _publisherToSkip.OnPublisherInvoked())
                .AddTo(_disposables);

            _publisher.OnButtonClicked
                .OnCondition(() => _skip.TryConsumeCount())
                .Subscribe(_onSkip.Invoke)
                .AddTo(_disposables);

            _publisher.OnButtonClicked
                .OnCondition(() => _skip.IsConditionMet())
                .Subscribe(_ => _skipToTake.OnPublisherInvoked())
                .AddTo(_disposables);

            _onSkip
                .OnCondition(() => _take.TryConsumeCount())
                .Subscribe(_ => _subscriber.OnPublisherInvoked())
                .AddTo(_disposables);

            _onSkip
                .OnCondition(() => _take.IsConditionMet())
                .Subscribe(_ => _takeToSubscriber.OnPublisherInvoked())
                .AddTo(_disposables);
        }

        public void OnDestroy() {
            _disposables.Dispose();
        }
    }
}
