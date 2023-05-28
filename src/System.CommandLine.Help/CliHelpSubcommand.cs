using System.Collections.Generic;
using System.Linq;
using Default = System.CommandLine.Help.CliHelpConfiguration.Defaults;

namespace System.CommandLine.Help
{
    public class CliHelpSubcommand : CliHelpSection<CliCommand>
    {
        public CliHelpSubcommand(CliHelpConfiguration helpConfiguration)
            : base(helpConfiguration, LocalizationResources.HelpCommandsTitle())
        {
        }


        public override IEnumerable<string>? GetBody(HelpContext helpContext)
        {
            var symbol = helpContext.Command;
            if (symbol is not CliCommand command )
            {
                return null;
            }


            var table = command.Subcommands.Where(x => !x.Hidden).Select(x => GetTwoColumnRow(x));

            if (!command.Subcommands.Any())
            {
                return null;
            }

            return table is null
              ? null
              : CliHelpHelpers.WriteTwoColumns(table, helpContext.MaxWidth, Indent);

        }

        private TwoColumnHelpRow GetTwoColumnRow(CliCommand command)
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            string firstColumnText = command.GetUsage();
            string secondColumnText = GetSecondColumnText(command);

            return new TwoColumnHelpRow(firstColumnText, secondColumnText);

            string GetSecondColumnText(CliCommand command)
            {
                var symbolDescription = command.Description ?? string.Empty;

                var defaultValueDescription = GetCommandDefaultValue(command);

                return $"{symbolDescription} {defaultValueDescription}".Trim();
            }

            string GetCommandDefaultValue(CliCommand command)
            {

                if (command.Hidden)
                {
                    return string.Empty;
                }

                var args = command.Arguments
                    .Where(arg => !arg.Hidden && arg.HasDefaultValue);

                if (!args.Any())
                {
                    return "";
                }

                return args.Count() switch
                {
                    0 => "",
                    1 => $"[{Default.DefaultValueTextAndLabel(args.First(), false)}]",
                    _ => $"[{string.Join(", ", args.Select(arg => Default.DefaultValueTextAndLabel(args.First(), false)))}]"
                };
            }
        }
    }
}
