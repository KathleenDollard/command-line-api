using System.Collections.Generic;
using System.CommandLine.CliOutput;
using System.Linq;

namespace System.CommandLine.Help
{
    public class CliHelpArguments : CliHelpSection
    {
        public CliHelpArguments(CliDefaultHelpConfiguration helpConfiguration,
                              CliSymbolInspector symbolInspector,
                              CliFormatter formatter)
           : base(helpConfiguration, symbolInspector, formatter, LocalizationResources.HelpArgumentsTitle())
        {
        }

        public override IEnumerable<string>? GetBody(CliOutputContext outputContext) 
        => outputContext switch
            {
                HelpContext helpContext => Formatter.FormatTable(GetBodyTable(helpContext), helpContext.MaxWidth),
                _ => null
            };

        private  Table<CliArgument>? GetBodyTable(HelpContext? helpContext)
        {
            if (helpContext?.Command is not CliCommand command)
            {
                return null;
            }

            var args = GetArguments(command);
            var table = new Table<CliArgument>(Formatter.IndentWidth, args);
            table.Body[0] = GetFirstColumn;
            table.Body[1] = GetSecondColumn;
            return table;

            static IEnumerable<CliArgument>? GetArguments(CliCommand command)
                => command?.SelfAndParentCommands()
                    .Reverse()
                    .SelectMany(cmd => cmd.Arguments.Where(a => !a.Hidden))
                    .Distinct();
        }
   
        private string GetFirstColumn(CliArgument argument)
            => SymbolInspector.GetUsage(argument);

        private string GetSecondColumn(CliArgument argument)
        {
            var symbolDescription = SymbolInspector.GetDescription(argument) ?? string.Empty;

            var defaultValueDescription = SymbolInspector.GetDefaultValueText(argument,false);
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
