using System.Collections.Generic;
using System.CommandLine.Help.Formatting;
using System.Linq;
using Default = System.CommandLine.Help.CliDefaultHelpConfiguration.Defaults;

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
            var table = new Table<CliOption>(2, options);
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
            var symbolDescription = option.Description ?? string.Empty;

            var defaultValueDescription = SymbolInspector.GetDefaultValueText(option);
            if (string.IsNullOrEmpty(defaultValueDescription))
            {
                return $"{symbolDescription}".Trim();
            }
            else
            {
                return $"{symbolDescription} [{defaultValueDescription}]".Trim();
            }
        }

        //private TwoColumnHelpRow GetTwoColumnRow(CliOption option)
        //{
        //    _ = option ?? throw new ArgumentNullException(nameof(option));

        //    string firstColumnText = SymbolInspector.GetUsage(option);
        //    string secondColumnText = GetSecondColumnText(option);

        //    return new TwoColumnHelpRow(firstColumnText, secondColumnText);

        //    string GetSecondColumnText(CliOption option)
        //    {
        //        var symbolDescription = option.Description;

        //        var defaultValueDescription = Default.DefaultValueText(option.GetDefaultValue());

        //        if (string.IsNullOrEmpty(defaultValueDescription))
        //        {
        //            return $"{symbolDescription}".Trim();
        //        }
        //        else
        //        {
        //            return $"{symbolDescription} [{LocalizationResources.HelpArgumentDefaultValueLabel()}: {defaultValueDescription}]".Trim();
        //        }
        //    }
        //}
    }
}
