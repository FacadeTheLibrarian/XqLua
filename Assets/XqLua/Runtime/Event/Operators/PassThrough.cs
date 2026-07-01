namespace XqLua {
    /// <summary>
    /// Publisherの値をそのまま流すOperator
    /// </summary>
    /// <typeparam name="T">Publisherの型</typeparam>
    internal sealed class PassThrough<T> : BaseOperator<T> {
        internal PassThrough(BaseOperator<T> previous) : base(previous, null) { }
        protected override void OnDispose() { }

        internal override bool IsConditionMet(T value) {
            if (_previous != null) {
                return _previous.IsConditionMet(value);
            }
            else {
                return true;
            }
        }
    }
}
