using System.Collections.Generic;
using System.CommandLine.CliOutput;
using System.Linq;
using System.Text;

namespace System.CommandLine.Help
{
    public class CliHelpUsage : CliSection<InspectorCommandData>
    {
        public CliHelpUsage()
            : base(LocalizationResources.HelpUsageTitle(), true)
        {
        }

        public override IEnumerable<InspectorCommandData> GetData(CliOutputContext outputContext)
        {
            if (outputContext is not HelpContext helpContext)
            {
                return Enumerable.Empty<InspectorCommandData>();
            }

            var symbolInspector = CliHelpUtilities.SymbolInspector(helpContext);
            return new InspectorCommandData[]
                {
                    symbolInspector.GetCommandData(helpContext.Command, null) 
                };
        }

        public override IEnumerable<CliOutputUnit>? GetBody(CliOutputContext outputContext)
        {
            if (outputContext is not HelpContext helpContext)
            { return null; }
            var symbolInspector = CliHelpUtilities.SymbolInspector(helpContext);
            var data = GetUsage(helpContext.Command, symbolInspector);
            return data is null
                ? null
                : new CliOutputUnit[] { new CliText(data, 1) };

        }

        // Consider rewriting to make it easier to adjust parts without rewriting whole
        private string GetUsage(CliCommand command, CliSymbolInspector symbolInspector)
        {
            return string.Join(" ", GetUsageParts(symbolInspector).Where(x => !string.IsNullOrWhiteSpace(x)));

            IEnumerable<string> GetUsageParts(CliSymbolInspector symbolInspector)
            {
                bool displayOptionTitle = false;

                var selfAndParents = symbolInspector.SelfAndParentCommands(command)
                    .Reverse();

                // KAD: We either accept a few extra allocations, or we expose HasOptions, etc.
                foreach (var parentCommand in selfAndParents)
                {
                    if (!displayOptionTitle)
                    {
                        displayOptionTitle = parentCommand.Options.Any(x => x.Recursive && !x.Hidden);
                    }

                    yield return parentCommand.Name;

                    if (parentCommand.Arguments.Any())
                    {
                        yield return FormatArgumentUsage(parentCommand.Arguments);
                    }
                }

                var hasCommandWithHelp = command.Subcommands.Any(x => !x.Hidden);

                if (hasCommandWithHelp)
                {
                    yield return LocalizationResources.HelpUsageCommand();
                }

                displayOptionTitle = displayOptionTitle || command.Options.Any(x => !x.Hidden);

                if (displayOptionTitle)
                {
                    yield return LocalizationResources.HelpUsageOptions();
                }

                if (!command.TreatUnmatchedTokensAsErrors)
                {
                    yield return LocalizationResources.HelpUsageAdditionalArguments();
                }
            }
        }
        private string FormatArgumentUsage(IList<CliArgument> arguments)
        {
            var sb = new StringBuilder(arguments.Count * 100);

            var end = default(List<char>);

            for (var i = 0; i < arguments.Count; i++)
            {
                var argument = arguments[i];
                if (argument.Hidden)
                {
                    continue;
                }

                var arityIndicator =
                    argument.Arity.MaximumNumberOfValues > 1
                        ? "..."
                        : "";

                var isOptional = IsOptional(argument);

                if (isOptional)
                {
                    sb.Append($"[<{argument.Name}>{arityIndicator}");
                    (end ??= new()).Add(']');
                }
                else
                {
                    sb.Append($"<{argument.Name}>{arityIndicator}");
                }

                sb.Append(' ');
            }

            if (sb.Length > 0)
            {
                sb.Length--;

                if (end is { })
                {
                    while (end.Count > 0)
                    {
                        sb.Append(end[end.Count - 1]);
                        end.RemoveAt(end.Count - 1);
                    }
                }
            }

            return sb.ToString();

            bool IsOptional(CliArgument argument) =>
                argument.Arity.MinimumNumberOfValues == 0;
        }



    }
}
