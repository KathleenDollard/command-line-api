// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Subsystems;

namespace System.CommandLine;

public class CompletionsSubsystem : CliSubsystem
{
    public CompletionsSubsystem(IAnnotationProvider? annotationProvider = null)
        : base("Completions", annotationProvider, SubsystemKind.Completions)
    { }

    // TODO: Figure out trigger for completions
    protected internal override bool GetIsActivated(ParseResult? parseResult)
        => parseResult is null
            ? false
            : false;

    protected internal override CliExit Execute(PipelineContext pipelineContext)
    {
        pipelineContext.ConsoleHack.WriteLine("Not yet implemented");
        return CliExit.SuccessfullyHandled(pipelineContext.ParseResult);
    }
}
