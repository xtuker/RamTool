namespace RamTool.Manager
{
    using System;
    using System.Runtime.InteropServices;

    using RamTool.Manager.WinApi;

    internal class Imports
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

        [DllImport("ntdll.dll")]
        internal static extern uint NtSetSystemInformation(int InfoClass, IntPtr Info, int Length);

        [DllImport("psapi.dll")]
        internal static extern int EmptyWorkingSet(IntPtr hwProc);

        internal const int ERROR_SUCCESS = 0;
        internal const int SE_PRIVILEGE_ENABLED = 2;
        internal const string SE_INCREASE_QUOTA_NAME = "SeIncreaseQuotaPrivilege";
        internal const string SE_PROFILE_SINGLE_PROCESS_NAME = "SeProfileSingleProcessPrivilege";
        internal const int SystemFileCacheInformation = 0x0015;
        internal const int SystemMemoryListInformation = 0x0050;
        internal const int MemoryPurgeStandbyList = 4;
    }
}