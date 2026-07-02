using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEngine;
#endif

#if XQLUA_DEBUG
using System.Diagnostics;
#endif

namespace XqLua {
#if UNITY_EDITOR
    [System.Serializable]
#endif

    /// <summary>
    /// 監視する値の変更をIPublisherを通して通知するクラス
    /// </summary>
    /// <typeparam name="T">監視する値の型</typeparam>
    public sealed class ReactiveProperty<T> : IReactiveProperty<T>, IDisposable {
        /// <summary>
        /// このクラスが監視する値が変更された場合に発火する
        /// </summary>
        private event Action<T> OnValueChanged = delegate { };

        /// <summary>
        /// 現在の値を取得または設定する
        /// 設定する際は、値が変更された場合のみOnValueChangedイベントが発火する
        /// </summary>
        /// <exception cref="AccessViolationException">デバッグが有効な場合、すでにDisposeされたPublisherの値を取得または設定しようとしたときにスローされます</exception>
        /// <exception cref="InvalidOperationException">デバッグが有効な場合、再帰的な値の変更が検出されたときにスローされます</exception>
        public T Value {
            get {
#if XQLUA_DEBUG
                if (_isDisposed) {
                    string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                    throw new AccessViolationException($"すでにDisposeされたPublisherを{caller}が取得しようとしています");
                }
#endif
                return _value;
            }
            set {
#if XQLUA_DEBUG
                if (_isDisposed) {
                    string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                    throw new AccessViolationException($"すでにDisposeされたPublisherを{caller}が発火しようとしています");
                }
#endif
#if XQLUA_DEBUG
                _recursiveCounter++;
                if (_recursiveCounter > 10) {
                    _recursiveCounter--;
                    throw new InvalidOperationException($"再帰的な値の変更が検出されました。\nSubscribeしたReactivePropertyの中でValueやSetForceNotifyを使っていませんか？");
                }
#endif
                T previous = _value;
                _value = value;
                if (!EqualityComparer<T>.Default.Equals(previous, value)) {
                    OnValueChanged?.Invoke(value);
                }
#if XQLUA_DEBUG
                _recursiveCounter--;
#endif
            }
        }

#if UNITY_EDITOR
        [SerializeField]
#endif
        private T _value = default;

#if XQLUA_DEBUG
        private int _recursiveCounter = 0;
        private bool _isDisposed = false;
#endif

        /// <summary>
        /// 値の変更を監視するReactivePropertyを生成
        /// </summary>
        /// <param name="value">初期値</param>
        public ReactiveProperty(T value = default) {
            _value = value;
        }

        /// <summary>
        /// リソースの破棄
        /// </summary>
        public void Dispose() {
#if XQLUA_DEBUG
            _isDisposed = true;
#endif
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
            OnValueChanged += subscription.OnEventInvoked;
            subscription.SetUnsubscription(() => OnValueChanged -= subscription.OnEventInvoked);

            subscriber(_value);

            return subscription;
        }

        /// <summary>
        /// 値の変更
        /// 値が同じでも強制的にイベントを発火する
        /// </summary>
        /// <param name="next">セットしたい値</param>
        public void SetForceNotify(T next) {
#if XQLUA_DEBUG
            if (_isDisposed) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new AccessViolationException($"すでにDisposeされたPublisherを{caller}が発火しようとしています");
            }
#endif
            _value = next;
            OnValueChanged.Invoke(next);
        }

        /// <summary>
        /// 値の変更
        /// イベントを発火させずに値を変更する
        /// </summary>
        /// <param name="next">セットしたい値</param>
        public void SetWithoutNotify(T next) {
#if XQLUA_DEBUG
            if (_isDisposed) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new AccessViolationException($"すでにDisposeされたPublisherを{caller}が発火しようとしています");
            }
#endif
            _value = next;
        }
    }
}