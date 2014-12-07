using System;
using System.Reflection;
using IOPriorityGuard.Managers;
using log4net;
using log4net.Config;

namespace IOPriorityGuard
{
    public class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var options = new AppOptions();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                _log.Debug("Options specified:");
                _log.DebugFormat("Monitor = {0}", options.Monitor);
                _log.DebugFormat("Process Names = {0}", string.Join(", ", options.ProcessNames));
                _log.DebugFormat("Disk Priority Class = {0}", options.DiskIoPriority);
                _log.DebugFormat("Base Priority Class = {0}", options.ProcessPriority);
                var app = new App(options, new ProcessManager());
                app.Run();
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;
            _log.Error("Fatal error", exception);
        }
    }
}