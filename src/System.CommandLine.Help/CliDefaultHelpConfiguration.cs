using System.Collections;
using System.Collections.Generic;
using System.CommandLine.Help.Formatting;
using System.Linq;

namespace System.CommandLine.Help
{
    public class CliDefaultHelpConfiguration : CliHelpConfiguration
    {
        public CliDefaultHelpConfiguration(CliConfiguration cliConfiguration, CliFormatter? formatter = null, int indent = 0 )
            :base(cliConfiguration, formatter ?? new CliConsoleFormatter(), indent)
        {
            SynopsisSection = new CliHelpSynopsis(this);
            UsageSection = new CliHelpUsage(this);
            ArgumentsSection = new CliHelpArguments(this);
            OptionsSection = new CliHelpOptions(this);
            SubCommandsSection = new CliHelpSubcommands(this);
            Sections.Add(SynopsisSection);
            Sections.Add(UsageSection);
            Sections.Add(ArgumentsSection);
            Sections.Add(OptionsSection);
            Sections.Add(SubCommandsSection);
        }


        public CliHelpSection SynopsisSection { get; set; }
        public CliHelpSection UsageSection { get; set; }
        public CliHelpSection ArgumentsSection { get; set; }
        public CliHelpSection OptionsSection { get; set; }
        public CliHelpSection SubCommandsSection { get; set; }

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
