using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using XqLua.Extension;
using XqLua.Unity.Extension;

namespace XqLua.Sample.BehaviourSample {
    public class TakeOperatorButton : MonoBehaviour {
        [SerializeField] private Button _button = default;
        [SerializeField] private Image _buttonImage = default;
        [SerializeField] private Text _text = default;
        [SerializeField] private int _defaultTakeCount = 2;
        [SerializeField] private float _highLightDuration = 1.0f;

        private bool _isCoroutineRunning = false;
        private bool _shouldSkip = false;
        private int _takeCount = default;

        private Disposables _disposables = default;
        public void Awake() {
            _disposables = new Disposables();
            _button.OnClickAsPublisher().Subscribe(_ => {
                _shouldSkip = false;
                _takeCount = _defaultTakeCount;
                _text.text = _defaultTakeCount.ToString();
                if (!_isCoroutineRunning) {
                    _isCoroutineRunning = true;
                    StartCoroutine(HighLight());
                }
            }).AddTo(_disposables);
            _text.text = _defaultTakeCount.ToString();
            _takeCount = _defaultTakeCount;
        }

        public void OnDestroy() {
            _disposables.Dispose();
        }

        public bool TryConsumeCount() {
            if (_shouldSkip) {
                return false;
            }
            else {
                _takeCount--;
                if (_takeCount < 0) {
                    _shouldSkip = true;
                    return false;
                }
                _text.text = _takeCount.ToString();
                if (!_isCoroutineRunning) {
                    StartCoroutine(HighLight());
                }
                return true;
            }
        }
        public bool IsConditionMet() {
            return !_shouldSkip;
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
