using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using XqLua.Async;
using XqLua.Message;

namespace XqLua.Test {
    public class MessageTest {

        [SetUp]
        public void SetUp() {

        }

        /// <summary>
        /// MessageTrayからメッセージを取得できるか？
        /// </summary>
        [Test]
        public void TakeMessageFromTray() {
            using MessageTray<TestMessage> tray = new MessageTray<TestMessage>();

            TestMessage message = new TestMessage() {
                value1 = 42,
                value2 = 3.14f
            };

            // メッセージはまだトレイに入っていないので、TryGetMessageはfalseを返すはず
            Assert.IsFalse(tray.TryReadMessage(out TestMessage _));

            tray.PutMessage(message);

            if(tray.TryReadMessage(out TestMessage receivedMessage)) {
                Assert.AreEqual(message.value1, receivedMessage.value1);
                Assert.AreEqual(message.value2, receivedMessage.value2);
            } else {
                Assert.Fail("Failed to get message from tray");
            }
        }

        /// <summary>
        /// MessageTrayからAwaitableSubscribeで正しく値を受け取れるか？
        /// </summary>
        [Test]
        public async Task AwaitableSubscriptionCanAwaitAndSetValue() {
            using CancellationTokenSource source = new CancellationTokenSource();
            using MessageTray<TestMessage> testTray = new MessageTray<TestMessage>();

            TestMessage message = new TestMessage() {
                value1 = 100,
                value2 = 200.5f
            };

            _ = AutoInvocation(testTray, message, 50, source.Token);

            TestMessage receiver = await testTray.AwaitableSubscribe(source.Token);

            Assert.AreEqual(100, receiver.value1);
            Assert.AreEqual(200.5f, receiver.value2);
        }

        /// <summary>
        /// MessageTrayからAwaitableSubscribeですでに値が入っている場合、正しく値を受け取れるか？
        /// </summary>
        [Test]
        public async Task AwaitableSubscriptionCanGetValueImmediately() {
            using CancellationTokenSource source = new CancellationTokenSource();
            using MessageTray<TestMessage> testTray = new MessageTray<TestMessage>();

            TestMessage message = new TestMessage() {
                value1 = 100,
                value2 = 200.5f
            };

            testTray.PutMessage(message);

            TestMessage receiver = await testTray.AwaitableSubscribe(source.Token);

            Assert.AreEqual(100, receiver.value1);
            Assert.AreEqual(200.5f, receiver.value2);
        }

        [TearDown]
        public void TearDown() {

        }

        /// <summary>
        /// テスト用のメッセージクラス
        /// </summary>
        private class TestMessage {
            public int value1;
            public float value2;
        }

        /// <summary>
        /// テスト用の関数
        /// </summary>
        private async Task AutoInvocation(MessageTray<TestMessage> testTray, TestMessage message, int millisecondsdelay, CancellationToken token) {
            try {
                await Task.Delay(millisecondsdelay, token);
            }
            catch {
                throw;
            }
            testTray.PutMessage(message);
        }
    }
}
