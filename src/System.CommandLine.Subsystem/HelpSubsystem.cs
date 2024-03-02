// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Subsystem.Annotations;
using System.CommandLine.Subsystem;

namespace System.CommandLine;

// stub Help subsystem demonstrating annotation model.
//
// usage:
//
//
//        var help = new HelpSubsystem();
//        var command = new CliCommand("greet")
//          .With(help.Description, "Greet the user");
//
public class HelpSubsystem(IAnnotationProvider? annotationProvider = null) 
    : CliSubsystem(HelpAnnotations.Prefix, annotationProvider: annotationProvider, SubsystemKind.Help)
{
    public void SetDescription(CliSymbol symbol, string description) 
        => SetAnnotation(symbol, HelpAnnotations.Description, description);

    public AnnotationAccessor<string> Description 
        => new(this, HelpAnnotations.Description);

    protected internal override CliConfiguration Initialize(CliConfiguration configuration)
    {
        var option = new CliOption<bool>("--help", ["-h"])
        {
            Arity = ArgumentArity.Zero
        };
        configuration.RootCommand.Add(option);

        return configuration;
    }

    protected internal override bool GetIsActivated(ParseResult? parseResult)
        => parseResult is not null && parseResult.GetValue<bool>("--help");

    protected internal override CliExit Execute(PipelineContext pipelineContext)
    {
        // TODO: Match testable output pattern
        pipelineContext.ConsoleHack.WriteLine("Help me!");
        return CliExit.SuccessfullyHandled(pipelineContext.ParseResult);
    }
}