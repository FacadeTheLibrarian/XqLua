using UnityEngine;
using UnityEngine.UI;
using XqLua.Extension;
using XqLua.Unity.Extension;

namespace XqLua.Test {
    public class SliderTest : MonoBehaviour {
        [SerializeField] private Slider _slider = default;

        private Disposables _disposables = default;

        public void Start() {
            _disposables = new Disposables();
            _slider.OnValueChangedAsPublisher().Subscribe(value => Debug.Log($"Slider Value Changed: {value}")).AddTo(_disposables);
        }

        public void OnDestroy() {
            _disposables.Dispose();
        }
    }
}
