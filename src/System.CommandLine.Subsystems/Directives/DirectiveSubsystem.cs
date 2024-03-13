// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Subsystems.Annotations;
using System.CommandLine.Subsystems;
using System.Text;
using System.CommandLine.Parsing;

namespace System.CommandLine.Directives;

public abstract class DirectiveSubsystem<T>(string name, string prefix, SubsystemKind kind, IAnnotationProvider? annotationProvider = null)
    : CliSubsystem(prefix, annotationProvider: annotationProvider, kind)
{
    protected CliOption<T> option = DirectiveOption<T>.Create(name);
    public T? Value { get; private set; }
    protected internal override CliConfiguration Initialize(InitializationContext context)
    {
        context.Configuration.RootCommand.Add(option);
        return context.Configuration;
    }

    protected internal override bool GetIsActivated(ParseResult? parseResult)
    {
        if (parseResult is not null && option is not null)
        {
            Value = parseResult.GetValue(option);
        }
        return Value is not null && !Value.Equals(default(T));
    }

    protected internal override CliExit Execute(PipelineContext pipelineContext)
    {
        // TODO: Match testable output pattern
        pipelineContext.ConsoleHack.WriteLine("Output name");
        return CliExit.SuccessfullyHandled(pipelineContext.ParseResult);
    }
}
