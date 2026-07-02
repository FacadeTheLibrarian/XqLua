using UnityEngine;
using UnityEngine.UI;
using XqLua.Extension;
using XqLua.Unity.Extension;

namespace XqLua.Sample.BehaviourSample {
    public class PublisherButton : MonoBehaviour {

        public IPublisher<Empty> OnButtonClicked => _onButtonClicked;

        [SerializeField] private Button _button = default;

        private Publisher<Empty> _onButtonClicked = default;
        private Disposables _disposables = default;

        public void Awake() {
            _disposables = new Disposables();
            _onButtonClicked = new Publisher<Empty>().AddTo(_disposables);

            _button.OnClickAsPublisher().Subscribe(_onButtonClicked.Invoke).AddTo(_disposables);
        }

        public void OnDestroy() {
            _disposables.Dispose();
        }
    }
}