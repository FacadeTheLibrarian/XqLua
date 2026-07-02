#if XQLUA_DEBUG
using System;
using System.Collections.Generic;
using UnityEditor;

namespace XqLua.Debug {
    public sealed class DisposableDebug {
        public static DisposableDebug Instance {
            get {
                if (_instance == null) {
                    _instance = new DisposableDebug();
                }
                return _instance;
            }
        }

        private static DisposableDebug _instance = default;

        private Dictionary<IDisposable, (string className, string caller)> _disposables = default;
        private DisposableDebug() {
            _disposables = new Dictionary<IDisposable, (string className, string caller)>();
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        public void AddDebug(IDisposable instance, string className, string caller) {
            _disposables.Add(instance, (className, caller));
        }

        public void DisposeDebug(IDisposable instance) {
            if (_disposables.ContainsKey(instance)) {
                _disposables.Remove(instance);
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state) {
            if (state != PlayModeStateChange.ExitingPlayMode) {
                return;
            }

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

            Queue<IDisposable> missedDisposables = new Queue<IDisposable>();
            foreach (IDisposable missedDisposable in _disposables.Keys) {
                (string className, string caller) information = _disposables[missedDisposable];
                UnityEngine.Debug.LogError($"{information.caller}で宣言された{information.className}がDisposeされていません！");
                missedDisposables.Enqueue(missedDisposable);
            }
            foreach(IDisposable missedDisposable in missedDisposables) {
                _disposables.Remove(missedDisposable);
                missedDisposable.Dispose();
            }
            missedDisposables.Clear();
            _disposables.Clear();

            missedDisposables = null;
            _disposables = null;
        }
    }
}
#endif