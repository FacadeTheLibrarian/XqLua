using System;
using System.Threading;
using UnityEngine;
using XqLua.Message;

namespace XqLua.Async {

    /// <summary>
    /// Publisherの購読を非同期で待機するための拡張メソッドを提供するクラス
    /// </summary>
    public static class PublisherAsyncExtension {
        /// <summary>
        /// AwaitableSubscriptionは非推奨になりました。まもなく削除されます。
        /// 代わりにAwaitableSubscribeを使用してください。
        /// </summary>
        [Obsolete]
        public static async Awaitable<T> AwaitableSubscription<T>(this IPublisher<T> publisher) {
            AwaitableCompletionSource<T> completionSource = new AwaitableCompletionSource<T>();
            Action<T> subscriber = value => completionSource.TrySetResult(value);
            IDisposableSubscription subscription = publisher.Subscribe(subscriber);
            T result = default;
            try {
                result = await completionSource.Awaitable;
            }
            catch {
                throw;
            }
            finally {
                subscription.Dispose();
            }
            return result;
        }

        /// <summary>
        /// Publisherの購読を非同期で待機するための拡張メソッド
        /// 1回だけAwaitすることができる
        /// </summary>
        /// <typeparam name="T">Publisherの型</typeparam>
        /// <param name="publisher">購読するPublisher</param>
        /// <param name="token">キャンセルトークン</param>
        /// <returns>非同期で待機可能なAwaitableオブジェクト</returns>
        public static async Awaitable<T> AwaitableSubscribe<T>(this IPublisher<T> publisher, CancellationToken token) {
            AwaitableCompletionSource<T> completionSource = new AwaitableCompletionSource<T>();
            CancellationTokenRegistration sourceRegistration = token.Register(() => completionSource.TrySetCanceled());

            Action<T> subscriber = value => completionSource.TrySetResult(value);

            IDisposableSubscription subscription = publisher.Subscribe(subscriber);
            T result = default;
            try {
                result = await completionSource.Awaitable;
            }
            catch {
                throw;
            }
            finally {
                subscription.Dispose();
                sourceRegistration.Dispose();
            }
            return result;
        }

        /// <summary>
        /// ReactivePropertyの購読を非同期で待機するための拡張メソッド
        /// 1回だけAwaitすることができる
        /// 最初の値は無視され、次に値が更新したときにAwaitが完了する
        /// </summary>
        /// <typeparam name="T">ReactivePropertyの型</typeparam>
        /// <param name="reactiveProperty">購読するReactiveProperty</param>
        /// <param name="token">キャンセルトークン</param>
        /// <returns>非同期で待機可能なAwaitableオブジェクト</returns>
        public static async Awaitable<T> AwaitableSubscribe<T>(this IReactiveProperty<T> reactiveProperty, CancellationToken token) {
            AwaitableCompletionSource<T> completionSource = new AwaitableCompletionSource<T>();
            CancellationTokenRegistration sourceRegistration = token.Register(() => completionSource.TrySetCanceled());

            Action<T> subscriber = value => completionSource.TrySetResult(value);

            IDisposableSubscription subscription = reactiveProperty.Skip(1).Subscribe(subscriber);
            T result = default;
            try {
                result = await completionSource.Awaitable;
            }
            catch {
                throw;
            }
            finally {
                subscription.Dispose();
                sourceRegistration.Dispose();
            }
            return result;
        }

        /// <summary>
        /// MessageTrayを購読し、値を取得するまで非同期で待機するための拡張メソッド
        /// 1回だけAwaitすることができる
        /// もしもうすでにMessageTrayに値が入っている場合は、即座にその値を返す
        /// また、メッセージは消費せず、MessageTrayに残る
        /// </summary>
        /// <typeparam name="T">MessageTrayの型</typeparam>
        /// <param name="tray">購読するMessageTray</param>
        /// <param name="token">キャンセルトークン</param>
        /// <returns>非同期で待機可能なAwaitableオブジェクト</returns>
        public static async Awaitable<T> AwaitableSubscribe<T>(this IMessageTray<T> tray, CancellationToken token) where T : class {
            if(tray.TryReadMessage(out T message)) {
                return message;
            }

            AwaitableCompletionSource<T> completionSource = new AwaitableCompletionSource<T>();
            CancellationTokenRegistration sourceRegistration = token.Register(() => completionSource.TrySetCanceled());

            Action<T> subscriber = value => completionSource.TrySetResult(value);
            IDisposableSubscription subscription = tray.OnMessageArrived.Subscribe(subscriber);
            T result = default;
            try {
                result = await completionSource.Awaitable;
            }
            catch {
                throw;
            }
            finally {
                subscription.Dispose();
                sourceRegistration.Dispose();
            }
            return result;
        }

