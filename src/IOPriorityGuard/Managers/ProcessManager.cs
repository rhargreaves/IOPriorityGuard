using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using IOPriorityGuard.Interfaces;
using IOPriorityGuard.Model;
using IOPriorityGuard.Native;

namespace IOPriorityGuard.Managers
{
    public class ProcessManager : IProcessManager
    {
        internal static SafeProcessHandle OpenProcess(Process proc, PROCESS_RIGHTS processRights)
        {
            var hProcess = Win32.OpenProcess((int) PROCESS_RIGHTS.PROCESS_ALL_ACCESS, false, proc.Id);
            int win32Error = Marshal.GetLastWin32Error();
            if (hProcess.IsInvalid)
            {
                throw new Win32Exception(win32Error, "Couldn't open the process.");
            }
            return hProcess;
        }

        public IList<Process> GetRunningProcesses(IEnumerable<string> processNames)
        {
            var processes = new List<Process>();
            foreach (var processName in processNames)
            {
                var friendlyName = Path.GetFileNameWithoutExtension(processName);
                processes.AddRange(Process.GetProcessesByName(friendlyName));
            }
            return processes;
        }

        public void SetProcessPriority(Process process, ProcessPriorityClass priorityClass)
        {
            process.PriorityClass = priorityClass;
        }

        public void SetDiskIoPriority(Process process, DiskIoPriorityClass ioPriorityLevel)
        {
            using (var hProcess = OpenProcess(process, PROCESS_RIGHTS.PROCESS_ALL_ACCESS))
            {
                var newIoPrioritylevel = (uint) ioPriorityLevel;
                var gcHandle = GCHandle.Alloc(newIoPrioritylevel, GCHandleType.Pinned);
                try
                {
                    int retVal = Win32.NtSetInformationProcess(hProcess.DangerousGetHandle(), PROCESS_INFORMATION_CLASS.ProcessIoPriority,
                        gcHandle.AddrOfPinnedObject(), sizeof (uint));
                    if (retVal != 0)
                        throw new Win32Exception(retVal);
                }
                finally
                {
                    gcHandle.Free();
                }
            }
        }

        public DiskIoPriorityClass GetDiskIoPrority(Process process)
        {
            using (var hProcess = OpenProcess(process, PROCESS_RIGHTS.PROCESS_ALL_ACCESS))
            {
                uint ioPriorityLevel = 0;
                var gcHandle = GCHandle.Alloc(ioPriorityLevel, GCHandleType.Pinned);
                try
                {
                    int sizeOfResult = 0;
                    int retVal = Win32.NtQueryInformationProcess(hProcess.DangerousGetHandle(), PROCESS_INFORMATION_CLASS.ProcessIoPriority,
                        gcHandle.AddrOfPinnedObject(), sizeof(uint), ref sizeOfResult);
                    if (retVal != 0)
                        throw new Win32Exception(retVal);
                    return (DiskIoPriorityClass) ioPriorityLevel;
                }
                finally
                {
                    gcHandle.Free();
                }
            }
        }
    }
}