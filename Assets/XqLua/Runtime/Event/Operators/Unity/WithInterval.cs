using System;
using UnityEngine;

#if XQLUA_DEBUG
using System.Diagnostics;
#endif

namespace XqLua.Unity {
    public static partial class OperatorExtension {
        /// <summary>
        /// 指定した間隔で値を流すOperatorを追加
        /// </summary>
        /// <typeparam name="T">Publisherの型</typeparam>
        /// <param name="publisher">対象のPublisher</param>
        /// <param name="intervalSecond">間隔時間（秒）</param>
        /// <param name="timeMode">時間の基準となるモード　systemTimeはシステム時間基準、unityTimeはUnityの時間基準(Time.timeScaleの影響を受ける)</param>
        /// <returns>間隔を追加したOperator</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な時、publisherがnullの場合にスローされます</exception>
        /// <exception cref="ArgumentOutOfRangeException">デバッグが有効な時、intervalSecondが負の値の場合にスローされます</exception>
        public static BaseOperator<T> WithInterval<T>(this IPublisher<T> publisher, float intervalSecond, TimeMode.e_timeMode timeMode = TimeMode.e_timeMode.systemTime) {
#if XQLUA_DEBUG
            if (publisher == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でWithIntervalを付けようとしたpublisherがnullです\n{caller}にあるpublisherのnewを忘れていませんか？");
            }
            if (intervalSecond < 0.0f) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentOutOfRangeException($"{caller}でWithIntervalを付けようとしたintervalSecondが負の値です\nintervalSecondの値を確認してください");
            }
#endif
            if (timeMode == TimeMode.e_timeMode.unityTime) {
                return new WithUnityInterval<T>(null, publisher, intervalSecond);
            }
            else {
                return new WithSystemInterval<T>(null, publisher, intervalSecond);
            }
        }

        /// <summary>
        /// 指定した間隔で値を流すOperatorを追加
        /// </summary>
        /// <typeparam name="T">Publisherの型</typeparam>
        /// <param name="publisher">対象のPublisher</param>
        /// <param name="intervalSecond">間隔時間（秒）</param>
        /// <param name="timeMode">時間の基準となるモード　systemTimeはシステム時間基準、unityTimeはUnityの時間基準(Time.timeScaleの影響を受ける)</param>
        /// <returns>間隔を追加したOperator</returns>
        /// <exception cref="ArgumentNullException">デバッグが有効な時、publisherがnullの場合にスローされます</exception>
        /// <exception cref="ArgumentOutOfRangeException">デバッグが有効な時、intervalSecondが負の値の場合にスローされます</exception>
        public static BaseOperator<T> WithInterval<T>(this BaseOperator<T> previousOperator, float intervalSecond, TimeMode.e_timeMode timeMode = TimeMode.e_timeMode.systemTime) {
#if XQLUA_DEBUG
            if (previousOperator == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でWithIntervalを付けようとしたpublisherがnullです\n{caller}にあるpublisherのnewを忘れていませんか？");
            }
            if (intervalSecond < 0.0f) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentOutOfRangeException($"{caller}でWithIntervalを付けようとしたintervalSecondが負の値です\nintervalSecondの値を確認してください");
            }
#endif
            if (timeMode == TimeMode.e_timeMode.unityTime) {
                return new WithUnityInterval<T>(previousOperator, previousOperator.Publisher, intervalSecond);
            }
            else {
                return new WithSystemInterval<T>(previousOperator, previousOperator.Publisher, intervalSecond);
            }
        }
    }
    internal sealed class WithSystemInterval<T> : BaseOperator<T> {
        private DateTime _lastTime = DateTime.MinValue;
        private double _interval = default;

        internal WithSystemInterval(BaseOperator<T> previous, IPublisher<T> sourcePublisher, float intervalSecond) : base(previous, sourcePublisher) {
            _interval = (double)intervalSecond;
        }
        protected override void OnDispose() { }

        internal override bool IsConditionMet(T value) {
            if (_previous == null) {
                return HasIntervalPassed();
            }
            else {
                bool isPreviousPassed = _previous.IsConditionMet(value);
                if (isPreviousPassed) {
                    return HasIntervalPassed();
                }
                else {
                    return false;
                }
            }
        }

        private bool HasIntervalPassed() {
            DateTime now = DateTime.Now;
            TimeSpan difference = now - _lastTime;
            if (difference.TotalSeconds > _interval) {
                _lastTime = now;
                return true;
            }
            else {
                return false;
            }
        }
    }

    public class TimeMode {
        public enum e_timeMode : int {
            systemTime = 0,
            unityTime = 1,
        }
    }

    internal sealed class WithUnityInterval<T> : BaseOperator<T> {
        private float _lastTime = float.MinValue;
        private float _interval = default;

        internal WithUnityInterval(BaseOperator<T> previous, IPublisher<T> sourcePublisher, float intervalSecond) : base(previous, sourcePublisher) {
            _interval = intervalSecond;
        }
        protected override void OnDispose() { }

        internal override bool IsConditionMet(T value) {
            if (_previous == null) {
                return HasIntervalPassed();
            }
            else {
                bool isPreviousPassed = _previous.IsConditionMet(value);
                if (isPreviousPassed) {
                    return HasIntervalPassed();
                }
                else {
                    return false;
                }
            }
        }

        private bool HasIntervalPassed() {
            float now = Time.time;
            if (now - _lastTime > _interval) {
                _lastTime = now;
                return true;
            }
            else {
                return false;
            }
        }
    }
}