        /// <summary>
        /// Publisherの購読を非同期で待機するための拡張メソッド
        /// 第二引数に対応
        /// 1回だけAwaitすることができる
        /// </summary>
        /// <typeparam name="T">Publisherの第一引数型</typeparam>
        /// <typeparam name="U">Publisherの第二引数型</typeparam>
        /// <param name="publisher">購読するPublisher</param>
        /// <param name="token">キャンセルトークン</param>
        /// <returns>非同期で待機可能なAwaitableオブジェクト</returns>
        public static async Awaitable<(T first, U second)> AwaitableSubscribe<T, U>(this IPublisher<T, U> publisher, CancellationToken token) {
            AwaitableCompletionSource<(T, U)> completionSource = new AwaitableCompletionSource<(T, U)>();
            CancellationTokenRegistration sourceRegistration = token.Register(() => completionSource.TrySetCanceled());

            Action<T, U> subscriber = (value1, value2) => completionSource.TrySetResult((value1, value2));

            IDisposableSubscription subscription = publisher.Subscribe(subscriber);
            (T, U) result = default;
            try {
                result = await completionSource.Awaitable;
            }
            catch {
                throw;
            }
            finally {
                subscription.Dispose();
                sourceRegistration.Dispose();
            }
            return result;
        }

        /// <summary>
        /// Publisherの購読を非同期で待機するための拡張メソッド
        /// 第三引数に対応
        /// 1回だけAwaitすることができる
        /// </summary>
        /// <typeparam name="T">Publisherの第一引数型</typeparam>
        /// <typeparam name="U">Publisherの第二引数型</typeparam>
        /// <typeparam name="V">Publisherの第三引数型</typeparam>
        /// <param name="publisher">購読するPublisher</param>
        /// <param name="token">キャンセルトークン</param>
        /// <returns>非同期で待機可能なAwaitableオブジェクト</returns>
        public static async Awaitable<(T first, U second, V third)> AwaitableSubscribe<T, U, V>(this IPublisher<T, U, V> publisher, CancellationToken token) {
            AwaitableCompletionSource<(T, U, V)> completionSource = new AwaitableCompletionSource<(T, U, V)>();
            CancellationTokenRegistration sourceRegistration = token.Register(() => completionSource.TrySetCanceled());

            Action<T, U, V> subscriber = (value1, value2, value3) => completionSource.TrySetResult((value1, value2, value3));

            IDisposableSubscription subscription = publisher.Subscribe(subscriber);
            (T, U, V) result = default;
            try {
                result = await completionSource.Awaitable;
            }
            catch {
                throw;
            }
            finally {
                subscription.Dispose();
                sourceRegistration.Dispose();
            }
            return result;
        }

        /// <summary>
        /// Publisherの購読を非同期で待機するための拡張メソッド
        /// 第三引数に対応
        /// 1回だけAwaitすることができる
        /// </summary>
        /// <typeparam name="T">Publisherの第一引数型</typeparam>
        /// <typeparam name="U">Publisherの第二引数型</typeparam>
        /// <typeparam name="V">Publisherの第三引数型</typeparam>
        /// <typeparam name="W">Publisherの第四引数型</typeparam>
        /// <param name="publisher">購読するPublisher</param>
        /// <param name="token">キャンセルトークン</param>
        /// <returns>非同期で待機可能なAwaitableオブジェクト</returns>
        public static async Awaitable<(T first, U second, V third, W fourth)> AwaitableSubscribe<T, U, V, W>(this IPublisher<T, U, V, W> publisher, CancellationToken token) {
            AwaitableCompletionSource<(T, U, V, W)> completionSource = new AwaitableCompletionSource<(T, U, V, W)>();
            CancellationTokenRegistration sourceRegistration = token.Register(() => completionSource.TrySetCanceled());

            Action<T, U, V, W> subscriber = (value1, value2, value3, value4) => completionSource.TrySetResult((value1, value2, value3, value4));

            IDisposableSubscription subscription = publisher.Subscribe(subscriber);
            (T, U, V, W) result = default;
            try {
                result = await completionSource.Awaitable;
            }
            catch {
                throw;
            }
            finally {
                subscription.Dispose();
                sourceRegistration.Dispose();
            }
            return result;
        }
    }
}
