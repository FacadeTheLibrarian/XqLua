using NUnit.Framework;
using System;
using XqLua.Extension;

namespace XqLua.Test {
    public class ReactivePropertyTest {

        private Disposables _disposables = default;

        [SetUp]
        public void SetUp() {
            _disposables = new Disposables();
        }

        /// <summary>
        /// 値型のReactivePropertyが正しく動作するか？
        /// </summary>
        [Test]
        public void PrimitiveShouldWork() {
            int receiver = -1;

            ReactiveProperty<int> reactiveProperty = new ReactiveProperty<int>(0);

            reactiveProperty.Subscribe(value => receiver = value).AddTo(_disposables);

            reactiveProperty.Value = 1;

            Assert.AreEqual(receiver, 1);
        }

        /// <summary>
        /// 同じ値型の値をセットした場合、OnValueChangedは呼ばれないか？
        /// </summary>
        [Test]
        public void SamePrimitiveAssignmentDoNotInvoke() {
            int receiver = -1;

            ReactiveProperty<int> reactiveProperty = new ReactiveProperty<int>(10);

            reactiveProperty.Subscribe(value => receiver = value).AddTo(_disposables);

            //NOTE: 本家同様購読時に一回発火する
            Assert.AreEqual(10, receiver);
            receiver = -1;

            reactiveProperty.Value = 10;
            Assert.AreEqual(-1, receiver);
        }

        /// <summary>
        /// 同じ値型の値をセットした場合でも、SetForceNotifyならOnValueChangedが呼ばるか？
        /// </summary>
        [Test]
        public void SamePrimitiveAssignmentWithForceNotifyShouldInvoke() {
            int receiver = 0;
            bool isInvoked = false;

            ReactiveProperty<int> reactiveProperty = new ReactiveProperty<int>(0);

            reactiveProperty.Subscribe(value => {
                receiver = value;
                isInvoked = true;
            }).AddTo(_disposables);

            reactiveProperty.SetForceNotify(0);

            Assert.IsTrue(isInvoked);
            Assert.AreEqual(receiver, 0);
        }

        /// <summary>
        /// 参照型のReactivePropertyが正しく動作するか？
        /// </summary>
        [Test]
        public void ClassShouldWork() {
            TestClass instance = new TestClass();
            ReactiveProperty<TestClass> reactiveProperty = new ReactiveProperty<TestClass>(instance);
            TestClass receiver = new TestClass();

            reactiveProperty.Subscribe(value => receiver = value).AddTo(_disposables);
            TestClass comparer = new TestClass();

            reactiveProperty.Value = comparer;

            Assert.AreEqual(receiver, comparer);
            comparer.value = 10;
            Assert.AreEqual(receiver, comparer);
            Assert.AreNotEqual(instance, comparer);
        }

        /// <summary>
        /// 同じ参照型の値をセットした場合、OnValueChangedは呼ばれないか？
        /// </summary>
        [Test]
        public void SameClassAssignmentDoNotInvoke() {
            TestClass instance = new TestClass();
            ReactiveProperty<TestClass> reactiveProperty = new ReactiveProperty<TestClass>(instance);
            TestClass comparer = new TestClass();
            TestClass receiver = default;

            reactiveProperty.Subscribe(value => receiver = value).AddTo(_disposables);

            Assert.AreEqual(instance, receiver);

            receiver = comparer;
            reactiveProperty.Value = instance;

            Assert.AreEqual(receiver, comparer);
            Assert.AreNotEqual(instance, receiver);
        }

        /// <summary>
        /// 同じ参照型の値をセットした場合でも、SetForceNotifyならOnValueChangedが呼ばるか？
        /// </summary>
        [Test]
        public void SameClassAssignmentWithForceNotifyShouldInvoke() {
            TestClass instance = new TestClass();
            TestClass receiver = new TestClass();
            bool isInvoked = false;

            ReactiveProperty<TestClass> reactiveProperty = new ReactiveProperty<TestClass>(instance);
            reactiveProperty.Subscribe(value => {
                receiver = value;
                isInvoked = true;
            }).AddTo(_disposables);

            reactiveProperty.SetForceNotify(instance);

            Assert.IsTrue(isInvoked);
            Assert.AreEqual(receiver, instance);
        }

        /// <summary>
        /// 強制通知が同値でもイベントを発火するか？
        /// </summary>
        [Test]
        public void SetForceNotifyDoInvoke() {
            int test = 0;

            ReactiveProperty<int> reactiveProperty = new ReactiveProperty<int>(100);

            reactiveProperty.Subscribe(value => test = value).AddTo(_disposables);

            reactiveProperty.SetForceNotify(100);
            Assert.AreEqual(100, test);
            test = 0;

            reactiveProperty.SetForceNotify(100);
            Assert.AreEqual(100, test);
        }

        /// <summary>
        /// イベントを発火させずに値を変更できるか？
        /// </summary>
        [Test]
        public void SetWithoutNotifyDoesNotInvoke() {
            int test = 0;

            ReactiveProperty<int> reactiveProperty = new ReactiveProperty<int>(0);

            reactiveProperty.Subscribe(value => test = value).AddTo(_disposables);

            reactiveProperty.SetWithoutNotify(42);
            Assert.AreEqual(0, test);
        }

        /// <summary>
        /// 再帰的に値を変更するRPをSubscribeしたとき、InvalidOperationExceptionが発生するか？
        /// </summary>
        [Test]
        public void InvaliedRecursiveCallShouldThrow() {
            ReactiveProperty<int> reactiveProperty = new ReactiveProperty<int>(0);
            Assert.Throws<InvalidOperationException>(() => {
                reactiveProperty.Subscribe(value => {
                    // 再帰的に値を変更する
                    reactiveProperty.Value = value + 1;
                }).AddTo(_disposables);
            });
        }

        /// <summary>
        /// リアクティブプロパティにもオペレータを追加できるか
        /// </summary>
        [Test]
        public void CanApplyOperatorsToReactiveProperty() {
            int receiver = 0;
            bool condition = false;

            ReactiveProperty<int> reactiveProperty = new ReactiveProperty<int>(0);

            reactiveProperty
                .OnCondition(() => condition)
                .Take(1)
                .Subscribe(value => receiver = value)
                .AddTo(_disposables);

            reactiveProperty.Value = 1;

            Assert.AreEqual(0, receiver);
            condition = true;

            reactiveProperty.Value = 2;

            Assert.AreEqual(2, receiver);

            reactiveProperty.Value = -1;
            Assert.AreEqual(2, receiver);
        }

        // NOTE: 追加テスト thx to Claude

        /// <summary>
        /// コンストラクタに渡した初期値がValueプロパティから取得できるか
        /// </summary>
        [Test]
        public void InitialValueShouldBeAccessible() {
            ReactiveProperty<int> reactiveProperty = new ReactiveProperty<int>(99);
            Assert.AreEqual(99, reactiveProperty.Value);
        }

        /// <summary>
        /// Set後にValueプロパティから最新値が取得できるか
        /// イベント購読なしでも値自体が正しく更新されているか
        /// </summary>
        [Test]
        public void ValueGetterShouldReturnCurrentValue() {
            ReactiveProperty<int> reactiveProperty = new ReactiveProperty<int>(0);
            reactiveProperty.Value = 42;
            Assert.AreEqual(42, reactiveProperty.Value);
        }

        // NOTE: 追加テスト 終

        [TearDown]
        public void TearDown() {
            _disposables.Dispose();
        }

        /// <summary>
        /// テスト用のスタブクラス
        /// </summary>
        private class TestClass {
            public int value = 0;
            public TestClass() { }
        }
    }
}