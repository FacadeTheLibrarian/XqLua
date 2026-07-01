using NUnit.Framework;
using System;
using XqLua.Extension;

namespace XqLua.Test {
    public class DisposablesTest {

        /// <summary>
        /// DisposablesのDisposeメソッドが正しく呼び出されるか？
        /// </summary>
        [Test]
        public void DisposeCallShouldWork() {
            Disposables disposables = new Disposables();
            TestClass testClass = new TestClass(-1).AddTo(disposables);
            disposables.Dispose();
            Assert.IsTrue(testClass.isDisposed);
        }

        /// <summary>
        /// Disposablesに自分自身をAddToした後Disposeしても正常にべき等でDisposeされるか？
        /// </summary>
        [Test]
        public void AddToSelfThenDisposeNeverThrows() {
            Disposables disposables = new Disposables();
            disposables.AddTo(disposables);
            Assert.DoesNotThrow(() => disposables.Dispose());
        }

        // NOTE: 追加テスト thx to Claude

        /// <summary>
        /// 既にDisposeされたDisposablesにAddToすると即座にDisposeされるか？
        /// </summary>
        [Test]
        public void AddToAlreadyDisposedDisposablesDisposesImmediately() {
            Disposables disposables = new Disposables();
            disposables.Dispose();

            TestClass testClass = new TestClass(-1).AddTo(disposables);

            Assert.IsTrue(testClass.isDisposed);
        }

        /// <summary>
        /// Disposablesを二回Disposeしても例外が発生しないか？
        /// </summary>
        [Test]
        public void DoubleDisposeNeverThrows() {
            Disposables disposables = new Disposables();
            disposables.Dispose();

            Assert.DoesNotThrow(() => disposables.Dispose());
        }
        // NOTE: 追加テスト 終

        /// <summary>
        /// テスト用のスタブクラス
        /// </summary>
        private class TestClass : IDisposable {
            public int value = 0;
            public bool isDisposed = false;

            public TestClass(int initialValue) {
                value = initialValue;
            }
            public void Dispose() {
                isDisposed = true;
            }
        }
    }
}