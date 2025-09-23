namespace RamTool.Manager.WinApi;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct SidAndAttributes
{
    public IntPtr Sid;
    public int Attributes;
}