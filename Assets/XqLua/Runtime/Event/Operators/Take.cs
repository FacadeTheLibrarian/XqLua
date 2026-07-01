using System;

#if XQLUA_DEBUG
using System.Diagnostics;
#endif

namespace XqLua {
    public static partial class OperatorExtension {

        /// <summary>
        /// 指定した件数だけ値を流すOperatorを追加します
        /// </summary>
        /// <typeparam name="T">Publisherの型</typeparam>
        /// <param name="publisher">対象のPublisher</param>
        /// <param name="count">流す件数</param>
        /// <returns>指定した件数だけ値を流すOperator</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な時、publisherがnullの場合にスローされます</exception>
        /// <exception cref="ArgumentOutOfRangeException">デバッグが有効な時、countが負の値の場合にスローされます</exception>
        public static BaseOperator<T> Take<T>(this IPublisher<T> publisher, int count) {
#if XQLUA_DEBUG
            if (publisher == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でTakeを付けようとしたpublisherがnullです\n{caller}にあるpublisherのnewを忘れていませんか？");
            }
            if (count < 0) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentOutOfRangeException($"{caller}でTakeを付けようとしたcountが負の値です\ncountの値を確認してください");
            }
#endif
            return new Take<T>(null, publisher, count);
        }

        /// <summary>
        /// 指定した件数だけ値を流すOperatorを追加します
        /// </summary>
        /// <typeparam name="T">Publisherの型</typeparam>
        /// <param name="publisher">対象のPublisher</param>
        /// <param name="count">流す件数</param>
        /// <returns>指定した件数だけ値を流すOperator</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な時、publisherがnullの場合にスローされます</exception>
        /// <exception cref="ArgumentOutOfRangeException">デバッグが有効な時、countが負の値の場合にスローされます</exception>
        public static BaseOperator<T> Take<T>(this BaseOperator<T> previousOperator, int count) {
#if XQLUA_DEBUG
            if (previousOperator == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でTakeを付けようとしたpublisherがnullです\n{caller}にあるpublisherのnewを忘れていませんか？");
            }
            if (count < 0) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentOutOfRangeException($"{caller}でSkipを付けようとしたcountが負の値です\ncountの値を確認してください");
            }
#endif
            return new Take<T>(previousOperator, previousOperator.Publisher, count);
        }
    }
    internal sealed class Take<T> : BaseOperator<T> {
        private int _count = 0;

        internal Take(BaseOperator<T> previous, IPublisher<T> sourcePublisher, int count) : base(previous, sourcePublisher) {
            _count = count;
        }
        protected override void OnDispose() {

        }

        internal override bool IsConditionMet(T value) {
            bool isPreviousConditionMet = true;

            if (_previous != null) {
                isPreviousConditionMet = _previous.IsConditionMet(value);
            }

            if (!isPreviousConditionMet) {
                return false;
            }

            if (_count <= 0) {
                return false;
            }

            _count--;
            return isPreviousConditionMet;
        }
    }
}