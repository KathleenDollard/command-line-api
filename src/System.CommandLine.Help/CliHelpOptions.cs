using System.Collections.Generic;
using System.Linq;
using Default = System.CommandLine.Help.CliDefaultHelpConfiguration.Defaults;

namespace System.CommandLine.Help
{
    public class CliHelpOptions : CliHelpSection
    {
        public CliHelpOptions(CliDefaultHelpConfiguration helpConfiguration)
            : base(helpConfiguration, LocalizationResources.HelpOptionsTitle())
        {
        }

        public override IEnumerable<string>? GetBody(HelpContext helpContext)
        {
            var symbol = helpContext.Command;
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
                : CliHelpHelpers.WriteTwoColumns(optionRows, helpContext.MaxWidth, Indent);

        }


        private TwoColumnHelpRow GetTwoColumnRow(CliOption option)
        {
            _ = option ?? throw new ArgumentNullException(nameof(option));

            string firstColumnText = option.GetUsage();
            string secondColumnText = GetSecondColumnText(option);

            return new TwoColumnHelpRow(firstColumnText, secondColumnText);

            string GetSecondColumnText(CliOption option)
            {
                var symbolDescription = option.Description;

                var defaultValueDescription = Default.DefaultValueText(option.GetDefaultValue());

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
