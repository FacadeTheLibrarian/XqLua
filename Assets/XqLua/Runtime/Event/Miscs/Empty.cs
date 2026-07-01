namespace XqLua {
    /// <summary>
    /// Publisher<T>のTがvoidであるとき、つまりeventでいえばActionを指定したときに使う型
    /// UniRx/R3ではUnitとなる
    /// Unitだと多少直感的でないため、Emptyを用意した
    /// </summary>
    public struct Empty {
        public static Empty Default = default;
    }
}