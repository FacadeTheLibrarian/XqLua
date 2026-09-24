using System;
using UnityEngine;

namespace XqLua.Unity.Hooks {

    /// <summary>
    /// UnityのGameObjectが破棄されるときに、Disposablesを破棄するためのフック用クラス
    /// </summary>
    internal sealed class OnDestroyHook : MonoBehaviour {
        private bool _hadDestroyed = false;
        private Disposables _disposables = default;

        /// <summary>
        /// Disposablesを初期化する
        /// </summary>
        internal OnDestroyHook Initialize() {
            _disposables = new Disposables();
            return this;
        }

        /// <summary>
        /// DisposablesにIDisposableを追加する
        /// </summary>
        internal void Add(IDisposable disposable) {
            if (_hadDestroyed) {
                disposable.Dispose();
            }
            else {
                _disposables.Add(disposable);
            }
        }

        private void OnDestroy() {
            _hadDestroyed = true;
            _disposables.Dispose();
        }
    }
}
