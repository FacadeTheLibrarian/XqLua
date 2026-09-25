namespace XqLua.Message {
    public interface IMessageTray<T> where T : class {
        public IPublisher<T> OnMessageArrived { get; }
        public bool TryReadMessage(out T message);
    }
}
