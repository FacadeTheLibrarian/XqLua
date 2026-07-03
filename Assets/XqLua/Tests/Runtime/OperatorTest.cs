using NUnit.Framework;
using XqLua.Extension;

namespace XqLua.Test {
    public class OperatorTest {

        private Disposables _disposables = default;

        [SetUp]
        public void SetUp() {
            _disposables = new Disposables();
        }

        /// <summary>
        /// OnConditionが正しく動作するか？
        /// </summary>
        [Test]
        public void ConditionalOperatorShouldWork() {
            int test = 0;
            bool condition = false;
            Publisher<Empty> publisher = new Publisher<Empty>().AddTo(_disposables);

            publisher
                .OnCondition(() => { return condition; })
                .Subscribe(_ => test = 100).AddTo(_disposables);
            publisher.Invoke(Empty.Default);

            Assert.AreEqual(0, test);

            condition = true;
            publisher.Invoke(Empty.Default);

            Assert.AreEqual(100, test);
        }

        /// <summary>
        /// OnValueIsが正しく動作するか？
        /// </summary>
        [Test]
        public void OnValueIsOperatorShouldWork() {
            int test = 0;
            Publisher<int> publisher = new Publisher<int>().AddTo(_disposables);
            publisher
                .OnValueIs((value) => { return value > 10; })
                .Subscribe((value) => test = value).AddTo(_disposables);
            publisher.Invoke(5);
            Assert.AreEqual(0, test);
            publisher.Invoke(15);
            Assert.AreEqual(15, test);
        }

        /// <summary>
        /// Skipが正しく動作するか？
        /// </summary>
        [Test]
        public void SkipShouldSkip() {
            string test = "スキップ中";
            Publisher<string> publisher = new Publisher<string>().AddTo(_disposables);
            publisher
                .Skip(3)
                .Subscribe((value) => test = value).AddTo(_disposables);

            publisher.Invoke("1回目");
            Assert.AreEqual("スキップ中", test);

            publisher.Invoke("2回目");
            Assert.AreEqual("スキップ中", test);

            publisher.Invoke("3回目");
            Assert.AreEqual("スキップ中", test);

            publisher.Invoke("スキップ終了");
            Assert.AreEqual("スキップ終了", test);
        }

        /// <summary>
        /// Takeが正しく動作するか？
        /// </summary>
        [Test]
        public void TakeShouldTake() {
            string test = "";
            Publisher<string> publisher = new Publisher<string>().AddTo(_disposables);
            publisher
                .Take(3)
                .Subscribe((value) => test = value).AddTo(_disposables);
            publisher.Invoke("1回目");
            Assert.AreEqual("1回目", test);

            publisher.Invoke("2回目");
            Assert.AreEqual("2回目", test);

            publisher.Invoke("取得終了");
            Assert.AreEqual("取得終了", test);

            publisher.Invoke("取得後");
            Assert.AreEqual("取得終了", test);
        }

        /// <summary>
        /// ConvertToが正しく動作するか？
        /// </summary>
        [Test]
        public void ConvertToShouldConvert() {
            string test = "";
            Publisher<int> publisher = new Publisher<int>().AddTo(_disposables);
            publisher
                .ConvertTo<int, string>((value) => { return value.ToString(); })
                .Subscribe((value) => test = value).AddTo(_disposables);
            publisher.Invoke(100);
            Assert.AreEqual("100", test);
        }

        /// <summary>
        /// OnValueChangedが正しく動作するか？
        /// </summary>
        [Test]
        public void OnValueChangedOperatorShouldWork() {
            int test = 0;
            int sub = 0;
            Publisher<int> publisher = new Publisher<int>().AddTo(_disposables);
            publisher
                .OnValueChanged()
                .Subscribe((value) => {
                    test = value;
                    sub = value;
                }).AddTo(_disposables);
            publisher.Invoke(5);
            Assert.AreEqual(5, test);

            sub = 0;
            publisher.Invoke(5);
            Assert.AreEqual(0, sub);

            publisher.Invoke(15);
            Assert.AreEqual(15, test);
            Assert.AreEqual(15, sub);
        }

        /// <summary>
        /// OnConditionとOnValueIsを組み合わせた場合に正しく動作するか？
        /// </summary>
        [Test]
        public void OnConditionAndOnValueIsOperatorShouldWork() {
            int test = 0;
            bool condition = false;
            Publisher<int> publisher = new Publisher<int>().AddTo(_disposables);
            publisher
                .OnCondition(() => condition)
                .OnValueIs((value) => { return value > 10; })
                .Subscribe((value) => test = value).AddTo(_disposables);

            publisher.Invoke(100);
            Assert.AreEqual(0, test);

            condition = true;

            publisher.Invoke(5);
            Assert.AreEqual(0, test);
            publisher.Invoke(15);
            Assert.AreEqual(15, test);
        }

