
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace XqLua.Sample.BehaviourSample {
    public class OnValueIsOperator : MonoBehaviour {
        [SerializeField] private InputField _inputField = default;
        [SerializeField] private Dropdown _dropdown = default;
        [SerializeField] private Image _image = default;
        [SerializeField] private float _highLightDuration = 1.0f;

        private bool _isCoroutineRunning = false;

        public bool IsConditionMet(int rhs) {
            if (!_isCoroutineRunning) {
                _isCoroutineRunning = true;
                StartCoroutine(HighLight());
            }
            int lhs = 0;
            if (!int.TryParse(_inputField.text, out lhs)) {
                _inputField.text = "0";
                lhs = 0;
            }
            switch (_dropdown.value) {
                case (0):
                    return rhs > lhs;
                case (1):
                    return rhs < lhs;
                case (2):
                    return rhs >= lhs;
                case (3):
                    return rhs <= lhs;
                case (4):
                    return rhs == lhs;
                default:
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
