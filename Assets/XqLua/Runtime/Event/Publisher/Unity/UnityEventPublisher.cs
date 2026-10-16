using System;
using UnityEngine.Events;

#if XQLUA_DEBUG
using System.Diagnostics;
#endif

namespace XqLua.Unity {

    /// <summary>
    /// もともと存在するイベントを購読するPublisher
    /// </summary>
    /// <typeparam name="T">イベントの型</typeparam>
    public sealed class UnityEventPublisher<T> : IPublisher<T> {
        private Action<UnityAction<T>> _subscribe = default;
        private Action<UnityAction<T>> _unsubscribe = default;

        internal UnityEventPublisher(Action<UnityAction<T>> subscribe, Action<UnityAction<T>> unsubscribe) {
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
            return new UnityEventSubscription<T>(subscriber, _subscribe, _unsubscribe);
        }
    }

    /// <summary>
    /// もともと存在するイベントを購読するPublisher
    /// Action専用のUnityEventPublisher
    /// </summary>
    public sealed class UnityEventPublisher : IPublisher<Empty> {
        private Action<Action> _subscribe = default;
        private Action<Action> _unsubscribe = default;

        internal UnityEventPublisher(Action<Action> subscribe, Action<Action> unsubscribe) {
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

    /// <summary>
    /// もともと存在するイベントを購読するPublisher
    /// 第二引数に対応
    /// </summary>
    /// <typeparam name="T">イベントの第一引数の型</typeparam>
    /// <typeparam name="U">イベントの第二引数の型</typeparam>
    public sealed class UnityEventPublisher<T, U> : IPublisher<T, U> {
        private Action<UnityAction<T, U>> _subscribe = default;
        private Action<UnityAction<T, U>> _unsubscribe = default;

        internal UnityEventPublisher(Action<UnityAction<T, U>> subscribe, Action<UnityAction<T, U>> unsubscribe) {
            _subscribe = subscribe;
            _unsubscribe = unsubscribe;
        }

        /// <summary>
        /// 購読する
        /// </summary>
        /// <param name="subscriber">呼び出してほしいメソッド</param>
        /// <returns>購読の解除を担当するDisposableSubscription</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な場合、subscriberがnullの場合にスローされます</exception>
        public IDisposableSubscription Subscribe(Action<T, U> subscriber) {
#if XQLUA_DEBUG
            if (subscriber == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でSubscribeを呼ぼうとしたとき、subscriberがnullでした\n購読しようとしたメソッドが入ったクラスがnullだったり、Action<T>変数がnullではないですか？");
            }
#endif
            return new UnityEventSubscription<T, U>(subscriber, _subscribe, _unsubscribe);
        }
    }

    /// <summary>
    /// もともと存在するイベントを購読するPublisher
    /// 第二引数に対応
    /// </summary>
    /// <typeparam name="T">イベントの第一引数の型</typeparam>
    /// <typeparam name="U">イベントの第二引数の型</typeparam>
    /// <typeparam name="V">イベントの第三引数の型</typeparam>
    public sealed class UnityEventPublisher<T, U, V> : IPublisher<T, U, V> {
        private Action<UnityAction<T, U, V>> _subscribe = default;
        private Action<UnityAction<T, U, V>> _unsubscribe = default;

        internal UnityEventPublisher(Action<UnityAction<T, U, V>> subscribe, Action<UnityAction<T, U, V>> unsubscribe) {
            _subscribe = subscribe;
            _unsubscribe = unsubscribe;
        }

        /// <summary>
        /// 購読する
        /// </summary>
        /// <param name="subscriber">呼び出してほしいメソッド</param>
        /// <returns>購読の解除を担当するDisposableSubscription</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な場合、subscriberがnullの場合にスローされます</exception>
        public IDisposableSubscription Subscribe(Action<T, U, V> subscriber) {
#if XQLUA_DEBUG
            if (subscriber == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でSubscribeを呼ぼうとしたとき、subscriberがnullでした\n購読しようとしたメソッドが入ったクラスがnullだったり、Action<T>変数がnullではないですか？");
            }
#endif
            return new UnityEventSubscription<T, U, V>(subscriber, _subscribe, _unsubscribe);
        }
    }

    /// <summary>
    /// もともと存在するイベントを購読するPublisher
    /// 第二引数に対応
    /// </summary>
    /// <typeparam name="T">イベントの第一引数の型</typeparam>
    /// <typeparam name="U">イベントの第二引数の型</typeparam>
    /// <typeparam name="V">イベントの第三引数の型</typeparam>
    /// <typeparam name="W">イベントの第四引数の型</typeparam>
    public sealed class UnityEventPublisher<T, U, V, W> : IPublisher<T, U, V, W> {
        private Action<UnityAction<T, U, V, W>> _subscribe = default;
        private Action<UnityAction<T, U, V, W>> _unsubscribe = default;

        internal UnityEventPublisher(Action<UnityAction<T, U, V, W>> subscribe, Action<UnityAction<T, U, V, W>> unsubscribe) {
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
        public IDisposableSubscription Subscribe(Action<T, U, V, W> subscriber) {
#if XQLUA_DEBUG
            if (subscriber == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でSubscribeを呼ぼうとしたとき、subscriberがnullでした\n購読しようとしたメソッドが入ったクラスがnullだったり、Action<T>変数がnullではないですか？");
            }
#endif
            return new UnityEventSubscription<T, U, V, W>(subscriber, _subscribe, _unsubscribe);
        }
    }
}
