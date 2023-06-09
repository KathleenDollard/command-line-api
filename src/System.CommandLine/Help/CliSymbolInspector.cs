using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.CommandLine.Parsing;
using System.Diagnostics.CodeAnalysis;
using System.CommandLine.Completions;
using System.Runtime.CompilerServices;

namespace System.CommandLine.Help
{
    public class CliSymbolInspector
    {

        // Regions used to explain the code during development. Can delete if code is moved into BCL 
        #region Retrieval methods

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
        public IEnumerable<string>? GetAliases(CliSymbol symbol)
        => symbol switch
        {
            CliCommand command => command.Aliases,
            CliOption option => option.Aliases,
            _ => null
        };

        /// <summary>
        /// The description for the symbol
        /// </summary>
        /// <param name="symbol">The symbol the description is for for.</param>
        /// <remarks>
        /// The expected customization here is to offer a different description for options when they appear 
        /// on different commands. The current command is in the help context property
        /// </remarks>
        /// <returns>The symbol description</returns>
        public string? GetDescription(CliSymbol symbol)
        => symbol.Description;

        public string? GetArgumentName(CliOption option)
        => option.ArgumentHelpName;

        public object? GetDefaultValue(CliSymbol symbol)
        => symbol switch
        {
            CliArgument { HasDefaultValue: true} argument=> argument.GetDefaultValue(),
            CliOption { Argument.HasDefaultValue: true } option => option.GetDefaultValue(),
            _ => null
        };

        public IEnumerable<string> GetCompletionsText(CliSymbol symbol, CompletionContext? completionContext = null)
        {
            completionContext ??= CompletionContext.Empty;
            return symbol switch
            {
                CliOption opt => opt.GetCompletions(completionContext)
                            .Select(x => x.Label),
                CliArgument arg => arg.GetCompletions(completionContext)
                            .Select(x => x.Label),
                CliCommand cmd => cmd.GetCompletions(completionContext)
                            .Select(x => x.Label),
                _ => Enumerable.Empty<string>()
            };
        }
        #endregion

        #region Data structure building methods
        // If these methods stay here, we may not need the individual retrieval methods
        public InspectorOptionData GetOptionData(CliOption option, string? parentCommandName)
            => new(GetName(option),
                   GetAliases(option),
                   GetDescription(option),
                   parentCommandName,
                   option.ArgumentHelpName,
                   GetCompletionsText(option),
                   option.ValueType,
                   option.Argument.HasDefaultValue,
                   GetDefaultValue(option),
                   option.Required,
                   option.Hidden);

        public IEnumerable<InspectorOptionData> GetOptionData(CliCommand command)
        {
            return GetOptions(SelfAndParentCommands(command))
                    .Where(a => !a.Hidden)
                    .Select(opt => GetOptionData(opt, GetName(command)));

            static IEnumerable<CliOption> GetOptions(IEnumerable<CliCommand> selfAndParents)
                => selfAndParents
                    .Reverse()
                    .SelectMany(cmd => cmd.Options.Where(a => !a.Hidden))
                    .Distinct();
        }

        public InspectorArgumentData GetArgumentData(CliArgument argument, string? parentCommandName)
            => new(GetName(argument),
                   GetDescription(argument),
                   parentCommandName,
                   GetCompletionsText(argument),
                   argument.ValueType,
                   argument.HasDefaultValue,
                   GetDefaultValue(argument),
                   argument.Hidden);

        public IEnumerable<InspectorArgumentData> GetArgumentData(CliCommand command)
        {
            return GetArguments(SelfAndParentCommands(command))
                     .Where(a => !a.Hidden)
                     .Select(arg => GetArgumentData(arg, GetName(command)));

            static IEnumerable<CliArgument> GetArguments(IEnumerable<CliCommand> selfAndParents)
                => selfAndParents
                    .Reverse()
                    .SelectMany(cmd => cmd.Arguments.Where(a => !a.Hidden))
                    .Distinct();
        }

        public InspectorCommandData GetCommandData(CliCommand command, string? parentCommandName)
            => new(GetName(command),
                   GetAliases(command),
                   GetDescription(command),
                   parentCommandName,
                   GetCompletionsText(command),
                   GetArgumentData(command),
                   GetOptionData(command),
                   GetSubcommandData(command),
                   command.Hidden);

