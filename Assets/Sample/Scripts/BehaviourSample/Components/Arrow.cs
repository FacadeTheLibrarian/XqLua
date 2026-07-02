using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace XqLua.Sample.BehaviourSample {
    public class Arrow : MonoBehaviour {
        [SerializeField] private Image _image = default;
        [SerializeField] private Image _left = default;
        [SerializeField] private Image _right = default;
        [SerializeField] private float _highLightDuration = 1.0f;

        private bool _isCoroutineRunning = false;

        public void OnPublisherInvoked() {
            if (_isCoroutineRunning) {
                return;
            }
            else {
                _isCoroutineRunning = true;
                StartCoroutine(HighLight());
            }
        }

        private IEnumerator HighLight() {
            float elapsedTime = 0.0f;
            while (elapsedTime < _highLightDuration) {
                elapsedTime += Time.deltaTime;
                Color lerped = Color.Lerp(Color.yellow, Color.white, (elapsedTime / _highLightDuration));
                _image.color = lerped;
                _left.color = lerped;
                _right.color = lerped;
                yield return null;
            }
            _isCoroutineRunning = false;
        }
    }
}