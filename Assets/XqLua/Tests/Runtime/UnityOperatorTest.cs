using NUnit.Framework;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine.TestTools;
using XqLua.Extension;
using XqLua.Unity;

namespace XqLua.Test {
    public class UnityOperatorTest {
        private Disposables _disposables = default;

        [SetUp]
        public void SetUp() {
            _disposables = new Disposables();
        }

        /// <summary>
        /// Delayが正しく動作するか？
        /// </summary>
        [UnityTest]
        public IEnumerator DelayShouldDelay() {
            int test = 0;
            Stopwatch stopWatch = new Stopwatch();
            Publisher<int> publisher = new Publisher<int>().AddTo(_disposables);
            publisher
                .WithDelay(1.0f)
                .Subscribe((value) => {
                    stopWatch.Stop();
                    test = value;
                }).AddTo(_disposables);

            stopWatch.Start();
            publisher.Invoke(10);
            Assert.AreEqual(0, test);

            while (stopWatch.IsRunning) {
                yield return null;
            }
            Assert.AreEqual(10, test);
            Assert.That(Math.Abs(1.0 - stopWatch.Elapsed.TotalSeconds) < 0.125);
        }

        /// <summary>
        /// Delayがキャンセルされた場合、再度Subscribeしたときに正しく動作するか？
        /// </summary>
        [Test]
        public void CancellingDelayNeverInvokesThenReSubscribable() {
            int test = 0;
            Publisher<int> publisher = new Publisher<int>().AddTo(_disposables);
            IDisposableSubscription subscription = publisher
                .WithDelay(5.0f)
                .Subscribe((value) => {
                    test = value;
                });

            publisher.Invoke(10);
            subscription.Dispose();

            Assert.AreEqual(0, test);

            publisher.Subscribe((value) => test = value).AddTo(_disposables);
            publisher.Invoke(10);

            Assert.AreEqual(10, test);
        }

        /// <summary>
        /// DebugLogが正しく動作するか？
        /// </summary>
        [Test]
        public void DebugLogShouldWork() {
            Publisher<int> publisher = new Publisher<int>().AddTo(_disposables);
            publisher.WithMessage("DebugTest").Subscribe((value) => { }).AddTo(_disposables);
            publisher.WithMessage("Debug {test}").WithDebugLog().Subscribe((value) => { }).AddTo(_disposables);

            publisher.Invoke(100);

            LogAssert.Expect(UnityEngine.LogType.Log, "DebugTest");
            LogAssert.Expect(UnityEngine.LogType.Log, "Debug 100");
            LogAssert.Expect(UnityEngine.LogType.Log, "そのままの値: 100, ToString: 100");

            publisher
                .WithMessage("Debug {test}")
                .WithDebugLog()
                .ConvertTo<int, string>((value) => { return value.ToString("F10"); })
                .WithMessage("Debug {test}")
                .WithDebugLog()
                .Subscribe(value => { })
                .AddTo(_disposables);

            publisher.Invoke(100);
            LogAssert.Expect(UnityEngine.LogType.Log, "Debug 100");
            LogAssert.Expect(UnityEngine.LogType.Log, "そのままの値: 100, ToString: 100");
            LogAssert.Expect(UnityEngine.LogType.Log, "Debug 100.0000000000");
            LogAssert.Expect(UnityEngine.LogType.Log, "そのままの値: 100.0000000000, ToString: 100.0000000000");
        }

        /// <summary>
        /// WithIntervalが正しく動作するか？
        /// </summary>
        [Test]
        public void WithSystemIntervalDoBlocks() {
            int test = 0;
            Publisher<int> publisher = new Publisher<int>().AddTo(_disposables);
            publisher
                .WithInterval(1.0f)
                .Subscribe((value) => test = value).AddTo(_disposables);
            for (int i = 0; i < 10; i++) {
                publisher.Invoke(i);
            }
            Assert.AreEqual(0, test);
        }

        /// <summary>
        /// WithIntervalが正しく動作するか？（SystemTime版）
        /// </summary>
        [UnityTest]
        public IEnumerator WithSystemIntervalShouldWork() {
            int test = 0;
            Publisher<int> publisher = new Publisher<int>().AddTo(_disposables);
            publisher
                .WithInterval(1.0f)
                .Subscribe((value) => test = value).AddTo(_disposables);

            publisher.Invoke(10);
            Assert.AreEqual(10, test);

            publisher.Invoke(20);
            Assert.AreEqual(10, test);

            Stopwatch stopWatch = new Stopwatch();
            Publisher<int> timerPublisher = new Publisher<int>().AddTo(_disposables);
            timerPublisher
                .WithDelay(1.0f)
                .Subscribe((value) => {
                    stopWatch.Stop();
                    test = value;
                }).AddTo(_disposables);

            stopWatch.Start();
            timerPublisher.Invoke(10);

            while (stopWatch.IsRunning) {
                yield return null;
            }

            publisher.Invoke(30);
            Assert.AreEqual(30, test);
        }

        /// <summary>
        /// WithIntervalが正しく動作するか？（UnityTime版）
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator WithUnityIntervalShouldWork() {
            int test = 0;
            Publisher<int> publisher = new Publisher<int>().AddTo(_disposables);
            publisher
                .WithInterval(1.0f, TimeMode.e_timeMode.unityTime)
                .Subscribe((value) => test = value).AddTo(_disposables);

            publisher.Invoke(10);
            Assert.AreEqual(10, test);

            publisher.Invoke(20);
            Assert.AreEqual(10, test);

            Stopwatch stopWatch = new Stopwatch();
            Publisher<int> timerPublisher = new Publisher<int>().AddTo(_disposables);
            timerPublisher
                .WithDelay(1.0f)
                .Subscribe((value) => {
                    stopWatch.Stop();
                    test = value;
                }).AddTo(_disposables);

            stopWatch.Start();
            timerPublisher.Invoke(10);

            UnityEngine.Time.timeScale = 0.75f;

            while (stopWatch.IsRunning) {
                yield return null;
            }

            publisher.Invoke(30);
            Assert.AreEqual(30, test);

            Assert.That(Math.Abs(1.0 - stopWatch.Elapsed.TotalSeconds * 0.75f) < 0.125);
        }

        // NOTE: 追加テスト thx to Claude
        /// <summary>
        /// WithMessageのLogType.WarningでUnityEngine.Debug.LogWarningが呼ばれるか
        /// </summary>
        [Test]
        public void WithMessageWarningLogTypeShouldWork() {
            Publisher<int> publisher = new Publisher<int>().AddTo(_disposables);
            publisher
                .WithMessage("警告: {test}", UnityEngine.LogType.Warning)
                .Subscribe(_ => { }).AddTo(_disposables);

            LogAssert.Expect(UnityEngine.LogType.Warning, "警告: 5");
            publisher.Invoke(5);
        }

        /// <summary>
        /// WithMessageのLogType.ErrorでUnityEngine.Debug.LogErrorが呼ばれるか
        /// </summary>
        [Test]
        public void WithMessageErrorLogTypeShouldWork() {
            Publisher<int> publisher = new Publisher<int>().AddTo(_disposables);
            publisher
                .WithMessage("エラー: {test}", UnityEngine.LogType.Error)
                .Subscribe(_ => { }).AddTo(_disposables);

            LogAssert.Expect(UnityEngine.LogType.Error, "エラー: 5");
            publisher.Invoke(5);
        }
        // NOTE: 追加テスト 終

        [TearDown]
        public void TearDown() {
            _disposables.Dispose();
        }
    }
}
