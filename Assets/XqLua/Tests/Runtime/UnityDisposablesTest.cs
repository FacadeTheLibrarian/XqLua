using NUnit.Framework;
using UnityEngine;
using System;
using XqLua.Extension;
using XqLua.Unity.Extension;
using UnityEngine.TestTools;
using System.Collections;

namespace XqLua.Test {
    public class UnityDisposablesTest {
        private Disposables _disposables = default;

        [SetUp]
        public void SetUp() {
            _disposables = new Disposables();
        }

        [UnityTest]
        public IEnumerator AddToGameObjectShouldBeDisposedOnDestory() {
            GameObject gameObject = new GameObject();

            yield return null;

            TestDisposable disposable = new TestDisposable().AddTo(gameObject);

            yield return null;

            GameObject.Destroy(gameObject);

            yield return null;

            Assert.IsTrue(disposable.IsDisposed);
        }

        [UnityTest]
        public IEnumerator AddToGameObjectMultipleShouldBeDisposedOnDestory() {
            GameObject gameObject = new GameObject();

            yield return null;

            TestDisposable disposable = new TestDisposable().AddTo(gameObject);
            TestDisposable disposable2 = new TestDisposable().AddTo(gameObject);

            yield return null;

            GameObject.Destroy(gameObject);

            yield return null;

            Assert.IsTrue(disposable.IsDisposed);
            Assert.IsTrue(disposable2.IsDisposed);
        }

        [UnityTest]
        public IEnumerator AddSubscriptionToGameObjectShouldBeDisposedOnDestory() {
            Publisher<Empty> publisher = new Publisher<Empty>();

            int testValue = 0;

            GameObject gameObject = new GameObject();

            yield return null;

            publisher.Subscribe(_ => testValue += 1).AddTo(gameObject);

            yield return null;

            publisher.Invoke(Empty.Default);

            Assert.AreEqual(1, testValue);

            GameObject.Destroy(gameObject);

            yield return null;

            publisher.Invoke(Empty.Default);

            Assert.AreEqual(1, testValue);

            publisher.Dispose();
        }

        [TearDown]
        public void TearDown() {
            _disposables.Dispose();
        }

        private class TestDisposable : IDisposable {
            public bool IsDisposed { get; private set; } = false;
            public void Dispose() {
                IsDisposed = true;
            }
        }

        public void TestMethod(Empty _) { }
    }
}
