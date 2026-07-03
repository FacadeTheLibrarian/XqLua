using System;

#if XQLUA_DEBUG
using System.Diagnostics;
using XqLua.Debug;
#endif

namespace XqLua {
    public class Publisher<T> : IPublisher<T>, IDisposable {

        internal event Action<T> OnEventInvoked = delegate { };
#if XQLUA_DEBUG
        private bool _isDisposed = false;
#endif
        public Publisher() {
#if XQLUA_DEBUG
            string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
            DisposableDebug.Instance.AddDebug(this, nameof(Publisher<T>), caller);
#endif
        }

        /// <summary>
        /// リソースの破棄
        /// </summary>
        public void Dispose() {
#if XQLUA_DEBUG
            _isDisposed = true;
            DisposableDebug.Instance.DisposeDebug(this);
#endif
            //NOTE: 本家SubjectはDisposeしないとリークするので、nopのDisposeを用意
            //      Debug状態ではDisposeがかかっているかを見る
            //UPDATE: 一応delegate { }で
            OnEventInvoked = delegate { };
        }

        /// <summary>
        /// 購読する
        /// </summary>
        /// <param name="subscriber">呼び出してほしいメソッド</param>
        /// <returns>購読の解除を担当するDisposableSubscription</returns>
        /// <exception cref="AccessViolationException">デバッグが有効な場合、すでにDisposeされたPublisherを購読しようとしたときにスローされます</exception>
        /// <exception cref="ArgumentNullException">デバッグが有効な場合、subscriberがnullの場合にスローされます</exception>
        public IDisposableSubscription Subscribe(Action<T> subscriber) {
#if XQLUA_DEBUG
            if (_isDisposed) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new AccessViolationException($"すでにDisposeされたPublisherを{caller}が購読しようとしています");
            }
            if (subscriber == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でSubscribeを呼ぼうとしたとき、subscriberがnullでした\n購読しようとしたメソッドが入ったクラスがnullだったり、Action<T>変数がnullではないですか？");
            }
#endif
            Subscription<T> subscription = new Subscription<T>(subscriber);
            OnEventInvoked += subscription.OnEventInvoked;
            subscription.SetUnsubscription(() => OnEventInvoked -= subscription.OnEventInvoked);
            return subscription;
        }

        /// <summary>
        /// 購読者にイベントを発火する
        /// </summary>
        /// <param name="value">発火するイベントの値</param>
        /// <exception cref="AccessViolationException">デバッグが有効な場合、すでにDisposeされたPublisherを発火しようとしたときにスローされます</exception>
        public void Invoke(T value) {
#if XQLUA_DEBUG
            if (_isDisposed) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new AccessViolationException($"すでにDisposeされたPublisherを{caller}が発火しようとしています");
            }
#endif
            OnEventInvoked.Invoke(value);
        }

        /// <summary>
        /// Publisherをイベントから作成する
        /// </summary>
        /// <param name="subscribe">イベントの購読を開始するためのデリゲート</param>
        /// <param name="unsubscribe">イベントの購読を解除するためのデリゲート</param>
        /// <returns>作成されたPublisher</returns>
        public static IPublisher<Empty> FromEvent(Action<Action> subscribe, Action<Action> unsubscribe) {
            return new EventPublisher(subscribe, unsubscribe);
        }

        /// <summary>
        /// Publisherをイベントから作成する
        /// </summary>
        /// <param name="subscribe">イベントの購読を開始するためのデリゲート</param>
        /// <param name="unsubscribe">イベントの購読を解除するためのデリゲート</param>
        /// <returns>作成されたPublisher</returns>
        public static IPublisher<T> FromEvent(Action<Action<T>> subscribe, Action<Action<T>> unsubscribe) {
            return new EventPublisher<T>(subscribe, unsubscribe);
        }
    }
}