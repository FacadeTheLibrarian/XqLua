using System;
using System.Collections.Generic;

#if XQLUA_DEBUG
using System.Diagnostics;
#endif

namespace XqLua {
    public static partial class OperatorExtension {

        /// <summary>
        /// 指定されたpublisherの値が変化したときに通知するOperatorを追加
        /// </summary>
        /// <typeparam name="T">Publisherの型</typeparam>
        /// <param name="publisher">対象のPublisher</param>
        /// <returns>値が変化したときにイベントを発行するOperator</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な時、publisherがnullの場合にスローされます</exception>
        public static BaseOperator<T> OnValueChanged<T>(this IPublisher<T> publisher) {
#if XQLUA_DEBUG
            if (publisher == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でOnValueChangedを付けようとしたpublisherがnullです\n{caller}にあるpublisherのnewを忘れていませんか？");
            }
#endif
            return new OnValueChanged<T>(null, publisher);
        }
        /// <summary>
        /// 指定されたpublisherの値が変化したときに通知するOperatorを追加
        /// </summary>
        /// <typeparam name="T">Publisherの型</typeparam>
        /// <param name="publisher">対象のPublisher</param>
        /// <returns>値が変化したときにイベントを発行するOperator</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な時、publisherがnullの場合にスローされます</exception>
        public static BaseOperator<T> OnValueChanged<T>(this BaseOperator<T> previousOperator) {
#if XQLUA_DEBUG
            if (previousOperator == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でOnValueChangedを付けようとしたpublisherがnullです\n{caller}にあるpublisherのnewを忘れていませんか？");
            }
#endif
            return new OnValueChanged<T>(previousOperator, previousOperator.Publisher);
        }
    }
    internal sealed class OnValueChanged<T> : BaseOperator<T> {
        private T _previousValue = default(T);

        internal OnValueChanged(BaseOperator<T> previous, IPublisher<T> sourcePublisher) : base(previous, sourcePublisher) { }
        protected override void OnDispose() {
            _previousValue = default(T);
        }

        internal override bool IsConditionMet(T value) {
            if (_previous == null) {
                return IsValueChanged(value);
            }
            else {
                bool isPreviousPassed = _previous.IsConditionMet(value);
                if (isPreviousPassed) {
                    bool isValueChanged = IsValueChanged(value);
                    return isValueChanged;
                }
                else {
                    return false;
                }
            }
        }

        private bool IsValueChanged(T value) {
            bool isValueChanged = !EqualityComparer<T>.Default.Equals(_previousValue, value);
            _previousValue = value;
            return isValueChanged;
        }
    }
}
