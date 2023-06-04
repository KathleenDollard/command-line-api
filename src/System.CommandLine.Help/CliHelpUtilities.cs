using System.Collections;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.Linq;

namespace System.CommandLine.Help
{
    public static class CliHelpUtilities
    {
        // KAD: perf question: Is it worth the statics here for the caching. We cannot guarantee that 
        //      GetSymbolInspector will be fast, and this is called about 6 times for every call to help
        private static HelpContext? previousHelpContext = null;
        private static CliSymbolInspector? symbolInspector = null;

        public static CliSymbolInspector SymbolInspector(HelpContext helpContext)
        {
            if (helpContext != previousHelpContext || symbolInspector is null)
            {
                previousHelpContext = helpContext;
                symbolInspector = previousHelpContext is null
                    ? throw new ArgumentException("Argument should be a HelpContext", nameof(helpContext))
                    : previousHelpContext.CliConfiguration.HelpConfiguration.GetSymbolInspector(previousHelpContext);
            }
            return symbolInspector;
        }

        // This is an attempt to move these out of the SymbolInspector as they are not absolutes
        // KAD: TODO: Once tests pass, find commonalities in these GetUsage and GetDefaultValue methods
        public static string GetUsage(InspectorOptionData data, bool showUsageOnBool = false, bool skipNameDefault = false)
        {
            var text = GetAliasText(data.Name ?? string.Empty, data.Aliases);
            var argumentLabel = GetArgUsage(data.ArgumentHelpName, data.Name, data.ValueType, data.Completions, showUsageOnBool: false, skipNameDefault: true );
            if (!string.IsNullOrEmpty(argumentLabel))
            {
                text += $" {argumentLabel}";
            }
            if (data.Required)
            {
                text += $" {LocalizationResources.HelpOptionsRequiredLabel()}";
            }

            return text;

            static string defaultReturn(string parentCommandName, bool skipNameDefault)
                => skipNameDefault ? "" : $"<{parentCommandName}>";

        }


        public static string GetUsage(InspectorArgumentData data, bool showUsageOnBool = false, bool skipNameDefault = false)
        {
            return GetArgUsage(null, data.Name, data.ValueType, data.Completions, showUsageOnBool: true);
            //if (!string.IsNullOrWhiteSpace(data.Name))
            //{
            //    return $"<{data.Name}>";
            //}

            //var valueType = data.ValueType;
            //// never show true/false
            //if (valueType == typeof(bool) || valueType == typeof(bool?))
            //{
            //    return showUsageOnBool
            //        ? defaultReturn(data.ParentCommandName, showUsageOnBool)
            //        : string.Empty;
            //}

            //var completions = data.Completions;
            //if (completions.Any())
            //{
            //    string joined = string.Join("|", completions);

            //    if (!string.IsNullOrEmpty(joined))
            //    {
            //        return $"<{joined}>";
            //    }
            //}
            //return defaultReturn(data.ParentCommandName, skipNameDefault);

            //static string defaultReturn(string parentCommandName, bool skipNameDefault)
            //    => skipNameDefault ? "" : $"<{parentCommandName}>";
        }

        public static string GetUsage(InspectorCommandData data, bool showUsageOnBool = false, bool skipNameDefault = false)
        {

            var text = GetAliasText(data.Name ?? string.Empty, data.Aliases);
            string argumentText = string.Empty;

            if (data.Arguments.Any())
            {
                var argumentLabels = data.Arguments
                    .Where(arg => !arg.Hidden)
                    .Select(arg => GetArgUsage(arg.Name, data.Name, arg.ValueType,data.Completions, false));
                argumentText = string.Join(", ", argumentLabels);
            }

            return string.IsNullOrEmpty(argumentText)
                ? text
                : $"{text} {argumentText}";

            //if (!string.IsNullOrWhiteSpace(data.Name))
            //{
            //    return $"<{data.Name}>";
            //}

            //var completions = data.Completions;
            //if (completions.Any())
            //{
            //    string joined = string.Join("|", completions);

            //    if (!string.IsNullOrEmpty(joined))
            //    {
            //        return $"<{joined}>";
            //    }
            //}
            //return defaultReturn(data.ParentCommandName, skipNameDefault);

            //static string defaultReturn(string parentCommandName, bool skipNameDefault)
            //    => skipNameDefault ? "" : $"<{parentCommandName}>";
        }

