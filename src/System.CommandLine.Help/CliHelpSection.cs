using System.Collections.Generic;

namespace System.CommandLine.Help
{
    public abstract class CliHelpSection
    {
        protected CliHelpSection(CliHelpConfiguration helpConfiguration, 
                                 HelpContext helpContext,
                                 string header,
                                 bool emitHeaderOnEmptyBody = false)
        {
            HelpConfiguration = helpConfiguration;
            HelpContext = helpContext;
            Header = header;
            EmitHeaderOnEmptyBody = emitHeaderOnEmptyBody;
        }

        protected CliHelpConfiguration HelpConfiguration { get; }
        protected string Header { get; }
        protected HelpContext HelpContext { get; }
        public bool EmitHeaderOnEmptyBody { get; }

        public int Indent => HelpConfiguration.HelpFormatting.Indent;
        public int MaxWidth  => HelpContext.MaxWidth;
        public CliHelpSymbolOutput SymbolOutput => HelpConfiguration.SymbolOutput;
            
        public virtual IEnumerable<string>? GetOpening(CliSymbol current)
        => new string[]
            {
                HelpConfiguration.HelpFormatting.Heading(Header)
            };

        public abstract IEnumerable<string>? GetBody(CliSymbol current);
        public abstract IEnumerable<string>? GetClosing(CliSymbol current);

    }
}

