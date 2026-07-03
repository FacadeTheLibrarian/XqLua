using System;

namespace XqLua {
    /// <summary>
    /// C#のeventを抽象化したインターフェース
    /// 呼び出してほしいメソッドに引数を渡す
    /// </summary>
    /// <typeparam name="T">引数の型</typeparam>
    public interface IPublisher<T> {
        /// <summary>
        /// 購読する
        /// </summary>
        /// <param name="subscriber">呼び出してほしいメソッド</param>
        /// <returns>購読の解除を担当するDisposableSubscription</returns>
        public IDisposableSubscription Subscribe(Action<T> subscriber);
    }
}