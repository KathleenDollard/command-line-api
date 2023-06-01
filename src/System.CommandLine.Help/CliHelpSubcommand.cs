using System.Collections.Generic;
using System.CommandLine.Help.Formatting;
using System.Linq;
using Default = System.CommandLine.Help.CliDefaultHelpConfiguration.Defaults;

namespace System.CommandLine.Help
{
    public class CliHelpSubcommands : CliHelpSection
    {
        public CliHelpSubcommands(CliDefaultHelpConfiguration helpConfiguration,
                               CliSymbolInspector symbolInspector,
                               CliFormatter formatter)
            : base(helpConfiguration, symbolInspector, formatter, LocalizationResources.HelpCommandsTitle())
        {
        }


        public override IEnumerable<string>? GetBody(HelpContext helpContext)
        {
            var table = GetBodyTable(helpContext);
            return Formatter.FormatTable(table, helpContext.MaxWidth);
        }

        private Table<CliCommand>? GetBodyTable(HelpContext helpContext)
        {
            var command = helpContext.Command;
            if (command is null)
            {
                return null;
            }

            var subCommands = command.Subcommands;

            var table = new Table<CliCommand>(Formatter.IndentWidth, subCommands);
            table.Body[0] = GetFirstColumn;
            table.Body[1] = GetSecondColumn;
            return table;
        }

        private string GetFirstColumn(CliCommand command)
            => SymbolInspector.GetUsage(command);

        private string GetSecondColumn(CliCommand command)
        {
            var symbolDescription = SymbolInspector.GetDescription(command) ?? string.Empty;

            var defaultValueDescription = GetCommandDefaultArgValues(command);
            return $"{symbolDescription} {defaultValueDescription}".Trim();
        }

        private string GetCommandDefaultArgValues(CliCommand command)
        {
            var args = command.Arguments
                .Where(arg => !arg.Hidden && arg.HasDefaultValue);

            return args.Count() switch
            {
                0 => "",
                1 => $"[{Default.DefaultValueTextAndLabel(args.First(), false)}]",
                _ => $"[{string.Join(", ", args.Select(arg => SymbolInspector.GetDefaultValueText(arg, true)))}]"
            };
        }
    }
}
