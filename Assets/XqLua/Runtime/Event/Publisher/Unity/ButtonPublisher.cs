using System;
using System.Diagnostics;
using UnityEngine.Events;
using UnityEngine.UI;

namespace XqLua.Unity {
    /// <summary>
    /// ButtonのonClickイベントを購読するPublisher
    /// </summary>
    public sealed class ButtonPublisher : IPublisher<Empty> {

        private Button _button = default;

        internal ButtonPublisher(Button button) {
            _button = button;
        }

        //NOTE: 本家Observable.FromEventではDisposableが実装されていないので
        //      ここでも実装せずで

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

            Action<UnityAction> subscription = (action) => _button.onClick.AddListener(action);
            Action<UnityAction> unsubscription = (action) => _button.onClick.RemoveListener(action);

            return new ButtonSubscription(subscriber, subscription, unsubscription);
        }
    }

    /// <summary>
    /// ButtonのonClickイベントを購読するPublisher
    /// Buttonは普通発火しても引数はないが、ButtonPublisher<T>は任意の値を購読者に渡すことができる
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ButtonPublisher<T> : IPublisher<T> {

        private Button _button = default;
        private T _value = default;

        internal ButtonPublisher(Button button, T value) {
            _button = button;
            _value = value;
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

            Action<UnityAction> subscription = (action) => _button.onClick.AddListener(action);
            Action<UnityAction> unsubscription = (action) => _button.onClick.RemoveListener(action);

            return new ButtonSubscription<T>(subscriber, subscription, unsubscription, _value);
        }
    }
}
