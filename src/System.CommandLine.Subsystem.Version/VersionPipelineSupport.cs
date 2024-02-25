
namespace System.CommandLine.Subsystem.Version
{
    public class VersionPipelineSupport : PipelineSupport
    {
        public VersionPipelineSupport(): base(CategoryAfterValidation)
        { }


        public override bool Initialization(CliConfiguration configuration, IReadOnlyList<string> arguments, string rawInput)
        {
            var option = new CliOption<bool>("--version", ["-v"])
            {
                Arity = ArgumentArity.Zero
            };
            configuration.RootCommand.Add(option);

            return true;
        }

        public override bool GetIsActivated(ParseResult parseResult)
            => parseResult.GetValue<bool>("--version");

        // TODO: Determine how we test console output
        public bool TempFlagForTest = false;
        public override CliExit Execute(ParseResult parseResult)
        {
            // TODO: Match testable output pattern
            Console.WriteLine(CliExecutable.ExecutableVersion);
            TempFlagForTest = true;
            return new CliExit(parseResult, true, 0);
        }
    }
}
