using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace XqLua.Sample {
    public class LandingScene : MonoBehaviour {
        [SerializeField] private CanvasGroup _routesGroup = default;
        [SerializeField] private CanvasGroup _backGroup = default;

        private string _currentScene = default;

        private void Awake() {
            _backGroup.interactable = false;
        }

        public void OnSceneCalled(string sceneName) {
            _routesGroup.alpha = 0.0f;
            _routesGroup.interactable = false;
            _routesGroup.blocksRaycasts = false;
            _currentScene = sceneName;
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string name) {
            yield return SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
            _backGroup.interactable = true;
        }

        public void OnBackCalled() {
            _backGroup.interactable = false;
            StartCoroutine(UnloadSceneAsync());
        }
        private IEnumerator UnloadSceneAsync() {
            yield return SceneManager.UnloadSceneAsync(_currentScene);
            _routesGroup.alpha = 1.0f;
            _routesGroup.interactable = true;
            _routesGroup.blocksRaycasts = true;

        }
    }
}