using System.Collections.Generic;
using System.CommandLine.Help.Formatting;
using System.Linq;

namespace System.CommandLine.Help
{
    public class CliHelpOptions : CliHelpSection
    {
        public CliHelpOptions(CliDefaultHelpConfiguration helpConfiguration,
                               CliSymbolInspector symbolInspector,
                               CliFormatter formatter)
            : base(helpConfiguration, symbolInspector, formatter, LocalizationResources.HelpOptionsTitle())
        {
        }

        public override IEnumerable<string>? GetBody(HelpContext helpContext)
        {
            var table = GetBodyTable(helpContext);
            return Formatter.FormatTable(table, helpContext.MaxWidth);
        }

        private Table<CliOption>? GetBodyTable(HelpContext helpContext)
        {
            var command = helpContext.Command;
            if (command is null)
            {
                return null;
            }

            var options = GetOptions(command);
            var table = new Table<CliOption>(Formatter.IndentWidth, options);
            table.Body[0] = GetFirstColumn;
            table.Body[1] = GetSecondColumn;
            return table;

            static IEnumerable<CliOption>? GetOptions(CliCommand command)
                => command?.SelfAndParentCommands()
                    .Reverse()
                    .SelectMany(cmd => cmd.Options.Where(a => !a.Hidden))
                    .Distinct();
        }

        //    List<TwoColumnHelpRow> optionRows = new();
        //    bool addedHelpOption = false;
        //    foreach (CliOption option in command.Options)
        //    {
        //        if (!option.Hidden)
        //        {
        //            optionRows.Add(GetTwoColumnRow(option));
        //            if (option is HelpOption)
        //            {
        //                addedHelpOption = true;
        //            }
        //        }
        //    }

        //    CliCommand? current = command;
        //    while (current is not null)
        //    {
        //        var parent = current.Parents.FirstOrDefault() as CliCommand;
        //        if (parent is not null)
        //        {
        //            foreach (var option in parent.Options)
        //            {
        //                // global help aliases may be duplicated, we just ignore them
        //                if (option is { Recursive: true, Hidden: false })
        //                {
        //                    if (option is not HelpOption || !addedHelpOption)
        //                    {
        //                        optionRows.Add(GetTwoColumnRow(option));
        //                    }
        //                }
        //            }
        //        }

        //        current = parent;
        //    }

        //    // The following should not be using indentWidth
        //    return optionRows is null
        //        ? null
        //        : CliHelpHelpers.WriteTwoColumns(optionRows, helpContext.MaxWidth, Formatter.IndentWidth);

        //}

        private string GetFirstColumn(CliOption option)
            => SymbolInspector.GetUsage(option);

        private string GetSecondColumn(CliOption option)
        {
            var symbolDescription = SymbolInspector.GetDescription(option) ?? string.Empty;

            var defaultValueDescription = SymbolInspector.GetDefaultValueText(option);

            return string.IsNullOrEmpty(defaultValueDescription)
                ? $"{symbolDescription}".Trim()
                : $"{symbolDescription} [{defaultValueDescription}]".Trim();
        }
    }
}
