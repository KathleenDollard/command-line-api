using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine.Help
{
    public class CliHelpArguments : CliHelpSection
    {
        public CliHelpArguments(CliHelpConfiguration helpConfiguration, HelpContext helpContext)
            : base(helpConfiguration, LocalizationResources.HelpArgumentsTitle())
        {
        }

        public override IEnumerable<string>? GetBody(HelpContext helpContext)
        {
            var current = helpContext.Command;
            if (current is null)
            { return null; }

            var selfAndParents = current.SelfAndParentCommands()
                .Reverse();

            var table = selfAndParents
                    .SelectMany(cmd => cmd.Arguments.Where(a => !a.Hidden))
                    .Select(a => GetTwoColumnRow(a))
                    .Distinct();

            return table is null
                ? null
                : CliHelpHelpers.WriteTwoColumns(table, helpContext.MaxWidth, Indent);
        }


        private TwoColumnHelpRow? GetTwoColumnRow(CliArgument argument)
        {
            _ = argument ?? throw new ArgumentNullException(nameof(argument));

            string firstColumnText = SymbolOutput.GetUsage(argument);
            string secondColumnText = GetSecondColumnText(argument);

            return new TwoColumnHelpRow(firstColumnText, secondColumnText);

            string GetSecondColumnText(CliArgument argument)
            {
                var symbolDescription = argument.Description ?? string.Empty;

                var defaultValueDescription = SymbolOutput.GetDefaultValueText(argument, false);
                if (string.IsNullOrEmpty(defaultValueDescription))
                {
                    return $"{symbolDescription}".Trim();
                }
                else
                {
                    return $"{symbolDescription} [{defaultValueDescription}]".Trim();
                }


            }
        }

        ///// <summary>
        ///// Gets the usage title for an argument (for example: <c>&lt;value&gt;</c>, typically used in the first column text in the arguments usage section, or within the synopsis.
        ///// </summary>
        //private static string GetArgumentUsageLabel(CliArgument argument)
        //{
        //    // Argument.HelpName is always first choice
        //    if (!string.IsNullOrWhiteSpace(argument.HelpName))
        //    {
        //        return $"<{argument.HelpName}>";
        //    }

        //    if (argument.ValueType == typeof(bool))
        //    {
        //        return string.Empty;
        //    }

        //    var completionItems = argument.GetCompletions(CompletionContext.Empty);
        //    if (completionItems.Any())
        //    {

        //        IEnumerable<string> completions = completionItems
        //            .Select(item => item.Label);

        //        string joined = string.Join("|", completions);

        //        if (!string.IsNullOrEmpty(joined))
        //        {
        //            return $"<{joined}>";
        //        }
        //    }

        //    return $"<{argument.Name}>";
        //}


        //private string GetArgumentDefaultValue(
        //    CliArgument argument,
        //    bool displayArgumentName)
        //{
        //    string? displayedDefaultValue = GetArgumentDefaultValue(argument);

        //    return string.IsNullOrWhiteSpace(displayedDefaultValue) 
        //        ? "" 
        //        : $"{GetLabel(argument, displayArgumentName)}: {displayedDefaultValue}";

        //    static string GetLabel(CliArgument argument, bool displayArgumentName) => 
        //        displayArgumentName
        //          ? LocalizationResources.HelpArgumentDefaultValueLabel()
        //          : argument.Name;
        //}

        //public static string GetArgumentDefaultValue(HelpContext helpContext)
        //{
        //    var defaultValue = symbol switch
        //    {
        //        CliArgument argument => argument.GetHelpDefaultValue(),
        //        CliOption option => option.GetHelpDefaultValue(),
        //        _ => null
        //    };
        //    if (defaultValue is not null)
        //    {
        //        if (defaultValue is IEnumerable enumerable and not string)
        //        {
        //            return string.Join("|", enumerable.OfType<object>().ToArray());
        //        }
        //        else
        //        {
        //            return defaultValue.ToString() ?? "";
        //        }
        //    }
        //    return string.Empty;
        //}
    }
}
