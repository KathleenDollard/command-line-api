// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.CommandLine.Subsystem;

public class CompletionsSubsystem : CliSubsystem
{
    public CompletionsSubsystem(IAnnotationProvider? annotationProvider = null)
        : base("Completions", annotationProvider, SubsystemKind.Completions)
    { }

    // TODO: Stash option rather than using string
    protected internal override bool GetIsActivated(ParseResult parseResult)
        => parseResult.Errors.Any();

    protected internal override CliExit Execute(PipelineContext pipelineContext)
    {
        pipelineContext.ConsoleHack.WriteLine("Not yet implemented");
        return CliExit.SuccessfullyHandled(pipelineContext.ParseResult);
    }
}
