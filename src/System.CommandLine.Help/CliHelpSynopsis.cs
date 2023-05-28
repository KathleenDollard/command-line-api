using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine.Help
{
    public class CliHelpSubcommands : CliHelpSection<CliCommand>
    {
        public CliHelpSubcommands(CliHelpConfiguration helpConfiguration)
            : base(helpConfiguration, LocalizationResources.HelpDescriptionTitle(), true)
        {
        }

        public override IEnumerable<string>? GetBody(HelpContext helpContext)
        => CliHelpHelpers.WrapAndIndentText(helpContext.Command.Description ?? string.Empty, maxWidth: helpContext.MaxWidth, indent: Indent);

    }
}
