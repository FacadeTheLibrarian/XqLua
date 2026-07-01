using System;
using System.Collections.Generic;

namespace XqLua {
    /// <summary>
    /// 複数のDisposableをまとめて破棄するためのクラス
    /// </summary>
    public sealed class Disposables : IDisposable {
        private List<IDisposable> _subscriptions = default;
        private bool _hasDisposed = false;

        /// <summary>
        /// Disposableをまとめて管理、破棄するためのDisposablesを生成
        /// </summary>
        public Disposables() {
            _subscriptions = new List<IDisposable>();
        }

        /// <summary>
        /// DisposablesにDisposableを追加する
        /// </summary>
        /// <param name="subscription">追加するDisposable</param>
        public void Add(IDisposable subscription) {
            if (_hasDisposed) {
                subscription.Dispose();
                return;
            }
            _subscriptions.Add(subscription);
        }

        /// <summary>
        /// Disposablesに追加された全てのDisposableを破棄する
        /// </summary>
        public void Dispose() {
            if (_hasDisposed) {
                return;
            }
            _hasDisposed = true;
            foreach (IDisposable disposable in _subscriptions) {
                disposable.Dispose();
            }
            _subscriptions.Clear();
        }
    }
}