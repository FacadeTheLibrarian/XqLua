
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace XqLua.Sample.BehaviourSample {
    public class OnConditionOperatorToggle : MonoBehaviour {
        [SerializeField] private Toggle _toggle = default;
        [SerializeField] private Image _toggleImage = default;
        [SerializeField] private float _highLightDuration = 1.0f;

        public bool IsConditionMet {
            get {
                if (!_isCoroutineRunning && _toggle.isOn) {
                    _isCoroutineRunning = true;
                    StartCoroutine(HighLight());
                }
                return _toggle.isOn;
            }
        }
        private bool _isCoroutineRunning = false;

        private IEnumerator HighLight() {
            _toggleImage.color = Color.yellow;
            float elapsedTime = 0.0f;
            float highLightDuration = _highLightDuration;
            while (elapsedTime < highLightDuration) {
                elapsedTime += Time.deltaTime;
                Color lerped = Color.Lerp(Color.yellow, Color.white, (elapsedTime / highLightDuration));
                _toggleImage.color = lerped;
                yield return null;
            }
            _isCoroutineRunning = false;
        }
    }
}
