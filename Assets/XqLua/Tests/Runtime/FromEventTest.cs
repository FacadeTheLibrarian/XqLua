using NUnit.Framework;
using System;
using UnityEngine.TestTools;
using XqLua.Extension;

namespace XqLua.Test {
    public class FromEventTest {
        private event Action<int> _testEvent = delegate { };
        private event Action _testVoidEvent = delegate { };

        private event Action<int, string> _testEvent2 = delegate { };

        private event Action<int, string, float> _testEvent3 = delegate { };
        private event Action<int, string, float, bool> _testEvent4 = delegate { };


        private Disposables _disposables = default;

        [SetUp]
        public void SetUp() {
            _disposables = new Disposables();
        }

        /// <summary>
        /// FromEventでC# eventをラップして購読できるか?
        /// </summary>
        [Test]
        public void FromEventShouldWork() {
            int test = 0;

            IPublisher<int> publisher = Publisher<int>.FromEvent(
                handler => { _testEvent += handler; UnityEngine.Debug.Log("購読"); },
                handler => { _testEvent -= handler; UnityEngine.Debug.Log("購読解除"); }
            );

            IDisposableSubscription disposable1 = publisher.Subscribe(value => test = value);
            LogAssert.Expect(UnityEngine.LogType.Log, "購読");

            _testEvent.Invoke(10);
            Assert.AreEqual(10, test);

            disposable1.Dispose();
            LogAssert.Expect(UnityEngine.LogType.Log, "購読解除");

            _testEvent.Invoke(100);
            Assert.AreEqual(10, test);
        }

        /// <summary>
        /// FromEventでC# eventをラップして購読解除後に再度購読できるか?
        /// </summary>
        [Test]
        public void SubscribeAfterDisposingSubscriptionShouldWork() {
            int test = 0;

            IPublisher<int> publisher = Publisher<int>.FromEvent(
                handler => { _testEvent += handler; UnityEngine.Debug.Log("購読"); },
                handler => { _testEvent -= handler; UnityEngine.Debug.Log("購読解除"); }
            );

            IDisposableSubscription disposable1 = publisher.Subscribe(value => test = value);
            LogAssert.Expect(UnityEngine.LogType.Log, "購読");

            _testEvent.Invoke(10);
            Assert.AreEqual(10, test);

            disposable1.Dispose();
            LogAssert.Expect(UnityEngine.LogType.Log, "購読解除");

            _testEvent.Invoke(100);
            Assert.AreEqual(10, test);

            IDisposableSubscription disposable2 = publisher.Subscribe(value => test = value);
            LogAssert.Expect(UnityEngine.LogType.Log, "購読");

            _testEvent.Invoke(10);
            Assert.AreEqual(10, test);

            disposable2.Dispose();
            LogAssert.Expect(UnityEngine.LogType.Log, "購読解除");

            _testEvent.Invoke(100);
            Assert.AreEqual(10, test);
        }

        /// <summary>
        /// FromEventで2つのパラメータを持つC# eventを購読して値を受け取れるか？
        /// </summary>
        [Test]
        public void FromEventWithTwoParametersShouldWork() {
            int test = 0;
            string testString = "";

            IPublisher<int, string> publisher = Publisher<int, string>.FromEvent(
                handler => { _testEvent2 += handler; UnityEngine.Debug.Log("購読"); },
                handler => { _testEvent2 -= handler; UnityEngine.Debug.Log("購読解除"); }
            );

            IDisposableSubscription disposable1 = publisher.Subscribe((value1, value2) => {
                test = value1;
                testString = value2;
            });
            LogAssert.Expect(UnityEngine.LogType.Log, "購読");

            _testEvent2.Invoke(10, "test");
            Assert.AreEqual(10, test);
            Assert.AreEqual("test", testString);

            disposable1.Dispose();
            LogAssert.Expect(UnityEngine.LogType.Log, "購読解除");
        }

