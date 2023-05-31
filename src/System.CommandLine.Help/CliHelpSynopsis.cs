using System.Collections.Generic;
using System.CommandLine.Help.Formatting;
using System.Linq;

namespace System.CommandLine.Help
{
    public class CliHelpSynopsis : CliHelpSection
    {
        public CliHelpSynopsis(CliDefaultHelpConfiguration helpConfiguration,
                               CliSymbolInspector symbolInspector,
                               CliFormatter formatter)
            : base(helpConfiguration, symbolInspector, formatter, LocalizationResources.HelpDescriptionTitle(), true)
        {
        }

        public override IEnumerable<string>? GetBody(HelpContext helpContext)
        => CliHelpHelpers.WrapAndIndentText(helpContext.Command.Description ?? string.Empty, maxWidth: helpContext.MaxWidth, indent: Formatter.IndentWidth);

    }
}
