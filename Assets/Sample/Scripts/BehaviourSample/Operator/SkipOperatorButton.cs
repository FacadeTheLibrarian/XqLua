using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using XqLua.Extension;
using XqLua.Unity.Extension;

namespace XqLua.Sample.BehaviourSample {
    public class SkipOperatorButton : MonoBehaviour {
        [SerializeField] private Button _button = default;
        [SerializeField] private Image _buttonImage = default;
        [SerializeField] private Text _text = default;
        [SerializeField] private int _defaultSkipCount = 2;
        [SerializeField] private float _highLightDuration = 1.0f;

        private bool _isCoroutineRunning = false;
        private bool _shouldSkip = false;
        private int _skipCount = default;

        private Disposables _disposables = default;
        public void Awake() {
            _disposables = new Disposables();
            _button.OnClickAsPublisher().Subscribe(_ => {
                _shouldSkip = false;
                _skipCount = _defaultSkipCount;
                _text.text = _defaultSkipCount.ToString();
                if (!_isCoroutineRunning) {
                    _isCoroutineRunning = true;
                    StartCoroutine(HighLight());
                }
            }).AddTo(_disposables);
            _text.text = _defaultSkipCount.ToString();
            _skipCount = _defaultSkipCount;
        }

        public void OnDestroy() {
            _disposables.Dispose();
        }

        public bool TryConsumeCount() {
            if (!_isCoroutineRunning) {
                StartCoroutine(HighLight());
            }
            if (_shouldSkip) {
                return true;
            }
            else {
                _skipCount--;
                if (_skipCount < 0) {
                    _shouldSkip = true;
                    return true;
                }
                _text.text = _skipCount.ToString();
                return false;
            }
        }
        public bool IsConditionMet() {
            return _shouldSkip;
        }

        private IEnumerator HighLight() {
            _buttonImage.color = Color.yellow;
            float elapsedTime = 0.0f;
            float highLightDuration = _highLightDuration;
            while (elapsedTime < highLightDuration) {
                elapsedTime += Time.deltaTime;
                Color lerped = Color.Lerp(Color.yellow, Color.white, (elapsedTime / highLightDuration));
                _buttonImage.color = lerped;
                yield return null;
            }
            _isCoroutineRunning = false;
        }
    }
}
