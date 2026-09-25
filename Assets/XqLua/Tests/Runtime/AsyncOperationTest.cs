using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using XqLua.Async;
using XqLua.Extension;

namespace XqLua.Test {
    public class AsyncOperationTest {

        private Disposables _disposables = default;

        [SetUp]
        public void Setup() {
            _disposables = new Disposables();
        }

        /// <summary>
        /// AwaitableSubscribeが正しく値を受け取れるか？
        /// </summary>
        [Test]
        public async Task AwaitableSubscriptionCanAwaitAndSetValue() {
            using CancellationTokenSource source = new CancellationTokenSource();
            Publisher<int> testPublisher = new Publisher<int>().AddTo(_disposables);

            _ = AutoInvocation(testPublisher, 100, 50, source.Token);

            int receiver = await testPublisher.AwaitableSubscribe(source.Token);

            Assert.AreEqual(100, receiver);
        }

        /// <summary>
        /// AwaitableSubscribeがキャンセルできるか？
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task AwaitableSubscriptionCanCancel() {
            using CancellationTokenSource source = new CancellationTokenSource();
            Publisher<int> testPublisher = new Publisher<int>().AddTo(_disposables);

            Awaitable<int> awaitable = testPublisher.AwaitableSubscribe(source.Token);
            source.Cancel();

            bool wasCancelled = false;
            try {
                await awaitable;
            }
            catch (OperationCanceledException) {
                wasCancelled = true;
            }

            Assert.IsTrue(wasCancelled);
        }

        /// <summary>
        /// ReactivePropertyに対するAwaitableSubscribeが正しく値を受け取れるか？
        /// </summary>
        [Test]
        public async Task AwaitableSubscriptionToReactivePropertyCanAwaitAndSetValue() {
            using CancellationTokenSource source = new CancellationTokenSource();
            ReactiveProperty<int> testRP = new ReactiveProperty<int>(100).AddTo(_disposables);

            _ = AutoInvocation(testRP, 200, 100, source.Token);

            int receiver = await testRP.AwaitableSubscribe(source.Token);

            Assert.AreEqual(200, receiver);
        }

        /// <summary>
        /// AwaitableSubscribeがキャンセルできるか？
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task AwaitableSubscriptionToReactivePropertyCanCancel() {
            using CancellationTokenSource source = new CancellationTokenSource();
            ReactiveProperty<int> testRP = new ReactiveProperty<int>(100).AddTo(_disposables);

            Awaitable<int> awaitable = testRP.AwaitableSubscribe(source.Token);
            source.Cancel();

            bool wasCancelled = false;
            try {
                await awaitable;
            }
            catch (OperationCanceledException) {
                wasCancelled = true;
            }

            Assert.IsTrue(wasCancelled);
        }

        /// <summary>
        /// 第2引数に対応したAwaitableSubscribeが正しく値を受け取れるか？
        /// </summary>
        [Test]
        public async Task AwaitableSubscriptionCanAwaitAndSetTwoValues() {
            using CancellationTokenSource source = new CancellationTokenSource();
            Publisher<int, string> testPublisher = new Publisher<int, string>().AddTo(_disposables);

            _ = AutoInvocation(testPublisher, 100, "hello", 50, source.Token);

            (int, string) receiver = await testPublisher.AwaitableSubscribe(source.Token);

            Assert.AreEqual(100, receiver.Item1);
            Assert.AreEqual("hello", receiver.Item2);
        }

        /// <summary>
        /// 第3引数に対応したAwaitableSubscribeが正しく値を受け取れるか？
        /// </summary>
        [Test]
        public async Task AwaitableSubscriptionCanAwaitAndSetThreeValues() {
            using CancellationTokenSource source = new CancellationTokenSource();
            Publisher<int, string, float> testPublisher = new Publisher<int, string, float>().AddTo(_disposables);

            _ = AutoInvocation(testPublisher, 100, "hello", 50.0f, 50, source.Token);

            (int, string, float) receiver = await testPublisher.AwaitableSubscribe(source.Token);

            Assert.AreEqual(100, receiver.Item1);
            Assert.AreEqual("hello", receiver.Item2);
            Assert.AreEqual(50.0f, receiver.Item3);
        }

        /// <summary>
        /// 第4引数に対応したAwaitableSubscribeが正しく値を受け取れるか？
        /// </summary>
        [Test]
        public async Task AwaitableSubscriptionCanAwaitAndSetFourValues() {
            using CancellationTokenSource source = new CancellationTokenSource();
            Publisher<int, string, float, bool> testPublisher = new Publisher<int, string, float, bool>().AddTo(_disposables);

            _ = AutoInvocation(testPublisher, 100, "hello", 50.0f, true, 50, source.Token);

            (int, string, float, bool) receiver = await testPublisher.AwaitableSubscribe(source.Token);

            Assert.AreEqual(100, receiver.Item1);
            Assert.AreEqual("hello", receiver.Item2);
            Assert.AreEqual(50.0f, receiver.Item3);
            Assert.AreEqual(true, receiver.Item4);
        }

        /// <summary>
        /// テスト用の関数
        /// </summary>
        private async Task AutoInvocation(Publisher<int> test, int value, int millisecondsdelay, CancellationToken token) {
            try {
                await Task.Delay(millisecondsdelay, token);
            }
            catch {
                throw;
            }
            test.Invoke(value);
        }

        /// <summary>
        /// テスト用の関数
        /// </summary>
        private async Task AutoInvocation(ReactiveProperty<int> test, int value, int millisecondsdelay, CancellationToken token) {
            try {
                await Task.Delay(millisecondsdelay, token);
            }
            catch {
                throw;
            }
            test.Value = value;
        }

        /// <summary>
        /// テスト用の関数
        /// </summary>
        private async Task AutoInvocation(Publisher<int, string> test, int value1, string value2,int millisecondsdelay, CancellationToken token) {
            try {
                await Task.Delay(millisecondsdelay, token);
            }
            catch {
                throw;
            }
            test.Invoke(value1, value2);
        }

        /// <summary>
        /// テスト用の関数
        /// </summary>
        private async Task AutoInvocation(Publisher<int, string, float> test, int value1, string value2, float value3, int millisecondsdelay, CancellationToken token) {
            try {
                await Task.Delay(millisecondsdelay, token);
            }
            catch {
                throw;
            }
            test.Invoke(value1, value2, value3);
        }

        /// <summary>
        /// テスト用の関数
        /// </summary>
        private async Task AutoInvocation(Publisher<int, string, float, bool> test, int value1, string value2, float value3, bool value4, int millisecondsdelay, CancellationToken token) {
            try {
                await Task.Delay(millisecondsdelay, token);
            }
            catch {
                throw;
            }
            test.Invoke(value1, value2, value3, value4);
        }

        [TearDown]
        public void TearDown() {
            _disposables.Dispose();
        }
    }
}
