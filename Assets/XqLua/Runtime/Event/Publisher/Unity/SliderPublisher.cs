using System;
using System.Diagnostics;
using UnityEngine.Events;
using UnityEngine.UI;

namespace XqLua.Unity {
    /// <summary>
    /// SliderのonValueChangedイベントを購読するPublisher
    /// </summary>
    public sealed class SliderPublisher : IPublisher<float> {

        private Slider _slider = default;

        internal SliderPublisher(Slider slider) {
            _slider = slider;
        }

        //NOTE: 本家Observable.FromEventではDisposableが実装されていないので
        //      ここでも実装せずで

        /// <summary>
        /// 購読する
        /// </summary>
        /// <param name="subscriber">呼び出してほしいメソッド</param>
        /// <returns>購読の解除を担当するDisposableSubscription</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な場合、subscriberがnullの場合にスローされます</exception>
        public IDisposableSubscription Subscribe(Action<float> subscriber) {
#if XQLUA_DEBUG
            if (subscriber == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でSubscribeを呼ぼうとしたとき、subscriberがnullでした\n購読しようとしたメソッドが入ったクラスがnullだったり、Action<T>変数がnullではないですか？");
            }
#endif

            Action<UnityAction<float>> subscription = (action) => _slider.onValueChanged.AddListener(action);
            Action<UnityAction<float>> unsubscription = (action) => _slider.onValueChanged.RemoveListener(action);

            return new UnityEventSubscription<float>(subscriber, subscription, unsubscription);
        }
    }
}
