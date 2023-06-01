using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.CommandLine.Parsing;
using System.Diagnostics.CodeAnalysis;

namespace System.CommandLine.Help
{
    public class CliSymbolInspector
    {
        protected HelpContext HelpContext { get; }

        public CliSymbolInspector(HelpContext helpContext)
            => HelpContext = helpContext;

        public string? GetName(CliSymbol symbol)
        {
            return symbol switch
            {
                CliArgument argument => GetArgumentNameOrHelpName(argument),
                _ => symbol.Name
            };
            static string GetArgumentNameOrHelpName(CliArgument argument)
                => string.IsNullOrWhiteSpace(argument.HelpName)
                    ? argument.Name
                    : argument.HelpName ?? string.Empty;
        }

        /// <summary>
        /// Get the aliases for a command or option
        /// </summary>
        /// <param name="symbol">The symbol to get the aliases for</param>
        /// <remarks>
        /// The expected customization here is to hide individual aliases that are logically deprecated
        /// </remarks>
        /// <returns>The aliases for commands and options, and an empty string for arguments</returns>
        public virtual IEnumerable<string>? GetAliases(CliSymbol symbol)
        => symbol switch
        {
            CliCommand command => command.Aliases,
            CliOption option => option.Aliases,
            _ => null
        };

        /// <summary>
        /// Given a list of aliases, provide a consistent display
        /// </summary>
        /// <param name="symbol">The symbol the aliases are for.</param>
        /// <param name="aliases">The aliases.</param>
        /// <remarks>
        /// The expected customization here is to layout the aliases differently, such as a different delimiter.
        /// </remarks>
        /// <returns>A string with the aliases sorted, distinct and comma delimited</returns>
        public virtual string GetAliasText(CliSymbol symbol, IEnumerable<string> aliases)
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

        /// <summary>
        /// The description for the symbol
        /// </summary>
        /// <param name="symbol">The symbol the description is for for.</param>
        /// <remarks>
        /// The expected customization here is to offer a different description for options when they appear 
        /// on different commands. The current command is in the help context property
        /// </remarks>
        /// <returns>The symbol description</returns>
        public virtual string? GetDescription(CliSymbol symbol)
        => symbol.Description;

        public string? GetArgumentName(CliOption option)
        => option.ArgumentHelpName;

        public object? GetDefaultValue(CliSymbol symbol)
        => symbol switch
        {
            CliArgument argument => argument.GetDefaultValue(),
            CliOption option => option.GetDefaultValue(),
            _ => null
        };

        public string GetDefaultValueText(
            CliSymbol symbol,
            bool displayArgumentName = false)
        {
            var defaultValue = symbol switch
            {
                CliArgument argument => argument.GetDefaultValue(),
                CliOption option => option.GetDefaultValue(),
                _ => null
            };
            var defaultValueText = defaultValue switch
            {
                null => string.Empty,
                string s => s,
                IEnumerable enumerable => string.Join("|", enumerable.OfType<object>().ToArray()),
                _ => defaultValue.ToString()
            };

            return string.IsNullOrWhiteSpace(defaultValueText)
                ? ""
                : $"{GetLabel(symbol, displayArgumentName)}: {defaultValueText}";

            string GetLabel(CliSymbol symbol, bool displayArgumentName) =>
               displayArgumentName
                 ? GetName(symbol) ?? string.Empty
                 : LocalizationResources.HelpArgumentDefaultValueLabel();
        }

        public string GetUsage(CliSymbol symbol)
        => symbol switch
        {
            CliArgument argument => GetArgUsage(null, argument.ValueType, argument, showUsageOnBool: true),
            CliOption option => GetUsage(option),
            CliCommand command => GetUsage(command),
            _=> string.Empty
        };

        protected string GetUsage(CliOption option)
        {
            var text = GetAliasText(option, option.Aliases);

            var argumentLabel = GetArgUsage(option.ArgumentHelpName, option.ValueType, option, showUsageOnBool: false, skipNameDefault: true);

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
        protected string GetUsage(CliCommand command)
        {
            var text = GetAliasText(command, command.Aliases);
            string argumentText = string.Empty;

            if (command.Arguments.Any())
            {
                var argumentLabels = command.Arguments
                    .Where(arg => !arg.Hidden)
                    .Select(arg => GetArgUsage("", arg.ValueType, arg, false));
                argumentText = string.Join(", ", argumentLabels);
            }

            return string.IsNullOrEmpty(argumentText)
                ? text
                : $"{text} {argumentText}";
        }


        protected string GetArgUsage(string? helpName, Type valueType, CliSymbol symbol, bool showUsageOnBool, bool skipNameDefault = false)
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
