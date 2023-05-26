using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine.Help
{
    public class CliHelpOptions : CliHelpSection
    {
        public CliHelpOptions(CliHelpConfiguration helpConfiguration, HelpContext helpContext)
            : base(helpConfiguration, helpContext, LocalizationResources.HelpOptionsTitle())
        {
        }

        public override IEnumerable<string>? GetBody(CliSymbol symbol)
        {
            if (symbol is not CliCommand command)
            {
                return null;
            }
            List<TwoColumnHelpRow> optionRows = new();
            bool addedHelpOption = false;
            foreach (CliOption option in command.Options)
            {
                if (!option.Hidden)
                {
                    optionRows.Add(GetTwoColumnRow(option));
                    if (option is HelpOption)
                    {
                        addedHelpOption = true;
                    }
                }
            }

            CliCommand? current = command;
            while (current is not null)
            {
                var parent = current.Parents.FirstOrDefault() as CliCommand;
                if (parent is not null)
                {
                    foreach (var option in parent.Options)
                    {
                        // global help aliases may be duplicated, we just ignore them
                        if (option is { Recursive: true, Hidden: false })
                        {
                            if (option is not HelpOption || !addedHelpOption)
                            {
                                optionRows.Add(GetTwoColumnRow(option));
                            }
                        }
                    }
                }

                current = parent;
            }

            return optionRows is null
                ? null
                : CliHelpHelpers.WriteTwoColumns(optionRows, HelpContext.MaxWidth, Indent);

        }

        public override IEnumerable<string>? GetClosing(CliSymbol current)
        => null;

        private TwoColumnHelpRow GetTwoColumnRow(CliOption option)
        {
            _ = option ?? throw new ArgumentNullException(nameof(option));

            string firstColumnText = SymbolOutput.GetUsage(option);
            string secondColumnText = GetSecondColumnText(option);

            return new TwoColumnHelpRow(firstColumnText, secondColumnText);

            string GetSecondColumnText(CliOption option)
            {
                var symbolDescription = option.Description;

                var defaultValueDescription = SymbolOutput.GetDefaultValueText(option);

                if (string.IsNullOrEmpty(defaultValueDescription))
                {
                    return $"{symbolDescription}".Trim();
                }
                else
                {
                    return $"{symbolDescription} [{LocalizationResources.HelpArgumentDefaultValueLabel()}: {defaultValueDescription}]".Trim();
                }
            }
        }
    }
}
