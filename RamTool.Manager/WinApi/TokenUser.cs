namespace RamTool.Manager.WinApi;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct TokenUser
{
    public SidAndAttributes User;
}