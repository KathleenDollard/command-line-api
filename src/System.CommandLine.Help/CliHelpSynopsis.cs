using System.Collections.Generic;
using System.CommandLine.CliOutput;

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

        public override IEnumerable<string>? GetBody(CliOutputContext outputContext)
        {
            return outputContext is not HelpContext helpContext
                ? null
                : CliHelpHelpers.WrapAndIndentText(helpContext.Command.Description ?? string.Empty, maxWidth: outputContext.MaxWidth, indent: Formatter.IndentWidth);
        }
    }
}
