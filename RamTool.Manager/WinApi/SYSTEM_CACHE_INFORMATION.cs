namespace RamTool.Manager.WinApi
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SystemCacheInformation86
    {
        public uint CurrentSize;
        public uint PeakSize;
        public uint PageFaultCount;
        public uint MinimumWorkingSet;
        public uint MaximumWorkingSet;
        public uint Unused1;
        public uint Unused2;
        public uint Unused3;
        public uint Unused4;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SystemCacheInformation64
    {
        public ulong CurrentSize;
        public ulong PeakSize;
        public ulong PageFaultCount;
        public ulong MinimumWorkingSet;
        public ulong MaximumWorkingSet;
        public ulong Unused1;
        public ulong Unused2;
        public ulong Unused3;
        public ulong Unused4;
    }
}