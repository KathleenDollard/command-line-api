using System.Collections;
using System.Collections.Generic;
using System.CommandLine.CliOutput;
using System.Linq;

namespace System.CommandLine.Help
{
    public abstract class CliHelpConfiguration
    {
        public CliHelpConfiguration(CliConfiguration cliConfiguration)
        {
            CliConfiguration = cliConfiguration;
            GetSymbolInspector = helpContext => new CliSymbolInspector(helpContext);
        }


        public CliConfiguration CliConfiguration { get; }


        public abstract IEnumerable<CliSection> GetSections(HelpContext helpContext);

        public Func<HelpContext, CliSymbolInspector> GetSymbolInspector { get; set; }
  
        public Func<ParseResult, CliFormatter>? GetFormatter { get; set; }

        public static class Defaults
        {
            public static string? AliasText(IEnumerable<string> aliases)
            => string.Join(", ", aliases);


            public static string DefaultValueTextAndLabel(
                CliArgument argument,
                bool displayArgumentName)
            => DefaultValueText(argument.GetDefaultValue()) switch 
            { 
                null => string.Empty,
                string s when (string.IsNullOrWhiteSpace(s)) => string.Empty,
                string s => $"{DefaultValueLabel(argument.Name, displayArgumentName)}: {s}",
            };


            public static string DefaultValueLabel(
                string name,
                bool displayArgumentName)
            => displayArgumentName
                      ? name
                      : LocalizationResources.HelpArgumentDefaultValueLabel();

            public static string DefaultValueText(
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
}
