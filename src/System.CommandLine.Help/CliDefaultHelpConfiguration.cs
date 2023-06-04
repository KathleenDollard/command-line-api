using System.Collections.Generic;
using System.CommandLine.CliOutput;

namespace System.CommandLine.Help
{
    public class CliDefaultHelpConfiguration : CliHelpConfiguration
    {
        public CliDefaultHelpConfiguration(CliConfiguration cliConfiguration, CliFormatter? formatter = null, int indent = 0)
            : base(cliConfiguration)
        { }


        public override IEnumerable<CliSection> GetSections(HelpContext helpContext)
        {
            var configuration = helpContext.CliConfiguration.HelpConfiguration;
            var symbolInspector = configuration.GetSymbolInspector(helpContext);

            var formatter = helpContext.Formatter;

            return new List<CliHelpSection>()
            {
                new CliHelpSynopsis(this, symbolInspector, formatter),
                new CliHelpUsage(this,symbolInspector, formatter),
                new CliHelpArguments(this,symbolInspector, formatter),
                new CliHelpOptions(this,symbolInspector, formatter),
                new CliHelpSubcommands(this,symbolInspector, formatter),
            };
        }

        //public static class Defaults
        //{
        //    public static string? AliasText(IEnumerable<string> aliases)
        //    => string.Join(", ", aliases);


        //    public static string DefaultValueTextAndLabel(
        //        CliArgument argument,
        //        bool displayArgumentName)
        //    => DefaultValueText(argument.GetDefaultValue()) switch
        //    {
        //        null => string.Empty,
        //        string s when (string.IsNullOrWhiteSpace(s)) => string.Empty,
        //        string s => $"{DefaultValueLabel(argument.Name, displayArgumentName)}: {s}",
        //    };


        //    public static string DefaultValueLabel(
        //        string name,
        //        bool displayArgumentName)
        //    => displayArgumentName
        //              ? name
        //              : LocalizationResources.HelpArgumentDefaultValueLabel();

        //    public static string DefaultValueText(object? defaultValue)
        //    => defaultValue switch
        //    {
        //        null => string.Empty,
        //        string s => s,
        //        IEnumerable enumerable => string.Join("|", enumerable.OfType<object>().ToArray()),
        //        _ => defaultValue.ToString()
        //    };
        //}
    }
}
