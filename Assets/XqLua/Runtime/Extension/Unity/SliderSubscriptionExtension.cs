using System;
using UnityEngine.UI;

#if XQLUA_DEBUG
using System.Diagnostics;
#endif

namespace XqLua.Unity.Extension {
    /// <summary>
    /// UnityEngine.UI.SliderのonValueChangedイベントを購読するための拡張メソッド
    /// </summary>
    public static class SliderSubscriptionExtension {
        /// <summary>

        /// </summary>
        /// <param name="slider">購読するSlider</param>
        /// <param name="subscriber">呼び出してほしいメソッド</param>
        /// <returns>購読の解除を担当するSubscription</returns>
        public static IPublisher<float> OnValueChangedAsPublisher(this Slider slider) {
#if XQLUA_DEBUG
            if (slider == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でOnClickPublisherを呼ぼうとしたとき、Buttonがnullでした\nSerializeFieldなどでButtonのアサインを忘れていませんか？");
            }
#endif
            SliderPublisher publisher = new SliderPublisher(slider);
            return publisher;
        }
    }
}