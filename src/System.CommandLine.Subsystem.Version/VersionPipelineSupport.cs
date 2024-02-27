// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace System.CommandLine.Subsystem.Version
{
    public class VersionPipelineSupport() : PipelineSupport<VersionSubsystem>(CategoryAfterValidation)
    {

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

        public override CliExit Execute(PipelineContext pipelineContext)
        {
            var subsystemVersion = Subsystem.Version;
            var version = subsystemVersion is null
                ? CliExecutable.ExecutableVersion
                : subsystemVersion;
            pipelineContext.ConsoleHack.WriteLine(version);
            return new CliExit(pipelineContext.ParseResult, true, 0);
        }
    }
}
