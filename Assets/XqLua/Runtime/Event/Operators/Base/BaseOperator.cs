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
}
