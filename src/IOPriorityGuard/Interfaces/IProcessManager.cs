using System.Collections.Generic;
using System.Diagnostics;
using IOPriorityGuard.Model;

namespace IOPriorityGuard.Interfaces
{
    public interface IProcessManager
    {
        IList<Process> GetRunningProcesses(IEnumerable<string> processNames);

        void SetProcessPriority(Process process, ProcessPriorityClass priorityClass);

        void SetDiskIoPriority(Process process, DiskIoPriorityClass ioPriorityLevel);

        DiskIoPriorityClass GetDiskIoPrority(Process process);
    }
}