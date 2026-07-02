using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.BehaviourSample {
    public class OnValueChangedPresenter : MonoBehaviour {
        [SerializeField] private ReactivePropertyButton _reactiveProperty = default;
        [SerializeField] private Arrow _publisherToOnValueChanged = default;
        [SerializeField] private OnValueChangedOperator _onValueChanged = default;
        [SerializeField] private Arrow _onValueChangedToSubscriber = default;
        [SerializeField] private ReactivePropertySubscriber _subscriber = default;

        private Disposables _disposables = default;

        public void Start() {
            _disposables = new Disposables();

            _reactiveProperty.Count
                .Subscribe(_ => _publisherToOnValueChanged.OnPublisherInvoked())
                .AddTo(_disposables);
            _reactiveProperty.Count
                .OnValueIs(value => _onValueChanged.IsConditionMet(value))
                .Subscribe(value => {
                    _onValueChangedToSubscriber.OnPublisherInvoked();
                    _subscriber.OnValueSet(value);
                })
                .AddTo(_disposables);

        }
        public void OnDestroy() {
            _disposables.Dispose();
        }
    }
}