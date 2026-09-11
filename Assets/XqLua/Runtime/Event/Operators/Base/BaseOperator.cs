using System;

namespace XqLua {
    /// <summary>
    /// Operatorの基底クラス
    /// このクラスを継承すると自作のOperatorを作ることができる
    /// </summary>
    /// <typeparam name="T">Publisherの型</typeparam>
    public abstract class BaseOperator<T> : IDisposable {
        internal IPublisher<T> Publisher => _publisher;

        protected IPublisher<T> _publisher = default;
        protected BaseOperator<T> _previous = default;
        public BaseOperator(BaseOperator<T> previous, IPublisher<T> sourcePublisher) {
            _publisher = sourcePublisher;
            _previous = previous;
        }
        public void Dispose() {
            OnDispose();
            if (_previous != null) {
                _previous.Dispose();
            }
        }
        /// <summary>
        /// リソースの破棄
        /// </summary>
        protected abstract void OnDispose();
        /// <summary>
        /// Operatorの条件を満たすかどうかを判定する
        /// </summary>
        /// <param name="value">判定対象の値</param>
        /// <returns>条件を満たす場合はtrue、そうでない場合はfalse</returns>
        internal abstract bool IsConditionMet(T value);
    }

    /// <summary>
    /// Operatorの基底クラス
    /// このクラスを継承すると自作のOperatorを作ることができる
    /// </summary>
    /// <typeparam name="T">Publisherの第一引数の型</typeparam>
    /// <typeparam name="U">Publisherの第二引数の型</typeparam>
    public abstract class BaseOperator<T, U> : IDisposable {
        internal IPublisher<T, U> Publisher => _publisher;

        protected IPublisher<T, U> _publisher = default;
        protected BaseOperator<T, U> _previous = default;
        public BaseOperator(BaseOperator<T, U> previous, IPublisher<T, U> sourcePublisher) {
            _publisher = sourcePublisher;
            _previous = previous;
        }
        public void Dispose() {
            OnDispose();
            if (_previous != null) {
                _previous.Dispose();
            }
        }
        /// <summary>
        /// リソースの破棄
        /// </summary>
        protected abstract void OnDispose();
        /// <summary>
        /// Operatorの条件を満たすかどうかを判定する
        /// </summary>
        /// <param name="first">判定対象の第一引数</param>
        /// <param name="second">判定対象の第二引数</param>
        /// <returns>条件を満たす場合はtrue、そうでない場合はfalse</returns>
        internal abstract bool IsConditionMet(T first, U second);
    }
}
