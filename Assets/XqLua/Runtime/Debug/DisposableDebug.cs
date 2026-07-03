#if XQLUA_DEBUG && UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;

namespace XqLua.Debugger {
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
            if (state != PlayModeStateChange.EnteredEditMode) {
                return;
            }
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

            List<IDisposable> missedDisposables = new List<IDisposable>(_disposables.Keys);
            List<string> missedDisposablesInfo = new List<string>();
            foreach (IDisposable missedDisposable in missedDisposables) {
                (string className, string caller) information = _disposables[missedDisposable];
                missedDisposablesInfo.Add($"{information.caller}で宣言された{information.className}がDisposeされていません！");
                missedDisposable.Dispose();
            }
            _disposables = null;
            _instance = null;

            if(missedDisposablesInfo.Count > 0) {
                string messageHeader = "以下のIDisposableがDisposeされていませんでした！\nDispose忘れはメモリリークの原因となります。Disposeするようにプログラムを修正してください。\n";
                string message = messageHeader + string.Join("\n", missedDisposablesInfo);
                throw new InvalidProgramException(message);
            }
        }
    }
}
#endif