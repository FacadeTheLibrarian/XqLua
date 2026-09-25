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

    /// <summary>
    /// C#のeventを抽象化したインターフェース
    /// 呼び出してほしいメソッドに引数を渡す
    /// </summary>
    /// <typeparam name="T">第一引数の型</typeparam>
    /// <typeparam name="U">第二引数の型</typeparam>
    public interface IPublisher<T, U> {
        /// <summary>
        /// 購読する
        /// </summary>
        /// <param name="subscriber">呼び出してほしいメソッド</param>
        /// <returns>購読の解除を担当するDisposableSubscription</returns>
        public IDisposableSubscription Subscribe(Action<T, U> subscriber);
    }

    /// <summary>
    /// C#のeventを抽象化したインターフェース
    /// 呼び出してほしいメソッドに引数を渡す
    /// </summary>
    /// <typeparam name="T">引数の型</typeparam>
    /// <typeparam name="U">第二引数の型</typeparam>
    /// <typeparam name="V">第三引数の型</typeparam>
    public interface IPublisher<T, U, V> {
        /// <summary>
        /// 購読する
        /// </summary>
        /// <param name="subscriber">呼び出してほしいメソッド</param>
        /// <returns>購読の解除を担当するDisposableSubscription</returns>
        public IDisposableSubscription Subscribe(Action<T, U, V> subscriber);
    }

    /// <summary>
    /// C#のeventを抽象化したインターフェース
    /// 呼び出してほしいメソッドに引数を渡す
    /// </summary>
    /// <typeparam name="T">引数の型</typeparam>
    /// <typeparam name="U">第二引数の型</typeparam>
    /// <typeparam name="V">第三引数の型</typeparam>
    /// <typeparam name="W">第四引数の型</typeparam>
    public interface IPublisher<T, U, V, W> {
        /// <summary>
        /// 購読する
        /// </summary>
        /// <param name="subscriber">呼び出してほしいメソッド</param>
        /// <returns>購読の解除を担当するDisposableSubscription</returns>
        public IDisposableSubscription Subscribe(Action<T, U, V, W> subscriber);
    }
}