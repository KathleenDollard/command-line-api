﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace System.CommandLine.Subsystem.Version
{
    public class VersionPipelineSupport() : PipelineSupport(CategoryAfterValidation)
    {
        internal VersionSubsystem VersionSubsystem { get; set; }

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
            var version = VersionSubsystem.Version is null
                ? CliExecutable.ExecutableVersion
                : VersionSubsystem.Version;
            pipelineContext.ConsoleHack.WriteLine(version);
            return new CliExit(pipelineContext.ParseResult, true, 0);
        }
    }
}
