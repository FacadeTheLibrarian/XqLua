using System;
using UnityEngine;
using System.Threading;

#if XQLUA_DEBUG
using System.Diagnostics;
#endif

namespace XqLua.Unity {
    public static partial class OperatorExtension {
        /// <summary>
        /// 指定した秒数の遅延を付与するOperatorを追加
        /// </summary>
        /// <typeparam name="T">Publisherの型</typeparam>
        /// <param name="publisher">対象のPublisher</param>
        /// <param name="delaySecond">遅延時間（秒）</param>
        /// <returns>遅延を追加したOperator</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な時、publisherがnullの場合にスローされます</exception>
        /// <exception cref="ArgumentOutOfRangeException">デバッグが有効な時、delaySecondが負の値の場合にスローされます</exception>
        public static BaseOperator<T> WithDelay<T>(this IPublisher<T> publisher, float delaySecond) {
#if XQLUA_DEBUG
            if (publisher == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でWithDelayを付けようとしたpublisherがnullです\n{caller}にあるpublisherのnewを忘れていませんか？");
            }
            if (delaySecond < 0.0f) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentOutOfRangeException($"{caller}でWithIntervalを付けようとしたintervalSecondが負の値です\nintervalSecondの値を確認してください");
            }
#endif
            Delay<T> conditionalOperator = new Delay<T>(null, publisher, delaySecond);
            return conditionalOperator;
        }

        /// <summary>
        /// 指定した秒数の遅延を付与するOperatorを追加
        /// </summary>
        /// <typeparam name="T">Publisherの型</typeparam>
        /// <param name="publisher">対象のPublisher</param>
        /// <param name="delaySecond">遅延時間（秒）</param>
        /// <returns>遅延を追加したOperator</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な時、publisherがnullの場合にスローされます</exception>
        /// <exception cref="ArgumentOutOfRangeException">デバッグが有効な時、delaySecondが負の値の場合にスローされます</exception>
        public static BaseOperator<T> WithDelay<T>(this BaseOperator<T> previousOperator, float delaySecond) {
#if XQLUA_DEBUG
            if (previousOperator == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でWithDelayを付けようとしたpublisherがnullです\n{caller}にあるpublisherのnewを忘れていませんか？");
            }
            if (delaySecond < 0.0f) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentOutOfRangeException($"{caller}でWithIntervalを付けようとしたintervalSecondが負の値です\nintervalSecondの値を確認してください");
            }
#endif
            Delay<T> conditionalOperator = new Delay<T>(previousOperator, previousOperator.Publisher, delaySecond);
            return conditionalOperator;
        }
    }
    internal sealed class Delay<T> : BaseOperator<T> {
        private Publisher<T> _delayedPublisher = default;
        private CancellationTokenSource _tokenSource = default;
        private IDisposableSubscription _sourceSubscription = default;
        private float _delay = 0.0f;

        internal Delay(BaseOperator<T> previous, IPublisher<T> sourcePublisher, float delaySecond) : base(previous, null) {
            _delayedPublisher = new Publisher<T>();
            _publisher = _delayedPublisher;

            _sourceSubscription = sourcePublisher.Subscribe(OnUpstreamPublisherInvoked);

            _tokenSource = new CancellationTokenSource();
            _delay = delaySecond;
        }
        protected override void OnDispose() {
            _tokenSource.Cancel();
            _tokenSource.Dispose();
            _delayedPublisher.Dispose();
            _sourceSubscription.Dispose();
        }

        private void OnUpstreamPublisherInvoked(T value) {
            if (_previous != null) {
                if (!_previous.IsConditionMet(value)) {
                    return;
                }
            }
            DelaySecond(value, _delay, _tokenSource.Token).GetAwaiter();
        }

        internal override bool IsConditionMet(T value) {
            return true;
        }

        private async Awaitable DelaySecond(T value, float delay, CancellationToken token) {
            try {
                await Awaitable.WaitForSecondsAsync(delay, token);
            }
            catch (OperationCanceledException) {
                return;
            }
            catch {
                throw;
            }
            _delayedPublisher.Invoke(value);
        }
    }
}