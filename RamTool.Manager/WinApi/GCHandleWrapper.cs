namespace RamTool.Manager.WinApi
{
    using System;
    using System.Runtime.InteropServices;

    internal class GCHandleWrapper : IDisposable
    {
        public GCHandle Handle { get; private set; }

        public int Size { get; private set; }

        private GCHandleWrapper()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Handle.Free();
        }

        public static GCHandleWrapper Create<T>(T obj)
        {
            return new GCHandleWrapper
            {
                Size = Marshal.SizeOf(obj),
                Handle = GCHandle.Alloc(obj, GCHandleType.Pinned)
            };
        }
    }
}