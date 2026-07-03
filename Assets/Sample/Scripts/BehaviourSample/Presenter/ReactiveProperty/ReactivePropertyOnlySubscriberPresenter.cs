using UnityEngine;
using XqLua.Extension;

namespace XqLua.Sample.BehaviourSample {
    public class ReactivePropertyOnlySubscriberPresenter : MonoBehaviour {
        [SerializeField] private ReactivePropertyButton _publisher = default;
        [SerializeField] private Arrow _arrow = default;
        [SerializeField] private ReactivePropertySubscriber _subscriber = default;

        private Disposables _disposables = default;

        public void Start() {
            _disposables = new Disposables();
            _publisher.Count.Subscribe(_subscriber.OnValueSet).AddTo(_disposables);
            _publisher.Count.Subscribe(_ => _arrow.OnPublisherInvoked()).AddTo(_disposables);
        }

        public void OnDestroy() {
            _disposables.Dispose();
        }
    }
}