using System;

namespace XqLua {
    /// <summary>
    /// 購読するための拡張メソッドを提供するクラス
    /// </summary>
    public static class SubscriptionExtension {
        /// <summary>
        /// Operatorに対して購読を行う拡張メソッド
        /// </summary>
        /// <typeparam name="T">Publisher/Operatorの型</typeparam>
        /// <param name="source">対象のOperator</param>
        /// <param name="subscriber">購読者のメソッド</param>
        /// <returns>購読の解除を担当するDisposableSubscription</returns>
        public static IDisposableSubscription Subscribe<T>(this BaseOperator<T> source, Action<T> subscriber) {
            Action<T> wrapper = (value) => {
                if (source.IsConditionMet(value)) {
                    subscriber(value);
                }
            };
            IDisposableSubscription subscription = source.Publisher.Subscribe(wrapper);
            return new OperatorSubscription<T>(subscription, source);
        }
    }

    /// <summary>
    /// 購読を抽象化したクラス
    /// </summary>
    /// <typeparam name="T">購読するPublisherの型</typeparam>
    internal sealed class Subscription<T> : IDisposableSubscription {
        private Action<T> _subscriber = default;
        private Action _unsubscription = default;

        public Subscription(Action<T> subscriber) {
            _subscriber = subscriber;
        }

        public void SetUnsubscription(Action unsubscription) {
            _unsubscription = unsubscription;
        }

        public void OnEventInvoked(T value) {
            _subscriber(value);
        }

        public void Dispose() {
            _unsubscription();
            _subscriber = null;
        }
    }
}