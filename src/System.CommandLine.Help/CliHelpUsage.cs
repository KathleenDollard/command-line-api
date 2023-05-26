using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace System.CommandLine.Help
{
    public class CliHelpUsage : CliHelpSection
    {
        public CliHelpUsage(CliHelpConfiguration helpConfiguration, HelpContext helpContext) 
            : base(helpConfiguration, helpContext, LocalizationResources.HelpUsageTitle())
        {
        }

        public override IEnumerable<string>? GetBody(CliSymbol current)
        => current switch
        {
            CliCommand command => CliHelpHelpers.WrapAndIndentText( GetUsage(command), HelpContext.MaxWidth, Indent),
            _=>null
        };

        public override IEnumerable<string>? GetClosing(CliSymbol current)
        => null;

        // Consider rewriting to make it easier to adjust parts without rewriting whole
        private string GetUsage(CliCommand command)
        {
            return string.Join(" ", GetUsageParts().Where(x => !string.IsNullOrWhiteSpace(x)));

            IEnumerable<string> GetUsageParts()
            {
                bool displayOptionTitle = false;

                var selfAndParents = command.SelfAndParentCommands()
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
