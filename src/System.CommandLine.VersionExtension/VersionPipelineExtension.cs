using System.CommandLine.Pipeline;

namespace System.CommandLine.VersionExtension
{
    public class VersionPipelineExtension : PipelineExtension
    {
        public VersionPipelineExtension(): base(CategoryAfterValidation)
        { }


        public override bool BeforeParsing(CliConfiguration configuration, IReadOnlyList<string> arguments, string rawInput)
        {
            var option = new CliOption<bool>("--version", ["-v"])
            {
                Arity = ArgumentArity.Zero
            };
            configuration.RootCommand.Add(option);

            return true;
        }

        public override bool GetIsActivated(ParseResult result)
            => result.GetValue<bool>("--version");

        // TODO: Determine how we test console output
        public bool TempFlagForTest = false;
        public override bool Execute(ParseResult result)
        {
            // TODO: Match testable output pattern
            Console.WriteLine(CliExecutable.ExecutableVersion);
            TempFlagForTest = true;
            return true;
        }
    }
}
