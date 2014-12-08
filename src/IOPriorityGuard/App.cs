using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using IOPriorityGuard.Interfaces;
using log4net;

namespace IOPriorityGuard
{
    public class App
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly AppOptions _options;
        private readonly IProcessManager _processManager;

        public App(AppOptions options, IProcessManager processManager)
        {
            _options = options;
            _processManager = processManager;
        }

        public void Run()
        {
            IList<Process> runningProcesses = _processManager.GetRunningProcesses(_options.ProcessNames);
            _log.InfoFormat("Number of processes found matching names {0} = {1}",
                string.Join(", ", _options.ProcessNames), runningProcesses.Count);
            foreach (Process process in runningProcesses)
            {
                SetPriorities(process);
            }

            if (_options.Monitor)
            {
                _log.Info("Monitoring...");
                string query = "SELECT * FROM Win32_ProcessStartTrace WHERE "
                                     +
                                     string.Join(" OR ",
                                         _options.ProcessNames.Select(n => string.Format("ProcessName = \"{0}\"", n)));
                _log.DebugFormat("WQL query is [{0}]", query);
                var watcher = new ManagementEventWatcher(new WqlEventQuery(query));
                watcher.EventArrived += ProcessStartEvent;
                watcher.Start();
                Thread.Sleep(Timeout.Infinite);
            }
        }

        private void ProcessStartEvent(object sender, EventArrivedEventArgs e)
        {
            int processId = int.Parse(e.NewEvent.Properties["ProcessId"].Value.ToString());
            Task.Run(() => SetPriorities(processId));
        }

        private void SetPriorities(int processId)
        {
            Process process;
            try
            {
                process = Process.GetProcessById(processId);
                _log.DebugFormat("Process started: [{0}] (PID = {1})",
                    process.ProcessName, processId);
            }
            catch (ArgumentException)
            {
                _log.DebugFormat("Process started but ended too quickly to get information on it. (PID = {0})",
                    processId);
                return;
            }
            SetPriorities(process);
        }

        private void SetPriorities(Process process)
        {
            try
            {
                _log.InfoFormat("Process PID = {0}, Name = [{1}]",
                    process.Id, process.ProcessName);
                _log.InfoFormat("Setting base priority to {0}.", _options.ProcessPriority);
                _processManager.SetProcessPriority(process, _options.ProcessPriority);
                _log.InfoFormat("Setting disk I/O priority to {0}.",
                    _options.DiskIoPriority);
                _processManager.SetDiskIoPriority(process, _options.DiskIoPriority);
            }
            catch (Exception exp)
            {
                _log.Error("Error applying changes to process.", exp);
            }
        }
    }
}