        public IEnumerable<InspectorCommandData> GetSubcommandData(CliCommand command)
        {
            var subCommands = command.Subcommands
                    .Where(a => !a.Hidden)
                    .Select(cmd => GetCommandData(cmd, GetName(command)));
            return subCommands;
        }

        public IEnumerable<CliCommand> SelfAndParentCommands(CliSymbol symbol)
        {
            var ret = new List<CliCommand>();
            CliCommand? current = symbol switch
            {
                CliCommand command => command,
                _ => symbol.Parents.FirstOrDefault() as CliCommand
            }; ;
            while (current is not null)
            {
                ret.Add(current);
                current = current.Parents.FirstOrDefault() as CliCommand;
            }
            return ret;
        }
        #endregion

        #region Semantic/concatenation methods
        // This is not the right place for this as the user may want to customize

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
            _ => string.Empty
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
        #endregion
    }

    public abstract class InspectorSymbolData
    {
        protected InspectorSymbolData(string? name,
                                   string? description,
                                   string? parentCommandName,
                                   IEnumerable<string> completions,
                                   bool hidden )
        {
            Name = name ?? string.Empty;
            Description = description ?? string.Empty;
            ParentCommandName = parentCommandName ?? string.Empty;
            Completions = completions;
            Hidden = hidden;
        }

        public string? Name { get; }
        public string Description { get; }
        public string ParentCommandName { get; }
        public IEnumerable<string> Completions { get; }
        public bool Hidden { get; }
    }

    public class InspectorOptionData : InspectorSymbolData
    {
        public InspectorOptionData(string? name,
                                   IEnumerable<string>? aliases,
                                   string? description,
                                   string? parentCommandName,
                                   string? argumentHelpName,
                                   IEnumerable<string> completions,
                                   Type valueType,
                                   bool hasDefaultValue,
                                   object? defaultValue,
                                   bool required,
                                   bool hidden )
    : base(name, description, parentCommandName, completions, hidden)
        {
            Aliases = aliases ?? Enumerable.Empty<string>();
            ArgumentHelpName = argumentHelpName;
            ValueType = valueType;
            HasDefaultValue = hasDefaultValue;
            DefaultValue = defaultValue;
            Required = required;
        }

        public IEnumerable<string> Aliases { get; }
        public string? ArgumentHelpName { get; }
        public Type ValueType { get; }
        public bool HasDefaultValue { get; }
        public object? DefaultValue { get; }
        public bool Required { get; }
    }

    public class InspectorArgumentData : InspectorSymbolData
    {
        public InspectorArgumentData(string? name,
                                     string? description,
                                     string? parentCommandName,
                                     IEnumerable<string> completions,
                                     Type valueType,
                                     bool hasDefaultValue,
                                     object? defaultValue,
                                     bool hidden )
            : base(name, description, parentCommandName, completions, hidden)
        {
            ValueType = valueType;
            HasDefaultValue = hasDefaultValue;
            DefaultValue = defaultValue;
        }

        public Type ValueType { get; }
        public bool HasDefaultValue { get; }
        public object? DefaultValue { get; }
    }

    public class InspectorCommandData : InspectorSymbolData
    { 
        public InspectorCommandData(string? name,
                                    IEnumerable<string>? aliases,
                                    string? description,
                                    string? parentCommandName,
                                    IEnumerable<string> completions,
                                    IEnumerable<InspectorArgumentData> arguments,
                                    IEnumerable<InspectorOptionData> options,
                                    IEnumerable<InspectorCommandData> subcommands,
                                    bool hidden)
         : base(name, description, parentCommandName, completions,hidden)
        {
            Aliases = aliases ?? Enumerable.Empty<string>();
            Arguments = arguments;
            Options = options;
            Subcommands = subcommands;
        }

        public IEnumerable<string> Aliases { get; }
        public IEnumerable<InspectorArgumentData> Arguments { get; }
        public IEnumerable<InspectorOptionData> Options { get; }
        public IEnumerable<InspectorCommandData> Subcommands { get; }
    }
}
