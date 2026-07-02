using System;
using UnityEngine.Events;

#if XQLUA_DEBUG
using System.Diagnostics;
using XqLua.Debug;
#endif

namespace XqLua.Unity {
    /// <summary>
    /// Publisherではなく、ButtonのonClickイベントの購読を抽象化したクラス
    /// </summary>
    public sealed class ButtonSubscription : IDisposableSubscription {
        private Action<Empty> _subscriber = default;
        private Action<UnityAction> _unsubscribe = default;

        public ButtonSubscription(Action<Empty> subscriber, Action<UnityAction> subscribe, Action<UnityAction> unsubscribe) {
            _subscriber = subscriber;
            _unsubscribe = unsubscribe;
            subscribe(OnEventInvoked);
#if XQLUA_DEBUG
            string caller = new StackFrame(2, false).GetMethod().DeclaringType.FullName;
            if (caller.Contains("Extension")) {
                caller = new StackFrame(3, false).GetMethod().DeclaringType.FullName;
            }
            DisposableDebug.Instance.AddDebug(this, "Subscription", caller);
#endif
        }

        private void OnEventInvoked() {
            _subscriber(Empty.Default);
        }

        public void Dispose() {
            _unsubscribe(OnEventInvoked);
            _subscriber = null;
#if XQLUA_DEBUG
            DisposableDebug.Instance.DisposeDebug(this);
#endif
        }
    }

    /// <summary>
    /// Publisherではなく、ButtonのonClickイベントの購読を抽象化したクラス
    /// </summary>
    /// <typeparam name="T">ButtonのonClickイベントに渡される引数の型</typeparam>
    public sealed class ButtonSubscription<T> : IDisposableSubscription {
        private Action<T> _subscriber = default;
        private Action<UnityAction> _unsubscribe = default;
        private T _value = default;

        public ButtonSubscription(Action<T> subscriber, Action<UnityAction> subscribe, Action<UnityAction> unsubscribe, T value) {
            _subscriber = subscriber;
            _unsubscribe = unsubscribe;

            _value = value;
            subscribe(OnEventInvoked);
#if XQLUA_DEBUG
            string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
            DisposableDebug.Instance.AddDebug(this, nameof(ButtonSubscription<T>), caller);
#endif
        }

        private void OnEventInvoked() {
            _subscriber(_value);
        }

        public void Dispose() {
            _unsubscribe(OnEventInvoked);
            _subscriber = null;
#if XQLUA_DEBUG
            DisposableDebug.Instance.DisposeDebug(this);
#endif
        }
    }
}