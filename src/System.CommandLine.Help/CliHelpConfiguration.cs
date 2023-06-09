using System.Collections;
using System.Collections.Generic;
using System.CommandLine.CliOutput;
using System.Linq;

namespace System.CommandLine.Help
{
    // Try to collapse this with CliDefaultHelpConfiguration
    public class CliHelpConfiguration : CliOutputConfiguration
    {
        public CliHelpConfiguration(CliConfiguration cliConfiguration, bool skipDefaultSections = false)
        {
            CliConfiguration = cliConfiguration;
            GetSymbolInspector = helpContext => new CliSymbolInspector();

        }

        public CliConfiguration CliConfiguration { get; }

        public CliFormatter? Formatter { get;set; }

        public Func<HelpContext, CliSymbolInspector> GetSymbolInspector { get; set; }

    }
}
