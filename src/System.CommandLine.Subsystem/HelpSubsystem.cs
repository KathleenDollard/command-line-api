// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Extended.Annotations;
using System.CommandLine.Subsystem.System.CommandLine.Extended.Annotations;

namespace System.CommandLine.Extended;

// stub Help subsystem demonstrating annotation model.
//
// usage:
//
//
//        var help = new HelpSubsystem();
//        var command = new CliCommand("greet")
//          .With(help.Description, "Greet the user");
//
internal class HelpSubsystem(IAnnotationProvider? annotationProvider = null) : CliSubsystem(annotationProvider)
{
    public void SetDescription(CliSymbol symbol, string description) => SetAnnotation(symbol, HelpAnnotations.Description, description);

    public AnnotationAccessor<string> Description => new(this, HelpAnnotations.Description);
}