        /// <summary>
        /// FromEventで2つのパラメータを持つC# eventを購読解除したあと値は受け取れないか？
        /// </summary>
        [Test]
        public void FromEventWithTwoParametersShouldNotWorkAfterDispose() {
            int test = 0;
            string testString = "";

            IPublisher<int, string> publisher = Publisher<int, string>.FromEvent(
                handler => { _testEvent2 += handler; UnityEngine.Debug.Log("購読"); },
                handler => { _testEvent2 -= handler; UnityEngine.Debug.Log("購読解除"); }
            );

            IDisposableSubscription disposable1 = publisher.Subscribe((value1, value2) => {
                test = value1;
                testString = value2;
            });
            LogAssert.Expect(UnityEngine.LogType.Log, "購読");

            disposable1.Dispose();
            LogAssert.Expect(UnityEngine.LogType.Log, "購読解除");

            _testEvent2.Invoke(1000, "testAfterDispose");
            Assert.AreEqual(0, test);
            Assert.AreEqual("", testString);
        }

        /// <summary>
        /// FromEventで3つのパラメータを持つC# eventを購読して値を受け取れるか？
        /// </summary>
        [Test]
        public void FromEventWithThreeParametersShouldWork() {
            int test = 0;
            string testString = "";
            float testFloat = 0.0f;

            IPublisher<int, string, float> publisher = Publisher<int, string, float>.FromEvent(
                handler => { _testEvent3 += handler; UnityEngine.Debug.Log("購読"); },
                handler => { _testEvent3 -= handler; UnityEngine.Debug.Log("購読解除"); }
            );

            IDisposableSubscription disposable1 = publisher.Subscribe((value1, value2, value3) => {
                test = value1;
                testString = value2;
                testFloat = value3;
            });
            LogAssert.Expect(UnityEngine.LogType.Log, "購読");

            _testEvent3.Invoke(10, "test", 1.5f);
            Assert.AreEqual(10, test);
            Assert.AreEqual("test", testString);
            Assert.AreEqual(1.5f, testFloat);

            disposable1.Dispose();
            LogAssert.Expect(UnityEngine.LogType.Log, "購読解除");
        }

        /// <summary>
        /// FromEventで3つのパラメータを持つC# eventを購読して値を受け取れるか？
        /// </summary>
        [Test]
        public void FromEventWithThreeParametersShouldNotWorkAfterDispose() {
            int test = 0;
            string testString = "";
            float testFloat = 0.0f;

            IPublisher<int, string, float> publisher = Publisher<int, string, float>.FromEvent(
                handler => { _testEvent3 += handler; UnityEngine.Debug.Log("購読"); },
                handler => { _testEvent3 -= handler; UnityEngine.Debug.Log("購読解除"); }
            );

            IDisposableSubscription disposable1 = publisher.Subscribe((value1, value2, value3) => {
                test = value1;
                testString = value2;
                testFloat = value3;
            });
            LogAssert.Expect(UnityEngine.LogType.Log, "購読");

            disposable1.Dispose();
            LogAssert.Expect(UnityEngine.LogType.Log, "購読解除");

            _testEvent3.Invoke(100, "testAfterDispose", 2.5f);
            Assert.AreEqual(0, test);
            Assert.AreEqual("", testString);
            Assert.AreEqual(0.0f, testFloat);
        }

        /// <summary>
        /// FromEventで4つのパラメータを持つC# eventを購読して値を受け取れるか？
        /// </summary>
        [Test]
        public void FromEventWithFourParametersShouldWork() {
            int test = 0;
            string testString = "";
            float testFloat = 0.0f;
            bool testBool = false;

            IPublisher<int, string, float, bool> publisher = Publisher<int, string, float, bool>.FromEvent(
                handler => { _testEvent4 += handler; UnityEngine.Debug.Log("購読"); },
                handler => { _testEvent4 -= handler; UnityEngine.Debug.Log("購読解除"); }
            );

            IDisposableSubscription disposable1 = publisher.Subscribe((value1, value2, value3, value4) => {
                test = value1;
                testString = value2;
                testFloat = value3;
                testBool = value4;
            });
            LogAssert.Expect(UnityEngine.LogType.Log, "購読");

            _testEvent4.Invoke(10, "test", 1.5f, true);
            Assert.AreEqual(10, test);
            Assert.AreEqual("test", testString);
            Assert.AreEqual(1.5f, testFloat);
            Assert.AreEqual(true, testBool);

            disposable1.Dispose();
            LogAssert.Expect(UnityEngine.LogType.Log, "購読解除");
        }

