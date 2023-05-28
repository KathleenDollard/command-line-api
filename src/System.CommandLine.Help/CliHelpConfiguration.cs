using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine.Help
{
    public class CliHelpConfiguration
    {
        public CliHelpConfiguration(int indent = 0)
        {
            Indent = indent == 0 ? 2 : indent;
            SynopsisSection = new CliHelpSubcommands(this);
            UsageSection = new CliHelpUsage(this);
            ArgumentsSection = new CliHelpArguments(this);
            OptionsSection = new CliHelpOptions(this);
            SubCommandsSection = new CliHelpSubcommands(this);
        }


        public int Indent { get; }

        public CliHelpSection<CliCommand> SynopsisSection { get; set; }
        public CliHelpSection<CliCommand> UsageSection { get; set; }
        public CliHelpSection<CliArgument> ArgumentsSection { get; set; }
        public CliHelpSection<CliOption> OptionsSection { get; set; }
        public CliHelpSection<CliCommand> SubCommandsSection { get; set; }

        public virtual IEnumerable<CliHelpSection> GetSections()
            => new List<CliHelpSection>()
            {
                SynopsisSection,
                UsageSection,
                ArgumentsSection,
                OptionsSection,
                SubCommandsSection
            };

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
