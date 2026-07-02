using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using XqLua.Extension;

namespace XqLua.Sample.BehaviourSample {
    public class WithDelayOperator : MonoBehaviour {
        public IPublisher<Empty> DelayedPublisher => _delayedPublisher;
        public bool IsRunning => _isRunning;

        [SerializeField] private Text _text = default;
        [SerializeField] private Image _image = default;
        [SerializeField] private Image _clock = default;
        [SerializeField] private float _delayTime = 1.0f;
        [SerializeField] private float _highLightDuration = 1.0f;
        private Publisher<Empty> _delayedPublisher = default;
        private bool _isRunning = false;

        private Disposables _disposables = default;
        public void Awake() {
            _disposables = new Disposables();
            _delayedPublisher = new Publisher<Empty>().AddTo(_disposables);
            _text.text = 0.0f.ToString("F3");
            _clock.fillAmount = 1.0f;
        }
        public void OnDestroy() {
            _disposables.Dispose();
        }

        public void OnPublisherInvoked() {
            if (_isRunning) {
                return;
            }
            _isRunning = true;
            StartCoroutine(DelayedInvoke());
        }

        public IEnumerator DelayedInvoke() {
            float elapsedTime = 0.0f;
            float delayTime = _delayTime;
            while (elapsedTime < delayTime) {
                elapsedTime += Time.deltaTime;
                _text.text = elapsedTime.ToString("F3");
                _clock.fillAmount = 1.0f - (elapsedTime / delayTime);
                yield return null;
            }
            _delayedPublisher.Invoke(Empty.Default);

            _image.color = Color.yellow;
            elapsedTime = 0.0f;
            float highLightDuration = _highLightDuration;
            while (elapsedTime < highLightDuration) {
                elapsedTime += Time.deltaTime;
                Color lerped = Color.Lerp(Color.yellow, Color.white, (elapsedTime / highLightDuration));
                _image.color = lerped;
                yield return null;
            }
            _isRunning = false;
        }
    }
}
