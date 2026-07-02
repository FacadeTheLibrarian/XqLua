using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace XqLua.Sample.BehaviourSample {
    public class ReactivePropertySubscriber : MonoBehaviour {
        [SerializeField] private Text _text = default;
        [SerializeField] private Image _image = default;
        [SerializeField] private float _highLightDuration = 1.0f;

        private bool _isCoroutineRunning = false;

        public void OnValueSet(int value) {
            _text.text = value.ToString();
            if (!_isCoroutineRunning) {
                _isCoroutineRunning = true;
                StartCoroutine(ImageColor());
            }
        }

        private IEnumerator ImageColor() {
            _image.color = Color.yellow;
            float elapsedTime = 0.0f;
            float highLightDuration = _highLightDuration;
            while (elapsedTime < highLightDuration) {
                elapsedTime += Time.deltaTime;
                Color lerped = Color.Lerp(Color.yellow, Color.white, (elapsedTime / highLightDuration));
                _image.color = lerped;
                yield return null;
            }
            _isCoroutineRunning = false;
        }
    }
}
