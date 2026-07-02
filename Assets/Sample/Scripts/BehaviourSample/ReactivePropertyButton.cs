using UnityEngine;
using UnityEngine.UI;
using XqLua.Extension;
using XqLua.Unity.Extension;

namespace XqLua.Sample.BehaviourSample {
    public class ReactivePropertyButton : MonoBehaviour {
        public IReactiveProperty<int> Count => _count;

        [SerializeField] private Button _button = default;
        [SerializeField] private Button _reset = default;
        [SerializeField] private Button _force = default;
        [SerializeField] private Text _text = default;

        private ReactiveProperty<int> _count = default;
        private Disposables _disposables = default;

        public void Awake() {
            _disposables = new Disposables();
            _count = new ReactiveProperty<int>(0).AddTo(_disposables);

            _button.OnClickAsPublisher().Subscribe(_ => {
                _count.Value++;
                _text.text = _count.Value.ToString();
            }).AddTo(_disposables);

            _reset.OnClickAsPublisher().Subscribe(_ => {
                _text.text = "0";
                _count.Value = 0;
            }).AddTo(_disposables);

            _force.OnClickAsPublisher().Subscribe(_ => {
                _count.SetForceNotify(_count.Value);
            }).AddTo(_disposables);
        }

        public void OnDestroy() {
            _disposables.Dispose();
        }
    }
}