using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using IOPriorityGuard.Interfaces;
using IOPriorityGuard.Native;

namespace IOPriorityGuard.Managers
{
    public class ProcessIoManager : IProcessIoManager
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

        public void SetIoPriority(Process process, IoPriority ioPriorityLevel)
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

        public IoPriority GetIoPrority(Process process)
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
                    return (IoPriority) ioPriorityLevel;
                }
                finally
                {
                    gcHandle.Free();
                }
            }
        }
    }
}