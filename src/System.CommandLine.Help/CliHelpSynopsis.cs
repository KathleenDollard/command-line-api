using System.Collections.Generic;
using System.CommandLine.CliOutput;

namespace System.CommandLine.Help
{
    public class CliHelpSynopsis : CliSection<CliCommand>
    {
        public CliHelpSynopsis()
            : base(LocalizationResources.HelpDescriptionTitle(), true)
        { }

        public override IEnumerable<CliOutputUnit>? GetBody(CliOutputContext outputContext)
        => outputContext is not HelpContext helpContext || helpContext.Command.Description is null
                ? null
                : new CliOutputUnit[] { new CliText(helpContext.Command.Description, 1) };


    }
}