        /// <summary>
        /// Given a list of aliases, provide a consistent display
        /// </summary>
        /// <param name="name">The name of the symbol</param>
        /// <param name="aliases">The aliases.</param>
        /// <remarks>
        /// The expected customization here is to layout the aliases differently, such as a different delimiter.
        /// </remarks>
        /// <returns>A string with the aliases sorted, distinct and comma delimited</returns>
        public static string GetAliasText(string name, IEnumerable<string> aliases)
        {
            var allAliases = aliases is null || !aliases.Any()
                ? new[] { name }
                : new[] { name }.Concat(aliases)
                    .Select(r => r.SplitPrefix())
                    .OrderBy(r => r.Prefix, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(r => r.Alias, StringComparer.OrdinalIgnoreCase)
                    .GroupBy(t => t.Alias)
                    .Select(t => t.First())
                    .Select(t => $"{t.Prefix}{t.Alias}");
            return string.Join(", ", allAliases);
        }

        private static string GetArgUsage(string? helpName, string? symbolName, Type valueType, IEnumerable<string> completions, bool showUsageOnBool, bool skipNameDefault = false)
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
                if (completions.Any())
                {
                    string joined = string.Join("|", completions);

                    if (!string.IsNullOrEmpty(joined))
                    {
                        return $"<{joined}>";
                    }
                }
            }

            return skipNameDefault ? "" : $"<{symbolName}>";
        }

        public static string GetDefaultValueText(InspectorOptionData data, bool displaySymbolName = false)
        {
            var defaultValueText = data.DefaultValue switch
            {
                null => string.Empty,
                string s => s,
                IEnumerable enumerable => string.Join("|", enumerable.OfType<object>().ToArray()),
                _ => data.DefaultValue.ToString()
            };

            return string.IsNullOrWhiteSpace(defaultValueText)
                ? ""
                : $"{GetLabel(data.Name, displaySymbolName)}: {defaultValueText}";

            string GetLabel(string? name, bool displayArgumentName) =>
               displayArgumentName
                 ? name ?? string.Empty
                 : LocalizationResources.HelpArgumentDefaultValueLabel();
        }


        public static string GetDefaultValueText(InspectorArgumentData data, bool displaySymbolName = false)
        {
            var defaultValueText = data.DefaultValue switch
            {
                null => string.Empty,
                string s => s,
                IEnumerable enumerable => string.Join("|", enumerable.OfType<object>().ToArray()),
                _ => data.DefaultValue.ToString()
            };

            return string.IsNullOrWhiteSpace(defaultValueText)
                ? ""
                : $"{GetLabel(data.Name, displaySymbolName)}: {defaultValueText}";

            string GetLabel(string? name, bool displayArgumentName) =>
               displayArgumentName
                 ? name ?? string.Empty
                 : LocalizationResources.HelpArgumentDefaultValueLabel();
        }

        public static string GetDefaultValueText(InspectorCommandData data, bool displaySymbolName = false)
        {
            var args = data.Arguments
                .Where(arg => !arg.Hidden && arg.HasDefaultValue);

            return args.Count() switch
            {
                0 => "",
                1 => $"[{DefaultValueTextAndLabel(args.First(), false)}]",
                _ => $"[{string.Join(", ", args.Select(arg => GetDefaultValueText(arg, true)))}]"
            };


            string GetLabel(string? name, bool displayArgumentName) =>
               displayArgumentName
                 ? name ?? string.Empty
                 : LocalizationResources.HelpArgumentDefaultValueLabel();
        }

        private static string DefaultValueTextAndLabel(InspectorArgumentData argData, bool displayArgumentName)
        => DefaultValueText(argData.DefaultValue) switch
        {
            null => string.Empty,
            string s when (string.IsNullOrWhiteSpace(s)) => string.Empty,
            string s => $"{DefaultValueLabel(argData.Name, displayArgumentName)}: {s}",
        };

        private static string DefaultValueLabel(
            string? name,
            bool displayArgumentName)
        => displayArgumentName
                  ? name ?? string.Empty
                  : LocalizationResources.HelpArgumentDefaultValueLabel();

        public static string? DefaultValueText(
            object? defaultValue)
       => defaultValue switch
       {
           null => string.Empty,
           string s => s,
           IEnumerable enumerable => string.Join("|", enumerable.OfType<object>().ToArray()),
           _ => defaultValue.ToString()
       };
    }
}
