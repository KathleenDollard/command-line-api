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

        public string GetDefaultValueText(CliOption option)
        {
            var defaultValue = option.GetDefaultValue();

            return defaultValue switch
            {
                null => string.Empty,
                string s => s,
                IEnumerable enumerable => string.Join("|", enumerable.OfType<object>().ToArray()),
                _ => defaultValue.ToString()
            };
        }
        public string GetDefaultValueText(
            CliArgument argument,
            bool displayArgumentName)
        {
            var defaultValue = argument.GetDefaultValue();
            var defaultValueText = defaultValue switch
            {
                null => string.Empty,
                string s => s,
                IEnumerable enumerable => string.Join("|", enumerable.OfType<object>().ToArray()),
                _ => defaultValue.ToString()
            };          

            return string.IsNullOrWhiteSpace(defaultValueText)
                ? ""
                : $"{GetLabel(argument, displayArgumentName)}: {defaultValueText}";

            static string GetLabel(CliArgument argument, bool displayArgumentName) =>
                displayArgumentName
                  ? argument.Name
                  : LocalizationResources.HelpArgumentDefaultValueLabel();
        }

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


            if (!showUsageOnBool && (valueType == typeof(bool) || valueType == typeof(bool?)))
            {
                return string.Empty;
            }

            if (!(valueType == typeof(bool) || valueType == typeof(bool?))) // never show true/false
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
