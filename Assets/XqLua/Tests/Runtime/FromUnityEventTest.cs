using NUnit.Framework;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using XqLua.Extension;

namespace XqLua.Test {
    public class FromUnityEventTest {

        private UnityAction<int, float, bool> _threeVariablesUnityEvent = default;
        private UnityAction<int, float, bool, string> _fourVariablesUnityEvent = default;

        private Disposables _disposables = default;

        [SetUp]
        public void SetUp() {
            _disposables = new Disposables();
        }

        /// <summary>
        /// FromUnityEventでUnityEventをラップして購読できるか?
        /// </summary>
        [UnityTest]
        public IEnumerator FromEventShouldWork() {
            bool sceneLoadedInvoked = false;
            bool sceneUnloadedInvoked = false;

            bool threeVariablesEventInvoked = false;
            bool fourVariablesEventInvoked = false;

            Publisher<Scene, LoadSceneMode>.FromUnityEvent(
                action => SceneManager.sceneLoaded += action,
                action => SceneManager.sceneLoaded -= action
            ).Subscribe((scene, mode) => {
                UnityEngine.Debug.Log($"Scene loaded: {scene.name}, Mode: {mode}");
                sceneLoadedInvoked = true;
            }).AddTo(_disposables);

            Publisher<Scene>.FromUnityEvent(
                action => SceneManager.sceneUnloaded += action,
                action => SceneManager.sceneUnloaded -= action
            ).Subscribe(scene => {
                UnityEngine.Debug.Log($"Scene unloaded: {scene.name}");
                sceneUnloadedInvoked = true;
            }).AddTo(_disposables);

            Publisher<int, float, bool>.FromUnityEvent(
                action => _threeVariablesUnityEvent += action,
                action => _threeVariablesUnityEvent -= action
            ).Subscribe((i, f, b) => {
                UnityEngine.Debug.Log($"Three variables event invoked: {i}, {f}, {b}");
                threeVariablesEventInvoked = true;
            }).AddTo(_disposables);

            Publisher<int, float, bool, string>.FromUnityEvent(
                action => _fourVariablesUnityEvent += action,
                action => _fourVariablesUnityEvent -= action
            ).Subscribe((i, f, b, s) => {
                UnityEngine.Debug.Log($"Four variables event invoked: {i}, {f}, {b}, {s}");
                fourVariablesEventInvoked = true;
            }).AddTo(_disposables);

            yield return SceneManager.LoadSceneAsync("TestScene", LoadSceneMode.Additive);

            Assert.IsTrue(sceneLoadedInvoked, "Scene loaded event was not invoked.");

            yield return SceneManager.UnloadSceneAsync("TestScene");

            Assert.IsTrue(sceneUnloadedInvoked, "Scene unloaded event was not invoked.");

            _threeVariablesUnityEvent?.Invoke(1, 2.0f, true);
            _fourVariablesUnityEvent?.Invoke(1, 2.0f, true, "test");

            Assert.IsTrue(threeVariablesEventInvoked, "Three variables event was not invoked.");
            Assert.IsTrue(fourVariablesEventInvoked, "Four variables event was not invoked.");
        }

        [TearDown]
        public void TearDown() {
            _disposables.Dispose();
        }
    }
}