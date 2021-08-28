using System;

namespace TimeControl.Tests
{
    public class NullScope : IDisposable
    {
        public static NullScope Instance { get; }
        public void Dispose() { }
        private NullScope() { }
    }
}
