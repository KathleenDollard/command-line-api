using System.Collections.Generic;

namespace System.CommandLine.Help
{
    public abstract class CliHelpSection
    {
        protected CliHelpSection(CliHelpConfiguration helpConfiguration, 
                                 string header,
                                 bool emitHeaderOnEmptyBody = false)
        {
            HelpConfiguration = helpConfiguration;
            Header = header;
            EmitHeaderOnEmptyBody = emitHeaderOnEmptyBody;
        }

        protected CliHelpConfiguration HelpConfiguration { get; }
        protected string Header { get; }
        public bool EmitHeaderOnEmptyBody { get; }

        public int Indent => HelpConfiguration.Indent;
        public CliHelpSymbolOutput SymbolOutput => HelpConfiguration.SymbolOutput;
            
        public virtual IEnumerable<string>? GetOpening(HelpContext helpContext)
        => new string[]
            {
                Heading(Header)
            };

        public abstract IEnumerable<string>? GetBody(HelpContext helpContext);
        public virtual IEnumerable<string>? GetClosing(HelpContext helpContext) => null;

        public virtual string Heading(string? heading)
            => heading ?? string.Empty;

    }
}

