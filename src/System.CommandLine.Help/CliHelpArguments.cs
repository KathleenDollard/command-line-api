using System.Collections.Generic;
using System.CommandLine.Help.Formatting;
using System.Linq;

namespace System.CommandLine.Help
{
    public class CliHelpArguments : CliHelpSection<CliArgument>
    {
        public CliHelpArguments(CliHelpConfiguration helpConfiguration)
            : base(helpConfiguration, LocalizationResources.HelpArgumentsTitle())
        {
        }

        public override Table<CliArgument> GetBodyTable(HelpContext helpContext)
        {
            var args = GetArguments(helpContext.Command);
            var table = new Table<CliArgument>(2, args);
            table.Body[0] = GetFirstColumn;
            table.Body[1] = GetSecondColumn;
            return table;

            static IEnumerable<CliArgument>? GetArguments(CliCommand command)
            => command is null
                ? null
                : command.SelfAndParentCommands()
                    .SelectMany(cmd => cmd.Arguments.Where(a => !a.Hidden))
                    .Reverse();
        }
            //    if (command is null)
            //    { return null; }

            //    var selfAndParents = 

            //    var table = selfAndParents
            //            .SelectMany(cmd => cmd.Arguments.Where(a => !a.Hidden))
            //            .Select(a => GetTwoColumnRow(a))
            //            .Distinct();

            //    return table is null
            //        ? null
            //        : CliHelpHelpers.WriteTwoColumns(table, helpContext.MaxWidth, Indent);
            //}
        //}

        private string GetFirstColumn(CliArgument argument)
            => argument.GetUsage();

        private string GetSecondColumn(CliArgument argument)
        {
            var symbolDescription = argument.Description ?? string.Empty;

            var defaultValueDescription = argument.GetDefaultValueText(false);
            if (string.IsNullOrEmpty(defaultValueDescription))
            {
                return $"{symbolDescription}".Trim();
            }
            else
            {
                return $"{symbolDescription} [{defaultValueDescription}]".Trim();
            }
        }


        //private TwoColumnHelpRow? GetTwoColumnRow(CliArgument argument)
        //{
        //    _ = argument ?? throw new ArgumentNullException(nameof(argument));

        //    string firstColumnText = argument.GetUsage();
        //    string secondColumnText = GetSecondColumnText(argument);

        //    return new TwoColumnHelpRow(firstColumnText, secondColumnText);

        //    string GetSecondColumnText(CliArgument argument)
        //    {
        //        var symbolDescription = argument.Description ?? string.Empty;

        //        var defaultValueDescription = argument.GetDefaultValueText(false);
        //        if (string.IsNullOrEmpty(defaultValueDescription))
        //        {
        //            return $"{symbolDescription}".Trim();
        //        }
        //        else
        //        {
        //            return $"{symbolDescription} [{defaultValueDescription}]".Trim();
        //        }


        //    }
        //}


    }
}
