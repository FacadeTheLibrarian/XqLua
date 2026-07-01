using System;
using UnityEngine.UI;

#if XQLUA_DEBUG
using System.Diagnostics;
#endif

namespace XqLua.Unity.Extension {
    /// <summary>
    /// UnityEngine.UI.ButtonのonClickイベントを購読するための拡張メソッド
    /// </summary>
    public static class ButtonSubscriptionExtension {
        /// <summary>

        /// </summary>
        /// <param name="button">購読するButton</param>
        /// <param name="subscriber">呼び出してほしいメソッド</param>
        /// <returns>購読の解除を担当するSubscription</returns>
        public static IPublisher<Empty> OnClickAsPublisher(this Button button) {
#if XQLUA_DEBUG
            if (button == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でOnClickPublisherを呼ぼうとしたとき、Buttonがnullでした\nSerializeFieldなどでButtonのアサインを忘れていませんか？");
            }
#endif
            ButtonPublisher publisher = new ButtonPublisher(button);
            return publisher;
        }
        /// <summary>

        /// </summary>
        /// <typeparam name="T">引数の型</typeparam>
        /// <param name="button">購読するButton</param>
        /// <param name="subscriber">呼び出してほしいメソッド</param>
        /// <param name="value">subscriberに渡す値</param>
        /// <returns>購読の解除を担当するSubscription</returns>
        public static IPublisher<T> OnClickAsValuedPublisher<T>(this Button button, T value) {
#if XQLUA_DEBUG
            if (button == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でOnClickValuedPublisherを呼ぼうとしたとき、Buttonがnullでした\nSerializeFieldなどでButtonのアサインを忘れていませんか？");
            }
            if (value is Empty) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentException($"{caller}でOnClickValuedPublisherを呼ぼうとしたとき、valueがEmptyでした\nボタンが押されたとき、値が欲しい場合にOnClickValuedPublisherを使ってください\n値が必要ない場合(Emptyを使うとき)はOnClickAsPublisherを使ってください");
            }
#endif
            ButtonPublisher<T> publisher = new ButtonPublisher<T>(button, value);
            return publisher;
        }
    }
}