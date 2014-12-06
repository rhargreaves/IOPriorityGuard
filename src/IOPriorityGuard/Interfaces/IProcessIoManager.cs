using System.Diagnostics;
using IOPriorityGuard.Native;

namespace IOPriorityGuard.Interfaces
{
    public interface IProcessIoManager
    {
        void SetIoPriority(Process process, IoPriority ioPriorityLevel);
        IoPriority GetIoPrority(Process process);
    }
}