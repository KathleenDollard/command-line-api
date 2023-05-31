using System.Collections.Generic;
using System.CommandLine.Help.Formatting;

namespace System.CommandLine.Help
{
    //public abstract class CliHelpSection 
    //{ 
    //}

    public abstract class CliHelpSection 
    {

        protected CliHelpSection(CliHelpConfiguration helpConfiguration, 
                                 CliSymbolInspector symbolInspector,
                                 CliFormatter formatter,
                                 string header,
                                 bool emitHeaderOnEmptyBody = false)
        {
            HelpConfiguration = helpConfiguration;
            SymbolInspector = symbolInspector;
            Formatter = formatter;
            Header = header;
            EmitHeaderOnEmptyBody = emitHeaderOnEmptyBody;
        }

        protected CliHelpConfiguration HelpConfiguration { get; }
        public CliSymbolInspector SymbolInspector { get; }
        public CliFormatter Formatter { get; }
        protected string Header { get; }
        public bool EmitHeaderOnEmptyBody { get; }

            
        public virtual IEnumerable<string>? GetOpening(HelpContext helpContext)
        => new string[]
            {
                Heading(Header)
            };

        public virtual IEnumerable<string>? GetBody(HelpContext helpContext) => null;
 
        public virtual IEnumerable<string>? GetClosing(HelpContext helpContext) => null;

        public virtual string Heading(string? heading)
            => heading ?? string.Empty;

    }
}

