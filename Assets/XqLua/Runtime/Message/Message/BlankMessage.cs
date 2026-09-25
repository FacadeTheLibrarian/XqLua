namespace XqLua.Message {
    public sealed class BlankMessage {
        public static BlankMessage Default => DEFAULT;
        private static readonly BlankMessage DEFAULT = new BlankMessage();
        public BlankMessage() { }
    }
}
