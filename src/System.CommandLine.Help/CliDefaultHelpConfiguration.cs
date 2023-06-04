using System.Collections.Generic;
using System.CommandLine.CliOutput;

namespace System.CommandLine.Help
{
    public class CliDefaultHelpConfiguration : CliHelpConfiguration
    {
        public CliDefaultHelpConfiguration(CliConfiguration cliConfiguration)
            : base(cliConfiguration)
        { 
        Sections.AddRange(new CliSection[]
            {
                new CliHelpSynopsis(),
                new CliHelpUsage(),
                new CliHelpArguments(),
                new CliHelpOptions(),
                new CliHelpSubcommands(),
            });
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
