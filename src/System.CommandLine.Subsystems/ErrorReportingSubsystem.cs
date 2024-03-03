// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.CommandLine.Subsystem;

public class ErrorReportingSubsystem : CliSubsystem
{
    public ErrorReportingSubsystem(IAnnotationProvider? annotationProvider = null)
        : base("ErrorReporting", annotationProvider, SubsystemKind.ErrorReporting)
    { }

    // TODO: Stash option rather than using string
    protected internal override bool GetIsActivated(ParseResult? parseResult)
        => parseResult is not null && parseResult.Errors.Any();

    protected internal override CliExit Execute(PipelineContext pipelineContext)
    {
        pipelineContext.ConsoleHack.WriteLine("You have errors!");
        return CliExit.SuccessfullyHandled(pipelineContext.ParseResult);
    }
}
