namespace RamTool.Manager.WinApi;

using System;
using System.Runtime.InteropServices;

internal class GcHandleWrapper : IDisposable
{
    public IntPtr Handle => _gcHandle.AddrOfPinnedObject();

    private GCHandle _gcHandle;

    public int Size { get; }

    private GcHandleWrapper(GCHandle handle, int size)
    {
        _gcHandle = handle;
        Size = size;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_gcHandle.IsAllocated)
            _gcHandle.Free();
    }

    public static GcHandleWrapper Create<T>(T obj)
    {
        return new GcHandleWrapper(GCHandle.Alloc(obj, GCHandleType.Pinned), Marshal.SizeOf<T>());
    }

    public static GcHandleWrapper CreateBuffer(int size)
    {
        return new GcHandleWrapper(GCHandle.Alloc(new byte[size], GCHandleType.Pinned), size);
    }
}