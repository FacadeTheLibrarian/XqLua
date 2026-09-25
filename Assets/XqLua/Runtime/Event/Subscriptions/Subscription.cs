using System;

#if XQLUA_DEBUG
using System.Diagnostics;
using XqLua.Debugger;
#endif

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

        /// <summary>
        /// Operatorに対して購読を行う拡張メソッド
        /// </summary>
        /// <typeparam name="T">Publisher/Operatorの第一引数の型</typeparam>
        /// <typeparam name="U">Publisher/Operatorの第二引数の型</typeparam>
        /// <param name="source">対象のOperator</param>
        /// <param name="subscriber">購読者のメソッド</param>
        /// <returns>購読の解除を担当するDisposableSubscription</returns>
        public static IDisposableSubscription Subscribe<T, U>(this BaseOperator<T, U> source, Action<T, U> subscriber) {
            Action<T, U> wrapper = (first, second) => {
                if (source.IsConditionMet(first, second)) {
                    subscriber(first, second);
                }
            };
            IDisposableSubscription subscription = source.Publisher.Subscribe(wrapper);
            return new OperatorSubscription<T, U>(subscription, source);
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
#if XQLUA_DEBUG
            string caller = new StackFrame(2, false).GetMethod().DeclaringType.FullName;
            if (caller.Contains("Extension")) {
                caller = new StackFrame(3, false).GetMethod().DeclaringType.FullName;
            }
            DisposableDebugger.Instance.AddDebug(this, "Subscription", caller);
#endif
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

#if XQLUA_DEBUG
            DisposableDebugger.Instance.DisposeDebug(this);
#endif
        }
    }

    /// <summary>
    /// 購読を抽象化したクラス
    /// </summary>
    /// <typeparam name="T">購読するPublisherの第一引数の型</typeparam>
    /// <typeparam name="U">購読するPublisherの第二引数の型</typeparam>
    internal sealed class Subscription<T, U> : IDisposableSubscription {
        private Action<T, U> _subscriber = default;
        private Action _unsubscription = default;

        public Subscription(Action<T, U> subscriber) {
            _subscriber = subscriber;
#if XQLUA_DEBUG
            string caller = new StackFrame(2, false).GetMethod().DeclaringType.FullName;
            if (caller.Contains("Extension")) {
                caller = new StackFrame(3, false).GetMethod().DeclaringType.FullName;
            }
            DisposableDebugger.Instance.AddDebug(this, "Subscription", caller);
#endif
        }

        public void SetUnsubscription(Action unsubscription) {
            _unsubscription = unsubscription;
        }

        public void OnEventInvoked(T firstValue, U secondParameter) {
            _subscriber(firstValue, secondParameter);
        }

        public void Dispose() {
            _unsubscription();
            _subscriber = null;

#if XQLUA_DEBUG
            DisposableDebugger.Instance.DisposeDebug(this);
#endif
        }
    }

    /// <summary>
    /// 購読を抽象化したクラス
    /// </summary>
    /// <typeparam name="T">購読するPublisherの第一引数の型</typeparam>
    /// <typeparam name="U">購読するPublisherの第二引数の型</typeparam>
    /// <typeparam name="V">購読するPublisherの第三引数の型</typeparam>
    internal sealed class Subscription<T, U, V> : IDisposableSubscription {
        private Action<T, U, V> _subscriber = default;
        private Action _unsubscription = default;

        public Subscription(Action<T, U, V> subscriber) {
            _subscriber = subscriber;
#if XQLUA_DEBUG
            string caller = new StackFrame(2, false).GetMethod().DeclaringType.FullName;
            if (caller.Contains("Extension")) {
                caller = new StackFrame(3, false).GetMethod().DeclaringType.FullName;
            }
            DisposableDebugger.Instance.AddDebug(this, "Subscription", caller);
#endif
        }

        public void SetUnsubscription(Action unsubscription) {
            _unsubscription = unsubscription;
        }

        public void OnEventInvoked(T firstValue, U secondParameter, V thirdParameter) {
            _subscriber(firstValue, secondParameter, thirdParameter);
        }

        public void Dispose() {
            _unsubscription();
            _subscriber = null;

#if XQLUA_DEBUG
            DisposableDebugger.Instance.DisposeDebug(this);
#endif
        }
    }

    /// <summary>
    /// 購読を抽象化したクラス
    /// </summary>
    /// <typeparam name="T">購読するPublisherの第一引数の型</typeparam>
    /// <typeparam name="U">購読するPublisherの第二引数の型</typeparam>
    /// <typeparam name="V">購読するPublisherの第三引数の型</typeparam>
    /// <typeparam name="W">購読するPublisherの第四引数の型</typeparam>
    internal sealed class Subscription<T, U, V, W> : IDisposableSubscription {
        private Action<T, U, V, W> _subscriber = default;
        private Action _unsubscription = default;

        public Subscription(Action<T, U, V, W> subscriber) {
            _subscriber = subscriber;
#if XQLUA_DEBUG
            string caller = new StackFrame(2, false).GetMethod().DeclaringType.FullName;
            if (caller.Contains("Extension")) {
                caller = new StackFrame(3, false).GetMethod().DeclaringType.FullName;
            }
            DisposableDebugger.Instance.AddDebug(this, "Subscription", caller);
#endif
        }

        public void SetUnsubscription(Action unsubscription) {
            _unsubscription = unsubscription;
        }

        public void OnEventInvoked(T firstValue, U secondParameter, V thirdParameter, W fourthParameter) {
            _subscriber(firstValue, secondParameter, thirdParameter, fourthParameter);
        }

        public void Dispose() {
            _unsubscription();
            _subscriber = null;

#if XQLUA_DEBUG
            DisposableDebugger.Instance.DisposeDebug(this);
#endif
        }
    }
}