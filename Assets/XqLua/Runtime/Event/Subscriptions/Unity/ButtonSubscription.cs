using System;
using UnityEngine.Events;

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
        }

        private void OnEventInvoked() {
            _subscriber(Empty.Default);
        }

        public void Dispose() {
            _unsubscribe(OnEventInvoked);
            _subscriber = null;
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
        }

        private void OnEventInvoked() {
            _subscriber(_value);
        }

        public void Dispose() {
            _unsubscribe(OnEventInvoked);
            _subscriber = null;
        }
    }
}