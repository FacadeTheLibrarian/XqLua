using System;
using Debug = UnityEngine.Debug;

#if XQLUA_DEBUG
using System.Diagnostics;
#endif

namespace XqLua.Unity {
    public static partial class OperatorExtension {
        /// <summary>
        /// Debug.Logで値を出力するOperatorを追加
        /// </summary>
        /// <typeparam name="T">Publisherの型</typeparam>
        /// <param name="publisher">対象のPublisher</param>
        /// <returns>DebugLogを追加したOperator</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な時、publisherがnullの場合にスローされます</exception>
        public static BaseOperator<T> WithDebugLog<T>(this IPublisher<T> publisher) {
#if XQLUA_DEBUG
            if (publisher == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でWithDebugLogを付けようとしたpublisherがnullです\n{caller}にあるpublisherのnewを忘れていませんか？");
            }
#endif
            WithDebugLog<T> conditionalOperator = new WithDebugLog<T>(null, publisher);
            return conditionalOperator;
        }

        /// <summary>
        /// Debug.Logで値を出力するOperatorを追加
        /// </summary>
        /// <typeparam name="T">Publisherの型</typeparam>
        /// <param name="publisher">対象のPublisher</param>
        /// <returns>DebugLogを追加したOperator</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な時、publisherがnullの場合にスローされます</exception>
        public static BaseOperator<T> WithDebugLog<T>(this BaseOperator<T> previousOperator) {
#if XQLUA_DEBUG
            if (previousOperator == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でWithDebugLogを付けようとしたpublisherがnullです\n{caller}にあるpublisherのnewを忘れていませんか？");
            }
#endif
            WithDebugLog<T> conditionalOperator = new WithDebugLog<T>(previousOperator, previousOperator.Publisher);
            return conditionalOperator;
        }
    }
    internal sealed class WithDebugLog<T> : BaseOperator<T> {
        internal WithDebugLog(BaseOperator<T> previous, IPublisher<T> sourcePublisher) : base(previous, sourcePublisher) {
        }
        protected override void OnDispose() {
            Debug.Log("WithDebugLogが正常にDisposeされました！");
        }

        internal override bool IsConditionMet(T value) {
            bool isPreviousConditionMet = true;

            if (_previous != null) {
                isPreviousConditionMet = _previous.IsConditionMet(value);
            }

            Debug.Log($"そのままの値: {value}, ToString: {value.ToString()}");
            return isPreviousConditionMet;
        }
    }
}