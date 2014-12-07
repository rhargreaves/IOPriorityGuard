using System.Collections.Generic;
using System.Diagnostics;
using CommandLine;
using CommandLine.Text;
using IOPriorityGuard.Model;

namespace IOPriorityGuard
{
    public class AppOptions
    {
        [OptionList('p', "process-names", Required = true, Separator=',', HelpText = "Comma seperated list of process names.")]
        public IList<string> ProcessNames { get; set; }

        [Option('d', "disk-io-priority-class", DefaultValue = DiskIoPriorityClass.Background,
            HelpText = "Disk I/O priority class (Background, Low or Normal).")]
        public DiskIoPriorityClass DiskIoPriority { get; set; }

        [Option('c', "process-priority-class", DefaultValue = ProcessPriorityClass.Idle,
         HelpText = "Base priority class (Idle, BelowNormal, Normal, AboveNormal, High, RealTime).")]
        public ProcessPriorityClass ProcessPriority { get; set; }

        [Option('m', "monitor", DefaultValue = false,
            HelpText = "Monitors for new processes and sets priority accordingly.")]
        public bool Monitor { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var helpText = HelpText.AutoBuild(this,
                current => HelpText.DefaultParsingErrorsHandler(this, current));
            return helpText;
        }
    }
}