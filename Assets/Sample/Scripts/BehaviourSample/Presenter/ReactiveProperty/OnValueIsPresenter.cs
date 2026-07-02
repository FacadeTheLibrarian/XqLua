using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.BehaviourSample {
    public class OnValueIsPresenter : MonoBehaviour {
        [SerializeField] private ReactivePropertyButton _reactiveProperty = default;
        [SerializeField] private Arrow _publisherToOnValueIs = default;
        [SerializeField] private OnValueIsOperator _onValueIs = default;
        [SerializeField] private Arrow _onValueIsToSubscriber = default;
        [SerializeField] private ReactivePropertySubscriber _subscriber = default;

        private Disposables _disposables = default;

        public void Start() {
            _disposables = new Disposables();

            _reactiveProperty.Count
                .Subscribe(_ => _publisherToOnValueIs.OnPublisherInvoked())
                .AddTo(_disposables);
            _reactiveProperty.Count
                .OnValueIs(value => _onValueIs.IsConditionMet(value))
                .Subscribe(value => {
                    _onValueIsToSubscriber.OnPublisherInvoked();
                    _subscriber.OnValueSet(value);
                })
                .AddTo(_disposables);

        }
        public void OnDestroy() {
            _disposables.Dispose();
        }
    }
}