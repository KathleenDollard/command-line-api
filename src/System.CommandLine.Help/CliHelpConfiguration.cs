using System.Collections.Generic;

namespace System.CommandLine.Help
{
    public class CliHelpConfiguration
    {
        public CliHelpConfiguration(CliHelpSymbolOutput? symbolOutput = null, int indent = 0)
        {
            SymbolOutput = symbolOutput ?? new CliHelpSymbolOutput(this);
            Indent = indent == 0 ? 2 : indent;
            SynopsisSection = new CliHelpSubcommands(this);
            UsageSection = new CliHelpUsage(this);
            ArgumentsSection = new CliHelpArguments(this);
            OptionsSection = new CliHelpOptions(this);
            SubCommandsSection = new CliHelpSubcommands(this);
        }


        public CliHelpSymbolOutput SymbolOutput { get; }
        public int Indent { get; }

        public CliHelpSection SynopsisSection { get; set; }
        public CliHelpSection UsageSection { get; set; }
        public CliHelpSection ArgumentsSection { get; set; }
        public CliHelpSection OptionsSection { get; set; }
        public CliHelpSection SubCommandsSection { get; set; }

        public virtual IEnumerable<CliHelpSection> GetSections()
            => new List<CliHelpSection>()
            {
                SynopsisSection,
                UsageSection,
                ArgumentsSection,
                OptionsSection,
                SubCommandsSection
            };

    }
}