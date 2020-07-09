namespace RamTool.Manager
{
    using System;
    using System.Collections.Generic;

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
            this.OnEmptyWorkingSet(this, EventArgs.Empty);
            var skipSet = new HashSet<string>()
            {
                "services",
                "csrss",
                "wininit",
                "csrss",
                "Registry",
                "Secure System",
                "smss",
                "MsMpEng",
                "avp",
                "Memory Compression",
                "System",
                "Idle"
            };

            foreach (var p in Process.GetProcesses().Where(x => !skipSet.Contains(x.ProcessName)))
            {
                try
                {
                    Imports.EmptyWorkingSet(p.Handle);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"{p.ProcessName}: {ex.Message}");
                }
            }
        }

        public void ClearFileSystemCache(bool clearStandbyCache)
        {
            try
            {
                if (this.SetIncreasePrivilege(Imports.SE_INCREASE_QUOTA_NAME))
                {
                    this.OnClearFileSystemCache(this, EventArgs.Empty);
                    using (var gcHandle = !Environment.Is64BitOperatingSystem
                        ? GCHandleWrapper.Create(new SystemCacheInformation86
                        {
                            MinimumWorkingSet = uint.MaxValue,
                            MaximumWorkingSet = uint.MaxValue
                        })
                        : GCHandleWrapper.Create(new SystemCacheInformation64
                        {
                            MinimumWorkingSet = ulong.MaxValue,
                            MaximumWorkingSet = ulong.MaxValue
                        }))
                    {
                        var result = Imports.NtSetSystemInformation(Imports.SystemFileCacheInformation, gcHandle.Handle.AddrOfPinnedObject(), gcHandle.Size);
                        if (result != Imports.ERROR_SUCCESS)
                        {
                            throw new Exception("NtSetSystemInformation(SYSTEMCACHEINFORMATION) error: ",
                                new Win32Exception(Marshal.GetLastWin32Error()));
                        }
                    }
                }

                if (clearStandbyCache && this.SetIncreasePrivilege(Imports.SE_PROFILE_SINGLE_PROCESS_NAME))
                {
                    this.OnClearStandbyCache(this, EventArgs.Empty);
                    using (var gcHandle = GCHandleWrapper.Create(Imports.MemoryPurgeStandbyList))
                    {
                        var result = Imports.NtSetSystemInformation(Imports.SystemMemoryListInformation, gcHandle.Handle.AddrOfPinnedObject(), gcHandle.Size);
                        if (result != Imports.ERROR_SUCCESS)
                        {
                            throw new Exception("NtSetSystemInformation(SYSTEMMEMORYLISTINFORMATION) error: ",
                                new Win32Exception(Marshal.GetLastWin32Error()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.Write(ex.ToString());
            }
        }

        //Function to increase Privilege, returns boolean
        private bool SetIncreasePrivilege(string privilegeName)
        {
            using (var current = WindowsIdentity.GetCurrent(TokenAccessLevels.Query | TokenAccessLevels.AdjustPrivileges))
            {
                var newst = new TokPriv1Luid
                {
                    Count = 1,
                    Luid = 0L,
                    Attr = Imports.SE_PRIVILEGE_ENABLED
                };

                //Retrieves the LUID used on a specified system to locally represent the specified privilege name
                if (!Imports.LookupPrivilegeValue(null, privilegeName, ref newst.Luid))
                {
                    throw new Exception("Error in LookupPrivilegeValue: ", new Win32Exception(Marshal.GetLastWin32Error()));
                }

                //Enables or disables privileges in a specified access token
                var num = Imports.AdjustTokenPrivileges(current.Token, false, ref newst, 0, IntPtr.Zero, IntPtr.Zero) ? 1 : 0;
                if (num == 0)
                {
                    throw new Exception("Error in AdjustTokenPrivileges: ", new Win32Exception(Marshal.GetLastWin32Error()));
                }

                return num != 0;
            }
        }
    }
}
