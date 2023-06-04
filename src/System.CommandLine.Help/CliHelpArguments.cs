using System.Collections.Generic;
using System.CommandLine.CliOutput;
using System.Linq;

namespace System.CommandLine.Help
{
    public class CliHelpArguments : CliSection<CliArgument>
    {
        public CliHelpArguments()
           : base(LocalizationResources.HelpArgumentsTitle())
        { }

        public override IEnumerable<CliOutputUnit>? GetBody(CliOutputContext outputContext)
        {
            if (outputContext is not HelpContext helpContext) { return null; }

            var unit = GetBodyTable(helpContext);
            return unit is null
                ? null
                : new CliOutputUnit[] { unit };
        }

        private CliTable<CliArgument>? GetBodyTable(HelpContext? helpContext)
        {
            if (helpContext is null)
            { return null; }

            var symbolInspector = CliHelpUtilities.SymbolInspector(helpContext);
            if (helpContext?.Command is not CliCommand command)
            {
                return null;
            }

            var args = GetArguments(command);
            var table = new CliTable<CliArgument>(2, args)
            {
                IndentLevel = 1
            };
            table.Body[0] = arg => GetFirstColumn(arg, symbolInspector);
            table.Body[1] = arg => GetSecondColumn(arg, symbolInspector);
            return table;

            static IEnumerable<CliArgument>? GetArguments(CliCommand command)
                => command?.SelfAndParentCommands()
                    .Reverse()
                    .SelectMany(cmd => cmd.Arguments.Where(a => !a.Hidden))
                    .Distinct();
        }

        private string GetFirstColumn(CliArgument argument, CliSymbolInspector symbolInspector)
            => symbolInspector.GetUsage(argument);

        private string GetSecondColumn(CliArgument argument, CliSymbolInspector symbolInspector)
        {
            var symbolDescription = symbolInspector.GetDescription(argument) ?? string.Empty;

            var defaultValueDescription = symbolInspector.GetDefaultValueText(argument, false);
            return string.IsNullOrEmpty(defaultValueDescription)
                ? $"{symbolDescription}".Trim()
                : $"{symbolDescription} [{defaultValueDescription}]".Trim();
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
