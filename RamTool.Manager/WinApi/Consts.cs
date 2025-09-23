// ReSharper disable InconsistentNaming
namespace RamTool.Manager.WinApi;

internal static class Consts
{
    internal const uint TOKEN_QUERY = 0x0008;
    internal const int ERROR_SUCCESS = 0;
    internal const int SE_PRIVILEGE_ENABLED = 2;
    internal const string SE_INCREASE_QUOTA_NAME = "SeIncreaseQuotaPrivilege";
    internal const string SE_PROFILE_SINGLE_PROCESS_NAME = "SeProfileSingleProcessPrivilege";
    internal const int SystemFileCacheInformation = 0x0015;
    internal const int SystemMemoryListInformation = 0x0050;
    internal const int MemoryPurgeStandbyList = 4;
}