using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace XqLua.Sample.BehaviourSample {
    public class OnValueChangedOperator : MonoBehaviour {
        [SerializeField] private Image _image = default; [SerializeField] private float _highLightDuration = 1.0f;

        private int _lastValue = int.MinValue;
        private bool _isCoroutineRunning = false;

        public bool IsConditionMet(int lastValue) {
            if (!_isCoroutineRunning) {
                _isCoroutineRunning = true;
                StartCoroutine(HighLight());
            }
            if (lastValue != _lastValue) {
                _lastValue = lastValue;
                return true;
            }
            else {
                return false;
            }
        }

        private IEnumerator HighLight() {
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
