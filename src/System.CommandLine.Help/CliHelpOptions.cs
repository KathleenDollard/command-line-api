using System.Collections.Generic;
using System.CommandLine.CliOutput;
using System.Linq;

namespace System.CommandLine.Help
{
    public class CliHelpOptions : CliSection<CliOption>
    {
        public CliHelpOptions()
            : base(LocalizationResources.HelpOptionsTitle())
        { }

        public override IEnumerable<CliOutputUnit>? GetBody(CliOutputContext outputContext)
        {
            if (outputContext is not HelpContext helpContext) { return null; }

            var unit = GetBodyTable(helpContext);
            return unit is null
                ? null
                : new CliOutputUnit[] { unit };
        }

        private CliTable<CliOption>? GetBodyTable(HelpContext helpContext)
        {
            if (helpContext?.Command is not CliCommand command)
            {
                return null;
            }

            var symbolInspector = CliHelpUtilities.SymbolInspector(helpContext);

            var options = GetOptions(command);
            var table = new CliTable<CliOption>(2, options);
            table.IndentLevel = 1;
            table.Body[0] = opt => GetFirstColumn(opt, symbolInspector);
            table.Body[1] = opt => GetSecondColumn(opt, symbolInspector);
            return table;

            static IEnumerable<CliOption>? GetOptions(CliCommand command)
                => command?.SelfAndParentCommands()
                    .Reverse()
                    .SelectMany(cmd => cmd.Options.Where(a => !a.Hidden))
                    .Distinct();
        }

        private string GetFirstColumn(CliOption option, CliSymbolInspector symbolInspector)
            => symbolInspector.GetUsage(option);

        private string GetSecondColumn(CliOption option, CliSymbolInspector symbolInspector)
        {
            var symbolDescription = symbolInspector.GetDescription(option) ?? string.Empty;

            var defaultValueDescription = symbolInspector.GetDefaultValueText(option);

            return string.IsNullOrEmpty(defaultValueDescription)
                ? $"{symbolDescription}".Trim()
                : $"{symbolDescription} [{defaultValueDescription}]".Trim();
        }
    }
}
