using System.CommandLine.Help;

namespace System.CommandLine.Execution
{
    public class CliInvokableConfiguration : CliConfiguration
    {

        public CliInvokableConfiguration(CliCommand rootCommand) : base(rootCommand)
        {

            RootCommand.Options.Add(new HelpOption());
            RootCommand.Options.Add(new VersionOption());
        }
    }
}
