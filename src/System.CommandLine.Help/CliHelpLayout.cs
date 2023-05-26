using System.Collections.Generic;

namespace System.CommandLine.Help
{
    public class CliHelpLayout
    {
        public CliHelpLayout(CliHelpConfiguration helpConfiguration)
        {
            HelpConfiguration = helpConfiguration;
        }
        public CliHelpConfiguration HelpConfiguration { get; }

        public IEnumerable<CliHelpSection> GetSections(HelpContext helpContext)
        {
            // KAD: Changed to classes because it is an open set
            yield return new CliHelpSynopsis(HelpConfiguration, helpContext);
            yield return new CliHelpUsage(HelpConfiguration, helpContext);
            yield return new CliHelpArguments(HelpConfiguration, helpContext);
            yield return new CliHelpOptions(HelpConfiguration, helpContext);
            yield return new CliHelpSubcommand(HelpConfiguration, helpContext);
            //yield return new CliAdditionalText(HelpConfiguration, helpContext);
        }

        //public virtual void WriteHeading(string? heading, string? description)
        //{
        //    if (!string.IsNullOrWhiteSpace(heading))
        //    {
        //        Writer.WriteLine(heading);
        //    }
        //    if (!string.IsNullOrWhiteSpace(description))
        //    {
        //        int indent = CliHelpFormatting.Indent;
        //        int maxWidth = HelpConfiguration.MaxWidth - indent;
        //        foreach (var part in CliHelpHelpers.WrapText(description!, maxWidth))
        //        {
        //            Writer.Write(indent);
        //            Writer.WriteLine(part);
        //        }
        //    }
        //}


    }
}