        /// <summary>
        /// FromEventで4つのパラメータを持つC# eventを購読して値を受け取れるか？
        /// </summary>
        [Test]
        public void FromEventWithFourParametersShouldNotWorkAfterDispose() {
            int test = 0;
            string testString = "";
            float testFloat = 0.0f;
            bool testBool = false;

            IPublisher<int, string, float, bool> publisher = Publisher<int, string, float, bool>.FromEvent(
                handler => { _testEvent4 += handler; UnityEngine.Debug.Log("購読"); },
                handler => { _testEvent4 -= handler; UnityEngine.Debug.Log("購読解除"); }
            );

            IDisposableSubscription disposable1 = publisher.Subscribe((value1, value2, value3, value4) => {
                test = value1;
                testString = value2;
                testFloat = value3;
                testBool = value4;
            });
            LogAssert.Expect(UnityEngine.LogType.Log, "購読");

            disposable1.Dispose();
            LogAssert.Expect(UnityEngine.LogType.Log, "購読解除");

            _testEvent4.Invoke(100, "testAfterDispose", 2.5f, true);
            Assert.AreEqual(0, test);
            Assert.AreEqual("", testString);
            Assert.AreEqual(0.0f, testFloat);
            Assert.AreEqual(false, testBool);
        }

        // NOTE: 追加テスト thx to Claude

        /// <summary>
        /// EventPublisher<T>で複数の購読者が全員イベントを受け取れるか
        /// </summary>
        [Test]
        public void EventPublisherGenericMultipleSubscribersShouldAllReceive() {
            int receiver1 = -1;
            int receiver2 = -1;

            IPublisher<int> publisher = Publisher<int>.FromEvent(
                h => _testEvent += h,
                h => _testEvent -= h
            );
            publisher.Subscribe(v => receiver1 = v).AddTo(_disposables);
            publisher.Subscribe(v => receiver2 = v).AddTo(_disposables);

            _testEvent?.Invoke(99);

            Assert.AreEqual(99, receiver1);
            Assert.AreEqual(99, receiver2);
        }

        /// <summary>
        /// 引数なしC# eventをEventPublisher.FromEventでラップして購読できるか
        /// </summary>
        [Test]
        public void EventPublisherVoidShouldWork() {
            bool isInvoked = false;

            IPublisher<Empty> publisher = Publisher<Empty>.FromEvent(
                h => _testVoidEvent += h,
                h => _testVoidEvent -= h
            );
            publisher.Subscribe(_ => isInvoked = true).AddTo(_disposables);

            _testVoidEvent.Invoke();

            Assert.IsTrue(isInvoked);
        }

        /// <summary>
        /// EventPublisher(Void)の購読解除後にeventを発火しても呼ばれないか
        /// </summary>
        [Test]
        public void EventPublisherVoidUnsubscriptionShouldWork() {
            bool isInvoked = false;

            IPublisher<Empty> publisher = Publisher<Empty>.FromEvent(
                h => _testVoidEvent += h,
                h => _testVoidEvent -= h
            );
            publisher.Subscribe(_ => isInvoked = true).Dispose();

            _testVoidEvent.Invoke();

            Assert.IsFalse(isInvoked);
        }

        /// <summary>
        /// EventPublisherにOperatorをチェーンして正しく動作するか
        /// </summary>
        [Test]
        public void EventPublisherWithOperatorShouldWork() {
            int receiver = -1;
            bool condition = false;

            IPublisher<int> publisher = Publisher<int>.FromEvent(
                h => _testEvent += h,
                h => _testEvent -= h
            );
            publisher
                .OnCondition(() => condition)
                .Subscribe(v => receiver = v).AddTo(_disposables);

            _testEvent.Invoke(42);
            Assert.AreEqual(-1, receiver);

            condition = true;
            _testEvent.Invoke(42);
            Assert.AreEqual(42, receiver);
        }
        // NOTE: 追加テスト 終

        [TearDown]
        public void TearDown() {
            _disposables.Dispose();
        }
    }
}
