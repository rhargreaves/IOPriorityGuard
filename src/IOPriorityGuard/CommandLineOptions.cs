using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using IOPriorityGuard.Native;

namespace IOPriorityGuard
{
    public class CommandLineOptions
    {
        [Option('p', "process-names", Required = true, HelpText = "Comma seperated list of process names")]
        public IList<string> ProcessNames { get; set; }

        [Option('l', "priority-level", Required = true, HelpText = "Priority level (Background, Low or Normal)")]
        public IoPriority IoPriority { get; set; }

        [Option('m', "monitor", DefaultValue = false,
            HelpText = "Monitors for new processes and sets priority accordingly")]
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