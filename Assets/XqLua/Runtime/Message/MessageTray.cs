using System;

namespace XqLua.Message {
    public sealed class MessageTray<T> : IMessageTray<T>, IDisposable where T : class {
        public IPublisher<T> OnMessageArrived => _onMessageArrived;
        private Publisher<T> _onMessageArrived = default;

        private T _onTray = null;

        public MessageTray() {
            _onMessageArrived = new Publisher<T>();
        }

        public void Dispose() {
            _onMessageArrived.Dispose();
        }

        public bool TryReadMessage(out T message) {
            if (_onTray != null) {
                message = _onTray;
                return true;
            }
            message = null;
            return false;
        }

        public void PutMessage(T message) {
            if(message == null) {
                throw new ArgumentNullException(nameof(message));
            }
            _onTray = message;
            _onMessageArrived.Invoke(message);
        }
    }
}