using System;
using System.Threading;
using UnityEngine;

namespace XqLua.Async {

    /// <summary>
    /// Publisherの購読を非同期で待機するための拡張メソッドを提供するクラス
    /// </summary>
    public static class PublisherAsyncExtension {
        /// <summary>
        /// Publisherの購読を非同期で待機するための拡張メソッド
        /// 1回だけAwaitすることができる
        /// </summary>
        /// <typeparam name="T">Publisherの型</typeparam>
        /// <param name="publisher">購読するPublisher</param>
        /// <param name="token">キャンセルトークン</param>
        /// <returns>非同期で待機可能なAwaitableオブジェクト</returns>
        public static async Awaitable<T> AwaitableSubscription<T>(this IPublisher<T> publisher, CancellationToken token) {
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
    }
}
