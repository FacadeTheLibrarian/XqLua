namespace XqLua {
    /// <summary>
    /// Operatorの購読を抽象化したクラス
    /// </summary>
    /// <typeparam name="T">Operatorの型</typeparam>
    internal sealed class OperatorSubscription<T> : IDisposableSubscription {
        private IDisposableSubscription _disposableSubscription = default;
        private BaseOperator<T> _disposableOperator = default;

        public OperatorSubscription(IDisposableSubscription disposableSubscription, BaseOperator<T> disposableOperator) {
            _disposableSubscription = disposableSubscription;
            _disposableOperator = disposableOperator;
        }

        public void Dispose() {
            _disposableSubscription.Dispose();
            _disposableOperator.Dispose();
        }
    }
}