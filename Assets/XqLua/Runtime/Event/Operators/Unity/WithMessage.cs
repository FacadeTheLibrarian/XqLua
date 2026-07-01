using System;
using Debug = UnityEngine.Debug;
using System.Text.RegularExpressions;

#if XQLUA_DEBUG
using System.Diagnostics;
#endif

namespace XqLua.Unity {
    public static partial class OperatorExtension {

        /// <summary>
        /// Debug.Logで渡された文章を出力するOperatorを追加
        /// { } を付けると、Publisherの値を文字列に変換して出力される</summary>
        /// <typeparam name="T">Publisherの型</typeparam>
        /// <param name="publisher">対象のPublisher</param>
        /// <param name="message">出力するメッセージ</param>
        /// <param name="logType">ログの種類</param>
        /// <returns>メッセージを出力するOperator</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な時、publisherがnullの場合にスローされます</exception>
        public static BaseOperator<T> WithMessage<T>(this IPublisher<T> publisher, string message, UnityEngine.LogType logType = UnityEngine.LogType.Log) {
#if XQLUA_DEBUG
            if (publisher == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でWithMessageを付けようとしたpublisherがnullです\n{caller}にあるpublisherのnewを忘れていませんか？");
            }
#endif
            WithMessage<T> conditionalOperator = new WithMessage<T>(null, publisher, message, logType);
            return conditionalOperator;
        }

        /// <summary>
        /// Debug.Logで渡された文章を出力するOperatorを追加
        /// { } を付けると、Publisherの値を文字列に変換して出力される</summary>
        /// <typeparam name="T">Publisherの型</typeparam>
        /// <param name="publisher">対象のPublisher</param>
        /// <param name="message">出力するメッセージ</param>
        /// <param name="logType">ログの種類</param>
        /// <returns>メッセージを出力するOperator</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な時、publisherがnullの場合にスローされます</exception>
        public static BaseOperator<T> WithMessage<T>(this BaseOperator<T> previousOperator, string message, UnityEngine.LogType logType = UnityEngine.LogType.Log) {
#if XQLUA_DEBUG
            if (previousOperator == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でWithMessageを付けようとしたpublisherがnullです\n{caller}にあるpublisherのnewを忘れていませんか？");
            }
#endif
            WithMessage<T> conditionalOperator = new WithMessage<T>(previousOperator, previousOperator.Publisher, message, logType);
            return conditionalOperator;
        }
    }
    internal sealed class WithMessage<T> : BaseOperator<T> {
        private string _message = default;
        private UnityEngine.LogType _logType = default;
        internal WithMessage(BaseOperator<T> previous, IPublisher<T> sourcePublisher, string message, UnityEngine.LogType logType) : base(previous, sourcePublisher) {
            _message = message;
            _logType = logType;
        }
        protected override void OnDispose() {
            Debug.Log("WithMessageが正常にDisposeされました！");
        }

        internal override bool IsConditionMet(T value) {
            bool isPreviousConditionMet = true;

            if (_previous != null) {
                isPreviousConditionMet = _previous.IsConditionMet(value);
            }

            string formatted = Regex.Replace(_message, @"\{([^}]*)\}", value.ToString());
            switch (_logType) {
                case UnityEngine.LogType.Error:
                    Debug.LogError(formatted);
                    break;
                case UnityEngine.LogType.Warning:
                    Debug.LogWarning(formatted);
                    break;
                default:
                    Debug.Log(formatted);
                    break;
            }
            return isPreviousConditionMet;
        }
    }
}