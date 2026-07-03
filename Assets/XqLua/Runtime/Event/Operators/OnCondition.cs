using System;

#if XQLUA_DEBUG
using System.Diagnostics;
#endif

namespace XqLua {
    public static partial class OperatorExtension {

        /// <summary>
        /// 指定した条件がtrueのときのみ、イベントを発行するOperatorを追加
        /// </summary>
        /// <typeparam name="T">Publisherの型</typeparam>
        /// <param name="publisher">対象のPublisher</param>
        /// <param name="condition">条件関数</param>
        /// <returns>条件を満たすときにイベントを発行するOperator</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な時、publisherがnullの場合にスローされます</exception>
        public static BaseOperator<T> OnCondition<T>(this IPublisher<T> publisher, Func<bool> condition) {
#if XQLUA_DEBUG
            if (publisher == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でOnConditionを付けようとしたpublisherがnullです\n{caller}にあるpublisherのnewを忘れていませんか？");
            }
#endif
            return new OnCondition<T>(null, publisher, condition);
        }

        /// <summary>
        /// 指定した条件がtrueのときのみ、イベントを発行するOperatorを追加
        /// </summary>
        /// <typeparam name="T">Publisherの型</typeparam>
        /// <param name="publisher">対象のPublisher</param>
        /// <param name="condition">条件関数</param>
        /// <returns>条件を満たすときにイベントを発行するOperator</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な時、publisherがnullの場合にスローされます</exception>
        public static BaseOperator<T> OnCondition<T>(this BaseOperator<T> previousOperator, Func<bool> condition) {
#if XQLUA_DEBUG
            if (previousOperator == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でOnConditionを付けようとしたpublisherがnullです\n{caller}にあるpublisherのnewを忘れていませんか？");
            }
#endif
            return new OnCondition<T>(previousOperator, previousOperator.Publisher, condition);
        }
    }
    internal sealed class OnCondition<T> : BaseOperator<T> {
        private Func<bool> _condition = default;

        internal OnCondition(BaseOperator<T> previous, IPublisher<T> sourcePublisher, Func<bool> condition) : base(previous, sourcePublisher) {
            _condition = condition;
        }
        protected override void OnDispose() {
            _condition = null;
        }

        internal override bool IsConditionMet(T value) {
            if (_previous == null) {
                return _condition();
            }
            else {
                bool isPreviousPassed = _previous.IsConditionMet(value);
                if (isPreviousPassed) {
                    return _condition();
                }
                else {
                    return false;
                }
            }
        }
    }
}
