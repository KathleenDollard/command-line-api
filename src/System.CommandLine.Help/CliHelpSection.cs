using System.Collections.Generic;
using System.CommandLine.Help.Formatting;

namespace System.CommandLine.Help
{
    public abstract class CliHelpSection { }

    public abstract class CliHelpSection<T> : CliHelpSection
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
            
        public virtual IEnumerable<string>? GetOpening(HelpContext helpContext)
        => new string[]
            {
                Heading(Header)
            };

        public virtual IEnumerable<string>? GetBody(HelpContext helpContext) => null;
        public virtual Table<T>? GetBodyTable(HelpContext helpContext) => null;
        public virtual IEnumerable<string>? GetClosing(HelpContext helpContext) => null;

        public virtual string Heading(string? heading)
            => heading ?? string.Empty;

    }
}

