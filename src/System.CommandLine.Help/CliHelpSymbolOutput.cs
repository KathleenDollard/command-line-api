using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.CommandLine.Parsing;

namespace System.CommandLine.Help
{
    public class CliHelpSymbolOutput
    {
        public CliHelpSymbolOutput(CliHelpConfiguration helpConfiguration)
        {
            HelpConfiguration = helpConfiguration;
        }

        public CliHelpConfiguration HelpConfiguration { get; }

        public virtual string? GetName(CliArgument symbol)
        => symbol.HelpName;
        public virtual string? GetName(CliSymbol symbol)
        => symbol switch
        {
            CliArgument argument => GetName(argument),
            _ => symbol.Name
        };

        public virtual string? GetAliases(CliOption option)
        => GetAliasesFromCollection(option.Aliases);
        public virtual string? GetAliases(CliCommand command)
        => GetAliasesFromCollection(command.Aliases);
        public virtual string? GetAliases(CliSymbol symbol)
        => symbol switch
        {
            CliCommand command => GetAliases(command),
            CliOption option => GetAliases(option),
            _ => null
        };

        public virtual string? GetDescription(CliSymbol symbol)
        => symbol.Description;

        public virtual string? GetArgumentName(CliOption option)
        => option.ArgumentHelpName;

        public virtual string? GetDefaultValue(CliOption option)
        => option.GetHelpDefaultValue()?.ToString();
        public virtual string? GetDefaultValue(CliArgument argument)
        => argument.GetHelpDefaultValue()?.ToString();
        public virtual string? GetDefaultValue(CliSymbol symbol)
        => symbol switch
        {
            CliArgument argument => GetDefaultValue(argument),
            CliOption option => GetDefaultValue(option),
            _ => null
        };

        public virtual string? CombineArgumentDefaultValues(CliCommand command)
        => string.Join("|",
            command.Arguments
                .Select(arg => GetDefaultValue(arg)));

        private string? GetAliasesFromCollection(IEnumerable<string> aliases)
            => string.Join(", ", aliases);

        public string GetUsage(CliArgument argument)
        {
            return GetUsage(null, argument.ValueType, argument, showUsageOnBool: true);
        }
        public string GetUsage(CliOption option)
        {
            var text = GetAliasText(option, option.Aliases);

            var argumentLabel = GetUsage(option.ArgumentHelpName, option.ValueType, option, showUsageOnBool: false, skipNameDefault: true);

            if (!string.IsNullOrEmpty(argumentLabel))
            {
                text += $" {argumentLabel}";
            }

            if (option.Required)
            {
                text += $" {LocalizationResources.HelpOptionsRequiredLabel()}";
            }

            return text;
        }
        public string GetUsage(CliCommand command)
        {
            var text = GetAliasText(command, command.Aliases);
            string argumentText = null;

            if (command.Arguments.Any())
            {
                var argumentLabels = command.Arguments
                    .Where(arg => !arg.Hidden)
                    .Select(arg => GetUsage("", arg.ValueType, arg, false));
                argumentText = string.Join(", ", argumentLabels);
            }

            return string.IsNullOrEmpty(argumentText)
                ? text
                : $"{text} {argumentText}";
        }
        public string GetDefaultValueText(
        CliArgument argument,
        bool displayArgumentName)
        {
            var defaultValue = GetDefaultValue(argument);

            string? displayedDefaultValue = defaultValue is null
                ? string.Empty
                : defaultValue is IEnumerable enumerable and not string
                   ? string.Join("|", enumerable.OfType<object>().ToArray())
                   : defaultValue?.ToString() ?? "";

            return string.IsNullOrWhiteSpace(displayedDefaultValue)
                ? ""
                : $"{GetLabel(argument, displayArgumentName)}: {displayedDefaultValue}";

            static string GetLabel(CliArgument argument, bool displayArgumentName) =>
                displayArgumentName
                  ? LocalizationResources.HelpArgumentDefaultValueLabel()
                  : argument.Name;
        }

        public string GetDefaultValueText(
            CliOption option)
        {
            if (option.Hidden)
            {
                return string.Empty;
            }
            var defaultValue = option.GetHelpDefaultValue();

            if (defaultValue is null) return "";

            var defaultValueText = defaultValue is IEnumerable enumerable and not string
                ? string.Join("|", enumerable.OfType<object>().ToArray())
                : defaultValue.ToString() ?? "";

            var argumentDefaultValue = GetDefaultValue(option);
            return $"{defaultValueText}";
        }

        private string GetAliasText(CliSymbol symbol, IEnumerable<string> aliases)
        {
            var allAliases = aliases is null || !aliases.Any()
                ? new[] { symbol.Name }
                : new[] { symbol.Name }.Concat(aliases)
                    .Select(r => r.SplitPrefix())
                    .OrderBy(r => r.Prefix, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(r => r.Alias, StringComparer.OrdinalIgnoreCase)
                    .GroupBy(t => t.Alias)
                    .Select(t => t.First())
                    .Select(t => $"{t.Prefix}{t.Alias}");
            return string.Join(", ", allAliases);
        }

        private string GetUsage(string? helpName, Type valueType, CliSymbol symbol, bool showUsageOnBool, bool skipNameDefault = false)
        {
            if (!string.IsNullOrWhiteSpace(helpName))
            {
                return $"<{helpName}>";
            }

            var isBool = valueType == typeof(bool);

            if (!showUsageOnBool && isBool)
            {
                return string.Empty;
            }

            if (!isBool) // never show true/false
            {
                var completionItems = symbol.GetCompletions();
                if (completionItems.Any())
                {
                    IEnumerable<string> completions = completionItems
                        .Select(item => item.Label);

                    string joined = string.Join("|", completions);

                    if (!string.IsNullOrEmpty(joined))
                    {
                        return $"<{joined}>";
                    }
                }
            }

            return skipNameDefault ? "" : $"<{symbol.Name}>";
        }
    }
}
