namespace RamTool.Manager;

using System;

using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;

using RamTool.Manager.WinApi;

public class RamManager
{
    public event EventHandler OnEmptyWorkingSet = (sender, args) => { };
    public event EventHandler OnClearFileSystemCache = (sender, args) => { };
    public event EventHandler OnClearStandbyCache = (sender, args) => { };

    public void EmptyWorkingSet()
    {
        OnEmptyWorkingSet(this, EventArgs.Empty);

        var identity = WindowsIdentity.GetCurrent();
        var currentSid = identity.User;

        if (currentSid == null)
        {
            return;
        }

        foreach (var process in Process.GetProcesses().Where(p => FilterBySid(p, currentSid)))
        {
            try
            {
                Imports.EmptyWorkingSet(process.Handle);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{process.ProcessName}: {ex.Message}");
            }
        }
    }

    public void ClearFileSystemCache(bool clearStandbyCache)
    {
        try
        {
            if (SetIncreasePrivilege(Consts.SE_INCREASE_QUOTA_NAME))
            {
                OnClearFileSystemCache(this, EventArgs.Empty);

                using var gcHandle = CreateSystemCacheInformation();
                var result = Imports.NtSetSystemInformation(Consts.SystemFileCacheInformation, gcHandle.Handle, gcHandle.Size);
                if (result != Consts.ERROR_SUCCESS)
                {
                    throw new Exception("NtSetSystemInformation(SYSTEMCACHEINFORMATION) error: ",
                        new Win32Exception(Marshal.GetLastWin32Error()));
                }
            }

            if (clearStandbyCache && SetIncreasePrivilege(Consts.SE_PROFILE_SINGLE_PROCESS_NAME))
            {
                OnClearStandbyCache(this, EventArgs.Empty);

                using var gcHandle = GcHandleWrapper.Create(Consts.MemoryPurgeStandbyList);
                var result = Imports.NtSetSystemInformation(Consts.SystemMemoryListInformation, gcHandle.Handle, gcHandle.Size);
                if (result != Consts.ERROR_SUCCESS)
                {
                    throw new Exception("NtSetSystemInformation(SYSTEMMEMORYLISTINFORMATION) error: ",
                        new Win32Exception(Marshal.GetLastWin32Error()));
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.Write(ex.ToString());
        }
    }

    private static bool FilterBySid(Process process, SecurityIdentifier currentSid)
    {
        var processTokenHandle = IntPtr.Zero;

        try
        {
            if (!Imports.OpenProcessToken(process.Handle, Consts.TOKEN_QUERY, out processTokenHandle))
            {
                return false;
            }

            Imports.GetTokenInformation(processTokenHandle, TokenInformationClass.TokenUser, IntPtr.Zero, 0, out var tokenInfoLength);

            using var tokenInfo = GcHandleWrapper.CreateBuffer(tokenInfoLength);

            if (!Imports.GetTokenInformation(processTokenHandle, TokenInformationClass.TokenUser, tokenInfo.Handle, tokenInfo.Size, out _))
            {
                return false;
            }

            var tokenUser = Marshal.PtrToStructure<TokenUser>(tokenInfo.Handle);
            var sid = new SecurityIdentifier(tokenUser.User.Sid);

            return sid.Equals(currentSid);
        }
        catch
        {
            return false;
        }
        finally
        {
            if (processTokenHandle != IntPtr.Zero)
            {
                Imports.CloseHandle(processTokenHandle);
            }
        }
    }

    private static GcHandleWrapper CreateSystemCacheInformation()
    {
        return Environment.Is64BitOperatingSystem
            ? GcHandleWrapper.Create(new SystemCacheInformation64
            {
                MinimumWorkingSet = ulong.MaxValue,
                MaximumWorkingSet = ulong.MaxValue
            })
            : GcHandleWrapper.Create(new SystemCacheInformation86
            {
                MinimumWorkingSet = uint.MaxValue,
                MaximumWorkingSet = uint.MaxValue
            });
    }

    //Function to increase Privilege, returns boolean
    private static bool SetIncreasePrivilege(string privilegeName)
    {
        using var current = WindowsIdentity.GetCurrent(TokenAccessLevels.Query | TokenAccessLevels.AdjustPrivileges);
        var newst = new TokPriv1Luid
        {
            Count = 1,
            Luid = 0L,
            Attr = Consts.SE_PRIVILEGE_ENABLED
        };

        //Retrieves the LUID used on a specified system to locally represent the specified privilege name
        if (!Imports.LookupPrivilegeValue(null, privilegeName, ref newst.Luid))
        {
            throw new Exception("Error in LookupPrivilegeValue: ", new Win32Exception(Marshal.GetLastWin32Error()));
        }

        //Enables or disables privileges in a specified access token
        if (!Imports.AdjustTokenPrivileges(current.Token, false, ref newst, 0, IntPtr.Zero, IntPtr.Zero))
        {
            throw new Exception("Error in AdjustTokenPrivileges: ", new Win32Exception(Marshal.GetLastWin32Error()));
        }

        return true;
    }
}