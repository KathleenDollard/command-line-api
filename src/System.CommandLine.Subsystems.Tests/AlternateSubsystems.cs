﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Subsystems;

namespace System.CommandLine.Subsystems.Tests
{
    internal class AlternateSubsystems
    {
        internal class AlternateVersion : VersionSubsystem
        {
            protected override CliExit Execute(PipelineContext pipelineContext)
            {
                pipelineContext.ConsoleHack.WriteLine($"***{CliExecutable.ExecutableVersion}***");
                pipelineContext.AlreadyHandled = true;
                return CliExit.SuccessfullyHandled(pipelineContext.ParseResult);
            }
        }

        internal class VersionThatUsesHelpData : VersionSubsystem
        {
            // for testing, this class accepts a symbol and accesses its description

            public VersionThatUsesHelpData(CliSymbol symbol)
            {
                Symbol = symbol;
            }

            private CliSymbol Symbol { get; }

            protected override CliExit Execute(PipelineContext pipelineContext)
            {
                var help = pipelineContext.Pipeline.Help ?? throw new InvalidOperationException("Help cannot be null for this subsystem to work");
                var data = help.GetDescription(Symbol);

                pipelineContext.ConsoleHack.WriteLine(data);
                pipelineContext.AlreadyHandled = true;
                return CliExit.SuccessfullyHandled(pipelineContext.ParseResult);
            }



        }
        internal class VersionWithInitializeAndTeardown : VersionSubsystem
        {
            internal bool InitializationWasRun;
            internal bool ExecutionWasRun;
            internal bool TeardownWasRun;

            protected override CliConfiguration Initialize(CliConfiguration configuration)
            {
                // marker hack needed because ConsoleHack not available in initialization
                InitializationWasRun = true;
                return base.Initialize(configuration);
            }

            protected override CliExit Execute(PipelineContext pipelineContext)
            {
                ExecutionWasRun = true;
                return base.Execute(pipelineContext);
            }

            protected override CliExit TearDown(CliExit cliExit)
            {
                TeardownWasRun = true;
                return base.TearDown(cliExit);
            }
        }

    }
}