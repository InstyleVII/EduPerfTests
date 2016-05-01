using System.Diagnostics;
using System;
using System.Runtime.InteropServices;

namespace EduPerfTests
{
    class BrowserMemory
    {
        private readonly Process[] _processes;
        private string _processName;
        public BrowserMemory(string processName)
        {
            _processName = processName;
            _processes = Process.GetProcessesByName(_processName);
        }

        public long PrivateMemorySize64 => _processes[0].PrivateMemorySize64;
        public long WorkingSet64 => _processes[0].WorkingSet64;
        public long PeakVirtualMemorySize64 => _processes[0].PeakVirtualMemorySize64;
        public long PeakPagedMemorySize64 => _processes[0].PeakPagedMemorySize64;
        public long PagedSystemMemorySize64 => _processes[0].PagedSystemMemorySize64;
        public long PagedMemorySize64 => _processes[0].PagedMemorySize64;
        public long NonpagedSystemMemorySize64 => _processes[0].NonpagedSystemMemorySize64;
        public long CurrentPrivateWorkingSet => GetCurrentMemoryUsage("Working Set - Private");
        private long GetCurrentMemoryUsage(string perfCounter)
        {
            long currentMemoryUsage;
            using (var procPerfCounter = new PerformanceCounter("Process", perfCounter, _processName))
            {
                currentMemoryUsage = procPerfCounter.RawValue;
            }
            return currentMemoryUsage;
        }
    }
}