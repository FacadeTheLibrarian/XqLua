using System;
using System.Diagnostics;

namespace XqLua {

    /// <summary>
    /// もともと存在するイベントを購読するPublisher
    /// </summary>
    /// <typeparam name="T">イベントの型</typeparam>
    public sealed class EventPublisher<T> : IPublisher<T> {
        private Action<Action<T>> _subscribe = default;
        private Action<Action<T>> _unsubscribe = default;

        internal EventPublisher(Action<Action<T>> subscribe, Action<Action<T>> unsubscribe) {
            _subscribe = subscribe;
            _unsubscribe = unsubscribe;
        }

        //NOTE: 本家Observable.FromEventではDisposableが実装されていないので
        //      ここでも実装せずで
        /// <summary>
        /// 購読する
        /// </summary>
        /// <param name="subscriber">呼び出してほしいメソッド</param>
        /// <returns>購読の解除を担当するDisposableSubscription</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な場合、subscriberがnullの場合にスローされます</exception>
        public IDisposableSubscription Subscribe(Action<T> subscriber) {
#if XQLUA_DEBUG
            if (subscriber == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でSubscribeを呼ぼうとしたとき、subscriberがnullでした\n購読しようとしたメソッドが入ったクラスがnullだったり、Action<T>変数がnullではないですか？");
            }
#endif
            return new EventSubscription<T>(subscriber, _subscribe, _unsubscribe);
        }
    }

    /// <summary>
    /// もともと存在するイベントを購読するPublisher
    /// Action専用のEventPublisher
    /// </summary>
    public sealed class EventPublisher : IPublisher<Empty> {
        private Action<Action> _subscribe = default;
        private Action<Action> _unsubscribe = default;

        internal EventPublisher(Action<Action> subscribe, Action<Action> unsubscribe) {
            _subscribe = subscribe;
            _unsubscribe = unsubscribe;
        }
        /// <summary>
        /// 購読する
        /// </summary>
        /// <param name="subscriber">呼び出してほしいメソッド</param>
        /// <returns>購読の解除を担当するDisposableSubscription</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な場合、subscriberがnullの場合にスローされます</exception>
        public IDisposableSubscription Subscribe(Action<Empty> subscriber) {
#if XQLUA_DEBUG
            if (subscriber == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でSubscribeを呼ぼうとしたとき、subscriberがnullでした\n購読しようとしたメソッドが入ったクラスがnullだったり、Action<T>変数がnullではないですか？");
            }
#endif
            return new EventSubscription(subscriber, _subscribe, _unsubscribe);
        }
    }
}
