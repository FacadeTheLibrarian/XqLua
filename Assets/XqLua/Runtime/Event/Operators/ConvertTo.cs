using System;

#if XQLUA_DEBUG
using System.Diagnostics;
#endif

namespace XqLua {
    public static partial class OperatorExtension {
        /// <summary>
        /// 指定した型に変換するOperatorを追加
        /// </summary>
        /// <typeparam name="TPrevious">変換前の型</typeparam>
        /// <typeparam name="TNext">変換後の型</typeparam>
        /// <param name="publisher">対象のPublisher</param>
        /// <param name="converter">変換関数</param>
        /// <returns>変換後の型のOperator</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な時、publisherがnullの場合にスローされます</exception>
        public static BaseOperator<TNext> ConvertTo<TPrevious, TNext>(this IPublisher<TPrevious> publisher, Func<TPrevious, TNext> converter) {
#if XQLUA_DEBUG
            if (publisher == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でConvertToを付けようとしたpublisherがnullです\n{caller}にあるpublisherのnewを忘れていませんか？");
            }
#endif
            return new ConvertTo<TPrevious, TNext>(null, publisher, converter);
        }

        /// <summary>
        /// 指定した型に変換するOperatorを追加
        /// </summary>
        /// <typeparam name="TPrevious">変換前の型</typeparam>
        /// <typeparam name="TNext">変換後の型</typeparam>
        /// <param name="publisher">対象のPublisher</param>
        /// <param name="converter">変換関数</param>
        /// <returns>変換後の型のOperator</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な時、publisherがnullの場合にスローされます</exception>
        public static BaseOperator<TNext> ConvertTo<TPrevious, TNext>(this BaseOperator<TPrevious> previousOperator, Func<TPrevious, TNext> converter) {
#if XQLUA_DEBUG
            if (previousOperator == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でConvertToを付けようとしたpublisherがnullです\n{caller}にあるpublisherのnewを忘れていませんか？");
            }
#endif
            return new ConvertTo<TPrevious, TNext>(previousOperator, previousOperator.Publisher, converter);
        }
    }
    internal sealed class ConvertTo<TPrev, TNext> : BaseOperator<TNext> {
        private Publisher<TNext> _convertedPublisher = default;
        private BaseOperator<TPrev> _previousOperator = default;
        private Func<TPrev, TNext> _converter = default;

        private IDisposableSubscription _sourceSubscription = default;

        internal ConvertTo(BaseOperator<TPrev> previous, IPublisher<TPrev> sourcePublisher, Func<TPrev, TNext> converter) : base(null, null) {
            _convertedPublisher = new Publisher<TNext>();
            _publisher = _convertedPublisher;

            _previousOperator = previous ?? new PassThrough<TPrev>(null);
            _converter = converter;

            _sourceSubscription = sourcePublisher.Subscribe(OnUpstreamPublisherInvoked);
        }
        protected override void OnDispose() {
            _sourceSubscription.Dispose();
            _previousOperator.Dispose();
            _convertedPublisher.Dispose();
            _converter = null;
        }

        private void OnUpstreamPublisherInvoked(TPrev value) {
            if (_previousOperator.IsConditionMet(value)) {
                _convertedPublisher.Invoke(_converter(value));
            }
        }

        internal override bool IsConditionMet(TNext value) {
            // NOTE: 値を変換した後、このOperatorは評価の先頭に来るので、常にtrueを返す
            return true;
        }
    }
}