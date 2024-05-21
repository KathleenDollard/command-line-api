﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Directives;
using System.CommandLine.Subsystems.Annotations;

namespace System.CommandLine.Subsystems.Tests
{
    internal class AlternateSubsystems
    {
        internal class AlternateVersion : VersionSubsystem
        {
            protected override CliExit Execute(PipelineResult pipelineContext)
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

            protected override CliExit Execute(PipelineResult pipelineContext)
            {
                TryGetAnnotation(Symbol, HelpAnnotations.Description, out string? description);
                pipelineContext.ConsoleHack.WriteLine(description);
                pipelineContext.AlreadyHandled = true;
                return CliExit.SuccessfullyHandled(pipelineContext.ParseResult);
            }
        }

        internal class VersionWithInitializeAndTeardown : VersionSubsystem
        {
            internal bool InitializationWasRun;
            internal bool ExecutionWasRun;
            internal bool TeardownWasRun;

            protected override CliConfiguration Initialize(InitializationContext context)
            {
                // marker hack needed because ConsoleHack not available in initialization
                InitializationWasRun = true;
                return base.Initialize(context);
            }

            protected override CliExit Execute(PipelineResult pipelineContext)
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

        internal class StringDirectiveSubsystem(IAnnotationProvider? annotationProvider = null)
           : DirectiveSubsystem("other", SubsystemKind.Diagram, annotationProvider)
        { }

        internal class BooleanDirectiveSubsystem(IAnnotationProvider? annotationProvider = null)
           : DirectiveSubsystem("diagram", SubsystemKind.Diagram, annotationProvider)
        { }

    }
}
