using System.CommandLine.Help;
using System.Net.NetworkInformation;

namespace System.CommandLine.Execution
{
    public class CliInvokableConfiguration : CliConfiguration
    {

        public CliInvokableConfiguration(CliCommand rootCommand) : base(rootCommand)
        {
            AddConfiguration( new HelpConfiguration(), rootCommand);
            RootCommand.Options.Add(new VersionOption());
        }
    }
}
