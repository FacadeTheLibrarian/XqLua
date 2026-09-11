#if XQLUA_DEBUG
using System.Diagnostics;
using XqLua.Debugger;
#endif

namespace XqLua {
    /// <summary>
    /// Operatorの購読を抽象化したクラス
    /// </summary>
    /// <typeparam name="T">Operatorの型</typeparam>
    internal sealed class OperatorSubscription<T> : IDisposableSubscription {
        private IDisposableSubscription _disposableSubscription = default;
        private BaseOperator<T> _disposableOperator = default;

        public OperatorSubscription(IDisposableSubscription disposableSubscription, BaseOperator<T> disposableOperator) {
            _disposableSubscription = disposableSubscription;
            _disposableOperator = disposableOperator;
#if XQLUA_DEBUG
            string caller = new StackFrame(2, false).GetMethod().DeclaringType.FullName;
            if (caller.Contains("Extension")) {
                caller = new StackFrame(3, false).GetMethod().DeclaringType.FullName;
            }
            DisposableDebugger.Instance.AddDebug(this, "Subscription", caller);
#endif
        }

        public void Dispose() {
            _disposableSubscription.Dispose();
            _disposableOperator.Dispose();
#if XQLUA_DEBUG
            DisposableDebugger.Instance.DisposeDebug(this);
#endif
        }
    }

    /// <summary>
    /// Operatorの購読を抽象化したクラス
    /// </summary>
    /// <typeparam name="T">Operatorの第一引数の型</typeparam>
    /// <typeparam name="U">Operatorの第二引数の型</typeparam>
    internal sealed class OperatorSubscription<T, U> : IDisposableSubscription {
        private IDisposableSubscription _disposableSubscription = default;
        private BaseOperator<T, U> _disposableOperator = default;

        public OperatorSubscription(IDisposableSubscription disposableSubscription, BaseOperator<T, U> disposableOperator) {
            _disposableSubscription = disposableSubscription;
            _disposableOperator = disposableOperator;
#if XQLUA_DEBUG
            string caller = new StackFrame(2, false).GetMethod().DeclaringType.FullName;
            if (caller.Contains("Extension")) {
                caller = new StackFrame(3, false).GetMethod().DeclaringType.FullName;
            }
            DisposableDebugger.Instance.AddDebug(this, "Subscription", caller);
#endif
        }

        public void Dispose() {
            _disposableSubscription.Dispose();
            _disposableOperator.Dispose();
#if XQLUA_DEBUG
            DisposableDebugger.Instance.DisposeDebug(this);
#endif
        }
    }
}