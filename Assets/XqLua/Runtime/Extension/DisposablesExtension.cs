using System;

#if XQLUA_DEBUG
using System.Diagnostics;
#endif

namespace XqLua.Extension {

    /// <summary>
    /// Disposablesを使いやすくするための拡張メソッド
    /// </summary>
    public static class DisposablesExtension {

        /// <summary>
        /// IDisposableSubscriptionをDisposablesに追加する拡張メソッド
        /// </summary>
        /// <param name="disposableAction">追加するIDisposableSubscription</param>
        /// <param name="disposables">追加先のDisposables</param>
        /// <exception cref="ArgumentNullException">デバッグが有効な場合、追加先のDisposablesがnullのときにスローされます</exception>
        public static void AddTo(this IDisposableSubscription disposableAction, Disposables disposables) {
#if XQLUA_DEBUG
            if (disposables == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}で{disposableAction.GetType().Name}をDisposablesに追加しようとしたとき、そのDisposablesがnullでした\n{caller}にあるDisposablesのnewを忘れていませんか？");
            }
#endif
            disposables.Add(disposableAction);
        }

        /// <summary>
        /// IDisposableを実装しているクラスをDisposablesに追加する拡張メソッド
        /// </summary>
        /// <param name="disposableClass">追加するIDisposableが実装されたクラス</param>
        /// <param name="disposables">追加先のDisposables</param>
        /// <exception cref="ArgumentNullException">デバッグが有効な場合、追加先のDisposablesがnullのときにスローされます</exception>
        public static T AddTo<T>(this T disposableClass, Disposables disposables) where T : IDisposable {
#if XQLUA_DEBUG
            if (disposables == null) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}で{disposableClass.GetType().Name}をDisposablesに追加しようとしたとき、そのDisposablesがnullでした\n{caller}にあるDisposablesのnewを忘れていませんか？");
            }
#endif
            disposables.Add(disposableClass);
            return disposableClass;
        }
    }
}