using System.Collections.Generic;
using System.CommandLine.CliOutput;

namespace System.CommandLine.Help
{


    public abstract class CliHelpSection  : CliSection
    {

        protected CliHelpSection(CliHelpConfiguration helpConfiguration, 
                                 CliSymbolInspector symbolInspector,
                                 CliFormatter formatter,
                                 string header,
                                 bool emitHeaderOnEmptyBody = false)
            : base(formatter, header, emitHeaderOnEmptyBody)
        {
            HelpConfiguration = helpConfiguration;
            SymbolInspector = symbolInspector;
        }

        protected CliHelpConfiguration HelpConfiguration { get; }
        public CliSymbolInspector SymbolInspector { get; }


    }
}

