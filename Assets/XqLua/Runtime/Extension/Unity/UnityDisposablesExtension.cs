using System;
using System.Diagnostics;
using UnityEngine;
using XqLua.Unity.Hooks;

namespace XqLua.Unity.Extension {
    /// <summary>
    /// IDisposableをUnityのGameObjectに紐づけるための拡張メソッド
    /// </summary>
    public static class UnityDisposablesExtension {
        /// <summary>
        /// IDisposableをGameObjectに紐づけする拡張メソッド
        /// </summary>
        /// <param name="disposable">追加するIDisposable</param>
        /// <param name="gameObject">追加先のGameObject</param>
        /// <exception cref="ArgumentNullException">デバッグが有効な場合、追加先のGameObjectがnullのときにスローされます</exception>
        public static T AddTo<T>(this T disposable, GameObject gameObject) where T : IDisposable {
#if XQLUA_DEBUG
            if (!gameObject) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でOnClickPublisherを呼ぼうとしたとき、GameObjectがnullでした\nSerializeFieldなどのアサインを忘れていませんか？");
            }
#endif
            OnDestroyHook trigger = default;
            if (!gameObject.TryGetComponent(out trigger)) {
                trigger = gameObject.AddComponent<OnDestroyHook>().Initialize();
            }
            trigger.Add(disposable);
            return disposable;
        }

        /// <summary>
        /// IDisposableをGameObjectに紐づけする拡張メソッド
        /// </summary>
        /// <param name="disposable">追加するIDisposable</param>
        /// <param name="monoBehaviour">追加先のMonoBehaviourを継承したスクリプト</param>
        /// <exception cref="ArgumentNullException">デバッグが有効な場合、追加先のGameObjectがnullのときにスローされます</exception>
        public static T AddTo<T, U>(this T disposable, U monoBehaviour) where T : IDisposable where U : MonoBehaviour {
#if XQLUA_DEBUG
            if (!monoBehaviour) {
                string caller = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
                throw new ArgumentNullException($"{caller}でOnClickPublisherを呼ぼうとしたとき、GameObjectがnullでした\nSerializeFieldなどのアサインを忘れていませんか？");
            }
#endif
            OnDestroyHook trigger = default;
            if (!monoBehaviour.TryGetComponent(out trigger)) {
                trigger = monoBehaviour.gameObject.AddComponent<OnDestroyHook>().Initialize();
            }
            trigger.Add(disposable);
            return disposable;
        }
    }
}
