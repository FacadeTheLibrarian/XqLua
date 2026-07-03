using System;

#if XQLUA_DEBUG
using System.Diagnostics;
#endif

namespace XqLua {
    public static partial class OperatorExtension {
        /// <summary>
        /// 指定した条件に一致する値が流れてきたときだけ購読者に通知するオペレーターを追加
        /// </summary>
        /// <typeparam name="T">Publisherの型</typeparam>
        /// <param name="publisher">対象のPublisher</param>
        /// <param name="filter">条件関数</param>
        /// <returns>条件を満たすときにイベントを発行するOperator</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な時、publisherがnullの場合にスローされます</exception>
        public static BaseOperator<T> OnValueIs<T>(this IPublisher<T> publisher, Func<T, bool> filter) {
#if XQLUA_DEBUG
            if (publisher == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でOnValueIsを付けようとしたpublisherがnullです\n{caller}にあるpublisherのnewを忘れていませんか？");
            }
#endif
            return new OnValueIs<T>(null, publisher, filter);
        }
        public static BaseOperator<T> OnValueIs<T>(this BaseOperator<T> previousOperator, Func<T, bool> filter) {
#if XQLUA_DEBUG
            if (previousOperator == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でOnValueIsを付けようとしたpublisherがnullです\n{caller}にあるpublisherのnewを忘れていませんか？");
            }
#endif
            return new OnValueIs<T>(previousOperator, previousOperator.Publisher, filter);
        }
    }

    /// <summary>
    /// 指定した条件に一致する値が流れてきたときだけ購読者に通知するオペレーターを追加
    /// </summary>
    /// <typeparam name="T">Publisherの型</typeparam>
    /// <param name="publisher">対象のPublisher</param>
    /// <param name="filter">条件関数</param>
    /// <returns>条件を満たすときにイベントを発行するOperator</returns>
    /// <exception cref="ArgumentNullException">デバッグが有効な時、publisherがnullの場合にスローされます</exception>
    internal sealed class OnValueIs<T> : BaseOperator<T> {
        private Func<T, bool> _condition = default;

        internal OnValueIs(BaseOperator<T> previous, IPublisher<T> sourcePublisher, Func<T, bool> condition) : base(previous, sourcePublisher) {
            _condition = condition;
        }
        protected override void OnDispose() {
            _condition = null;
        }

        internal override bool IsConditionMet(T value) {
            if (_previous == null) {
                return _condition(value);
            }
            else {
                bool isPreviousPassed = _previous.IsConditionMet(value);
                if (isPreviousPassed) {
                    return _condition(value);
                }
                else {
                    return false;
                }
            }
        }
    }
}
