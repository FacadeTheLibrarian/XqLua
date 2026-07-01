namespace XqLua {
    /// <summary>
    /// 監視する値の変更をIPublisherを通して通知するインターフェース
    /// </summary>
    /// <typeparam name="T">監視する値の型</typeparam>
    public interface IReactiveProperty<T> : IPublisher<T> {
        /// <summary>
        /// 監視している値の現在の値
        /// </summary>
        public T Value { get; }
    }
}