using NUnit.Framework;
using XqLua.Extension;

namespace XqLua.Test {
    public class PublisherTest {

        private Disposables _disposables = default;

        [SetUp]
        public void SetUp() {
            _disposables = new Disposables();
        }

        /// <summary>
        /// Publisherが値を持つイベントを正しく発火できるか？
        /// </summary>
        [Test]
        public void ValuedEventShouldWork() {
            Publisher<int> actionEvent = new Publisher<int>();
            int receiver = -1;
            actionEvent.Subscribe(value => receiver = value).AddTo(_disposables);

            actionEvent.Invoke(1);
            Assert.AreEqual(receiver, 1);
        }

        /// <summary>
        /// 空のPublisherを発火させようとしても例外が発生しないか？
        /// </summary>
        [Test]
        public void InvokeWithoutSubscriptionNeverThrows() {
            Publisher<Empty> actionEvent = new Publisher<Empty>();

            Assert.DoesNotThrow(() => actionEvent.Invoke(Empty.Default));
        }

        /// <summary>
        /// 購読解除後にPublisherを発火させようとしても例外が発生しないか？
        /// </summary>
        [Test]
        public void InvokeAfterUnsubscriptionNeverThrows() {
            Publisher<Empty> actionEvent = new Publisher<Empty>();
            int receiver = -1;
            actionEvent.Subscribe(_ => receiver = 0).Dispose();

            Assert.AreEqual(receiver, -1);
            Assert.DoesNotThrow(() => actionEvent.Invoke(Empty.Default));
        }

        //NOTE: 追加テスト thx to Claude

        /// <summary>
        /// 複数の購読者が全員イベントを受け取れるか（マルチキャスト）
        /// </summary>
        [Test]
        public void MultipleSubscribersShouldAllReceive() {
            Publisher<int> publisher = new Publisher<int>();
            int receiver1 = -1;
            int receiver2 = -1;
            int receiver3 = -1;

            publisher.Subscribe(v => receiver1 = v).AddTo(_disposables);
            publisher.Subscribe(v => receiver2 = v).AddTo(_disposables);
            publisher.Subscribe(v => receiver3 = v).AddTo(_disposables);

            publisher.Invoke(42);

            Assert.AreEqual(42, receiver1);
            Assert.AreEqual(42, receiver2);
            Assert.AreEqual(42, receiver3);
        }

        /// <summary>
        /// 一部の購読者だけDisposeしたとき、他の購読者は引き続きイベントを受け取れるか
        /// </summary>
        [Test]
        public void PartialUnsubscriptionShouldKeepOtherSubscribersActive() {
            Publisher<int> publisher = new Publisher<int>();
            int receiver1 = -1;
            int receiver2 = -1;

            publisher.Subscribe(v => receiver1 = v).AddTo(_disposables);
            IDisposableSubscription sub2 = publisher.Subscribe(v => receiver2 = v);

            publisher.Invoke(10);
            Assert.AreEqual(10, receiver1);
            Assert.AreEqual(10, receiver2);

            sub2.Dispose();
            publisher.Invoke(20);

            Assert.AreEqual(20, receiver1);
            Assert.AreEqual(10, receiver2); // sub2はDispose済みなので更新されない
        }

        //NOTE: 追加テスト 終

        [TearDown]
        public void TearDown() {
            _disposables.Dispose();
        }
    }
}