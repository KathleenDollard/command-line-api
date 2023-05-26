using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine.Help
{
    public class CliHelpSynopsis : CliHelpSection
    {
        public CliHelpSynopsis(CliHelpConfiguration helpConfiguration, HelpContext helpContext)
            : base(helpConfiguration, helpContext, LocalizationResources.HelpDescriptionTitle(), true)
        {
        }

        public override IEnumerable<string>? GetBody(CliSymbol current)
        => CliHelpHelpers.WrapAndIndentText(HelpContext.Command.Description ?? string.Empty, maxWidth: HelpContext.MaxWidth, indent: Indent);

        public override IEnumerable<string>? GetClosing(CliSymbol current)
        => null;
    }
}
