using System;

#if XQLUA_DEBUG
using System.Diagnostics;
using XqLua.Debugger;
#endif

namespace XqLua {
    /// <summary>
    /// C#のeventを抽象化したクラス
    /// </summary>
    /// <typeparam name="T">イベントの値の型</typeparam>
    public class Publisher<T> : IPublisher<T>, IDisposable {

        internal event Action<T> OnEventInvoked = delegate { };
#if XQLUA_DEBUG
        private bool _isDisposed = false;
#endif
        public Publisher() {
#if XQLUA_DEBUG
            string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
            DisposableDebugger.Instance.AddDebug(this, nameof(Publisher<T>), caller);
#endif
        }

        /// <summary>
        /// リソースの破棄
        /// </summary>
        public void Dispose() {
#if XQLUA_DEBUG
            _isDisposed = true;
            DisposableDebugger.Instance.DisposeDebug(this);
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
        /// Publisherを引数のないイベントから作成する
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

    /// <summary>
    /// Publisherを第二引数まで対応させたクラス
    /// </summary>
    /// <typeparam name="T">イベントの第一引数の型</typeparam>
    /// <typeparam name="U">イベントの第二引数の型</typeparam>
    public class Publisher<T, U> : IPublisher<T, U>, IDisposable {

        internal event Action<T, U> OnEventInvoked = delegate { };
#if XQLUA_DEBUG
        private bool _isDisposed = false;
#endif
        public Publisher() {
#if XQLUA_DEBUG
            string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
            DisposableDebugger.Instance.AddDebug(this, nameof(Publisher<T>), caller);
#endif
        }

        /// <summary>
        /// リソースの破棄
        /// </summary>
        public void Dispose() {
#if XQLUA_DEBUG
            _isDisposed = true;
            DisposableDebugger.Instance.DisposeDebug(this);
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
        public IDisposableSubscription Subscribe(Action<T, U> subscriber) {
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
            Subscription<T, U> subscription = new Subscription<T, U>(subscriber);
            OnEventInvoked += subscription.OnEventInvoked;
            subscription.SetUnsubscription(() => OnEventInvoked -= subscription.OnEventInvoked);
            return subscription;
        }

        /// <summary>
        /// 購読者にイベントを発火する
        /// </summary>
        /// <param name="first">発火するイベントの第一引数</param>
        /// <param name="second">発火するイベントの第二引数</param>
        /// <exception cref="AccessViolationException">デバッグが有効な場合、すでにDisposeされたPublisherを発火しようとしたときにスローされます</exception>
        public void Invoke(T first, U second) {
#if XQLUA_DEBUG
            if (_isDisposed) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new AccessViolationException($"すでにDisposeされたPublisherを{caller}が発火しようとしています");
            }
#endif
            OnEventInvoked.Invoke(first, second);
        }

        /// <summary>
        /// Publisherを引数を二つ取るイベントから作成する
        /// </summary>
        /// <param name="subscribe">イベントの購読を開始するためのデリゲート</param>
        /// <param name="unsubscribe">イベントの購読を解除するためのデリゲート</param>
        /// <returns>作成されたPublisher</returns>
        public static IPublisher<T, U> FromEvent(Action<Action<T, U>> subscribe, Action<Action<T, U>> unsubscribe) {
            return new EventPublisher<T, U>(subscribe, unsubscribe);
        }
    }

    /// <summary>
    /// Publisherを第三引数まで対応させたクラス
    /// </summary>
    /// <typeparam name="T">イベントの第一引数の型</typeparam>
    /// <typeparam name="U">イベントの第二引数の型</typeparam>
    /// <typeparam name="V">イベントの第三引数の型</typeparam>
    public class Publisher<T, U, V> : IPublisher<T, U, V>, IDisposable {

        internal event Action<T, U, V> OnEventInvoked = delegate { };
#if XQLUA_DEBUG
        private bool _isDisposed = false;
#endif
        public Publisher() {
#if XQLUA_DEBUG
            string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
            DisposableDebugger.Instance.AddDebug(this, nameof(Publisher<T>), caller);
#endif
        }

        /// <summary>
        /// リソースの破棄
        /// </summary>
        public void Dispose() {
#if XQLUA_DEBUG
            _isDisposed = true;
            DisposableDebugger.Instance.DisposeDebug(this);
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
        public IDisposableSubscription Subscribe(Action<T, U, V> subscriber) {
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
            Subscription<T, U, V> subscription = new Subscription<T, U, V>(subscriber);
            OnEventInvoked += subscription.OnEventInvoked;
            subscription.SetUnsubscription(() => OnEventInvoked -= subscription.OnEventInvoked);
            return subscription;
        }

        /// <summary>
        /// 購読者にイベントを発火する
        /// </summary>
        /// <param name="first">発火するイベントの第一引数</param>
        /// <param name="second">発火するイベントの第二引数</param>
        /// <param name="third">発火するイベントの第三引数</param>
        /// <exception cref="AccessViolationException">デバッグが有効な場合、すでにDisposeされたPublisherを発火しようとしたときにスローされます</exception>
        public void Invoke(T first, U second, V third) {
#if XQLUA_DEBUG
            if (_isDisposed) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new AccessViolationException($"すでにDisposeされたPublisherを{caller}が発火しようとしています");
            }
#endif
            OnEventInvoked.Invoke(first, second, third);
        }

        /// <summary>
        /// Publisherを引数を二つ取るイベントから作成する
        /// </summary>
        /// <param name="subscribe">イベントの購読を開始するためのデリゲート</param>
        /// <param name="unsubscribe">イベントの購読を解除するためのデリゲート</param>
        /// <returns>作成されたPublisher</returns>
        public static IPublisher<T, U, V> FromEvent(Action<Action<T, U, V>> subscribe, Action<Action<T, U, V>> unsubscribe) {
            return new EventPublisher<T, U, V>(subscribe, unsubscribe);
        }
    }

    /// <summary>
    /// Publisherを第三引数まで対応させたクラス
    /// </summary>
    /// <typeparam name="T">イベントの第一引数の型</typeparam>
    /// <typeparam name="U">イベントの第二引数の型</typeparam>
    /// <typeparam name="V">イベントの第三引数の型</typeparam>
    /// <typeparam name="W">イベントの第四引数の型</typeparam>
    public class Publisher<T, U, V, W> : IPublisher<T, U, V, W>, IDisposable {

        internal event Action<T, U, V, W> OnEventInvoked = delegate { };
#if XQLUA_DEBUG
        private bool _isDisposed = false;
#endif
        public Publisher() {
#if XQLUA_DEBUG
            string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
            DisposableDebugger.Instance.AddDebug(this, nameof(Publisher<T>), caller);
#endif
        }

        /// <summary>
        /// リソースの破棄
        /// </summary>
        public void Dispose() {
#if XQLUA_DEBUG
            _isDisposed = true;
            DisposableDebugger.Instance.DisposeDebug(this);
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
        public IDisposableSubscription Subscribe(Action<T, U, V, W> subscriber) {
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
            Subscription<T, U, V, W> subscription = new Subscription<T, U, V, W>(subscriber);
            OnEventInvoked += subscription.OnEventInvoked;
            subscription.SetUnsubscription(() => OnEventInvoked -= subscription.OnEventInvoked);
            return subscription;
        }

        /// <summary>
        /// 購読者にイベントを発火する
        /// </summary>
        /// <param name="first">発火するイベントの第一引数</param>
        /// <param name="second">発火するイベントの第二引数</param>
        /// <param name="third">発火するイベントの第三引数</param>
        /// <param name="fourth">発火するイベントの第四引数</param>
        /// <exception cref="AccessViolationException">デバッグが有効な場合、すでにDisposeされたPublisherを発火しようとしたときにスローされます</exception>
        public void Invoke(T first, U second, V third, W fourth) {
#if XQLUA_DEBUG
            if (_isDisposed) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new AccessViolationException($"すでにDisposeされたPublisherを{caller}が発火しようとしています");
            }
#endif
            OnEventInvoked.Invoke(first, second, third, fourth);
        }

        /// <summary>
        /// Publisherを引数を二つ取るイベントから作成する
        /// </summary>
        /// <param name="subscribe">イベントの購読を開始するためのデリゲート</param>
        /// <param name="unsubscribe">イベントの購読を解除するためのデリゲート</param>
        /// <returns>作成されたPublisher</returns>
        public static IPublisher<T, U, V, W> FromEvent(Action<Action<T, U, V, W>> subscribe, Action<Action<T, U, V, W>> unsubscribe) {
            return new EventPublisher<T, U, V, W>(subscribe, unsubscribe);
        }
    }
}