        /// <summary>
        /// OnCondition, OnValueIs, Skip, Take, ConvertToを組み合わせた場合に正しく動作するか？
        /// </summary>
        [Test]
        public void AllOperatorsChainShouldWork() {
            int test = 0;
            bool condition = false;
            Publisher<int> publisher = new Publisher<int>().AddTo(_disposables);
            publisher
                .OnCondition(() => condition)
                .OnValueIs((value) => { return value > 10; })
                .Skip(2)
                .Take(2)
                .ConvertTo<int, string>((value) => { return value.ToString(); })
                .Subscribe((value) => test = int.Parse(value)).AddTo(_disposables);
            //NOTE: OnConditionがブロック
            publisher.Invoke(100);
            Assert.AreEqual(0, test);

            condition = true;
            //NOTE: OnValueIsがブロック
            publisher.Invoke(5);
            Assert.AreEqual(0, test);

            //NOTE: Skipがブロック
            publisher.Invoke(15);
            Assert.AreEqual(0, test);
            publisher.Invoke(20);
            Assert.AreEqual(0, test);

            //NOTE: Takeが通過 & 型変換 -> パース
            publisher.Invoke(25);
            Assert.AreEqual(25, test);
            publisher.Invoke(30);
            Assert.AreEqual(30, test);

            //NOTE: Takeが終了し、以降はブロック
            publisher.Invoke(35);
            Assert.AreEqual(30, test);
        }

        // NOTE: 追加テスト thx to Claude

        /// <summary>
        /// Skip(0)は何もスキップせず全ての値を通過させるか
        /// </summary>
        [Test]
        public void SkipZeroShouldSkipNothing() {
            int test = -1;
            Publisher<int> publisher = new Publisher<int>().AddTo(_disposables);
            publisher
                .Skip(0)
                .Subscribe(v => test = v).AddTo(_disposables);

            publisher.Invoke(1);
            Assert.AreEqual(1, test);
        }

        /// <summary>
        /// Take(0)は何も通過させないか
        /// </summary>
        [Test]
        public void TakeZeroShouldTakeNothing() {
            int test = -1;
            Publisher<int> publisher = new Publisher<int>().AddTo(_disposables);
            publisher
                .Take(0)
                .Subscribe(v => test = v).AddTo(_disposables);

            publisher.Invoke(1);
            Assert.AreEqual(-1, test);
        }

        /// <summary>
        /// Skip → Take のチェーンが正しく動作するか
        /// Skip(2).Take(2) なら 3・4回目だけ通過し、5回目以降は来ないか
        /// Update: このテストで気付きを得た ありがとうClaude
        /// </summary>
        [Test]
        public void SkipThenTakeChainShouldWork() {
            string test = "";
            Publisher<string> publisher = new Publisher<string>().AddTo(_disposables);
            publisher
                .Skip(2)
                .Take(2)
                .Subscribe(v => test = v).AddTo(_disposables);

            publisher.Invoke("1回目"); // Skip
            Assert.AreEqual("", test);
            publisher.Invoke("2回目"); // Skip
            Assert.AreEqual("", test);
            publisher.Invoke("3回目"); // Take 1回目
            Assert.AreEqual("3回目", test);
            publisher.Invoke("4回目"); // Take 2回目
            Assert.AreEqual("4回目", test);
            publisher.Invoke("5回目"); // Take終了後
            Assert.AreEqual("4回目", test);
        }

        /// <summary>
        /// OnValueChangedが参照型でも正しく動作するか
        /// 同じ参照は通過しないこと、異なる参照は通過することを検証
        /// </summary>
        [Test]
        public void OnValueChangedWithClassShouldWork() {
            object instanceA = new object();
            object instanceB = new object();
            object received = null;
            int callCount = 0;

            Publisher<object> publisher = new Publisher<object>().AddTo(_disposables);
            publisher
                .OnValueChanged()
                .Subscribe(v => { received = v; callCount++; }).AddTo(_disposables);

            publisher.Invoke(instanceA);
            Assert.AreEqual(instanceA, received);
            Assert.AreEqual(1, callCount);

            publisher.Invoke(instanceA); // 同じ参照なので通過しない
            Assert.AreEqual(1, callCount);

            publisher.Invoke(instanceB); // 異なる参照なので通過する
            Assert.AreEqual(instanceB, received);
            Assert.AreEqual(2, callCount);
        }

        /// <summary>
        /// ConvertToで型変換した後にOnConditionをチェーンできるか
        /// </summary>
        [Test]
        public void ConvertToThenOnConditionShouldWork() {
            string test = "";
            bool condition = false;
            Publisher<int> publisher = new Publisher<int>().AddTo(_disposables);
            publisher
                .ConvertTo<int, string>(v => v.ToString())
                .OnCondition(() => condition)
                .Subscribe(v => test = v).AddTo(_disposables);

            publisher.Invoke(42);
            Assert.AreEqual("", test);

            condition = true;
            publisher.Invoke(42);
            Assert.AreEqual("42", test);
        }

        // NOTE: 追加テスト 終

        [TearDown]
        public void TearDown() {
            _disposables.Dispose();
        }
    }
}