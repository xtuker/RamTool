// ReSharper disable InconsistentNaming
namespace RamTool.Manager;

using System;
using System.Runtime.InteropServices;

using RamTool.Manager.WinApi;

internal partial class Imports
{
    [LibraryImport("advapi32.dll", EntryPoint = "LookupPrivilegeValueA", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool LookupPrivilegeValue(string? host, string name, ref long pluid);

    [LibraryImport("advapi32.dll", EntryPoint = "AdjustTokenPrivileges", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool AdjustTokenPrivileges(IntPtr htok, [MarshalAs(UnmanagedType.Bool)] bool disall, ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

    [LibraryImport("ntdll.dll", EntryPoint = "NtSetSystemInformation")]
    internal static partial uint NtSetSystemInformation(int InfoClass, IntPtr Info, int Length);

    [LibraryImport("psapi.dll", EntryPoint = "EmptyWorkingSet")]
    internal static partial int EmptyWorkingSet(IntPtr hwProc);

    [LibraryImport("advapi32.dll", EntryPoint = "OpenProcessToken", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

    [LibraryImport("advapi32.dll", EntryPoint = "GetTokenInformation", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetTokenInformation(IntPtr TokenHandle, TokenInformationClass TokenInformationClass, IntPtr TokenInformation, int TokenInformationLength, out int ReturnLength);

    [LibraryImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool CloseHandle(IntPtr hObject);
}