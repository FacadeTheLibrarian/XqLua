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
        /// AwaitableSubscriptionが正しく値を受け取れるか？
        /// </summary>
        [Test]
        public async Task AwaitableSubscriptionCanAwaitAndSetValue() {
            using CancellationTokenSource source = new CancellationTokenSource();
            Publisher<int> testPublisher = new Publisher<int>().AddTo(_disposables);

            _ = AutoInvocation(testPublisher, 100, 50, source.Token);

            int receiver = await testPublisher.AwaitableSubscription(source.Token);

            Assert.AreEqual(100, receiver);
        }

        /// <summary>
        /// AwaitableSubscriptionがキャンセルできるか？
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task AwaitableSubscriptionCanCancel() {
            using CancellationTokenSource source = new CancellationTokenSource();
            Publisher<int> testPublisher = new Publisher<int>().AddTo(_disposables);

            Awaitable<int> awaitable = testPublisher.AwaitableSubscription(source.Token);
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
        /// テスト用のクラス
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

        [TearDown]
        public void TearDown() {
            _disposables.Dispose();
        }
    }
}
