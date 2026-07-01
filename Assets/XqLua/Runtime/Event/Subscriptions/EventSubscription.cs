using System;

namespace XqLua {
    /// <summary>
    /// Publisherではなく、イベントの購読を抽象化したクラス
    /// </summary>
    /// <typeparam name="T">イベントの型</typeparam>
    public sealed class EventSubscription<T> : IDisposableSubscription {
        private Action<T> _subscriber = default;
        private Action<Action<T>> _unsubscribe = default;

        public EventSubscription(Action<T> subscriber, Action<Action<T>> subscribe, Action<Action<T>> unsubscribe) {
            _subscriber = subscriber;
            _unsubscribe = unsubscribe;
            subscribe(OnEventInvoked);
        }

        private void OnEventInvoked(T value) {
            _subscriber(value);
        }

        public void Dispose() {
            _unsubscribe(OnEventInvoked);
            _subscriber = null;
        }
    }

    /// <summary>
    /// Publisherではなく、イベントの購読を抽象化したクラス
    /// Empty、もしくは引数なしのイベント専用
    /// </summary>
    public sealed class EventSubscription : IDisposableSubscription {
        private Action<Empty> _subscriber = default;
        private Action<Action> _unsubscribe = default;

        public EventSubscription(Action<Empty> subscriber, Action<Action> subscribe, Action<Action> unsubscribe) {
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
}