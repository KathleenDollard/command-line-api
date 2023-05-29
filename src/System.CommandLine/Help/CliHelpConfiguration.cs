using System.Collections;
using System.Collections.Generic;
using System.CommandLine.Help.Formatting;
using System.Linq;
using System.CommandLine.Help.Formatting;

namespace System.CommandLine.Help
{
    public class CliHelpConfiguration
    {
        public CliHelpConfiguration(CliConfiguration cliConfiguration, CliFormatter formatter, int indent = 0 )
        {
            Indent = indent == 0 ? 2 : indent;
            CliConfiguration = cliConfiguration;
            Formatter = formatter;
            Sections = new List<CliHelpSection>();
        }


        public int Indent { get; }
        public CliConfiguration CliConfiguration { get; }
        public CliFormatter Formatter { get; }
        public List<CliHelpSection> Sections { get; }
  

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
