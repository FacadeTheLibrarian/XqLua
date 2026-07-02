using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.BehaviourSample {
    public class OnConditionPresenter : MonoBehaviour {

        [SerializeField] private PublisherButton _publisher = default;
        [SerializeField] private Arrow _publisherToOperator = default;
        [SerializeField] private OnConditionOperatorToggle _operator = default;
        [SerializeField] private Arrow _operatorToSubscriber = default;
        [SerializeField] private Subscriber _subscriber = default;

        private Disposables _disposables = default;

        public void Start() {
            _disposables = new Disposables();
            _publisher.OnButtonClicked
                .OnCondition(() => _operator.IsConditionMet)
                .Subscribe(_ => _subscriber.OnPublisherInvoked())
                .AddTo(_disposables);
            _publisher.OnButtonClicked
                .Subscribe(_ => _publisherToOperator.OnPublisherInvoked())
                .AddTo(_disposables);
            _publisher.OnButtonClicked
                .OnCondition(() => _operator.IsConditionMet)
                .Subscribe(_ => _operatorToSubscriber.OnPublisherInvoked())
                .AddTo(_disposables);
        }

        public void OnDestroy() {
            _disposables.Dispose();
        }
    }
}
