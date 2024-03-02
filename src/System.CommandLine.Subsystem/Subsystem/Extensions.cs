// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.CommandLine.Subsystem;

public static class Extensions
{
    public static PipelineContext ExecuteSubSystemIfNeeded(this PipelineContext? pipelineContext, CliSubsystem? subsystem)
        => subsystem is null
            ? pipelineContext ?? new PipelineContext(null, "")
            : subsystem.ExecuteIfNeeded(pipelineContext ?? new PipelineContext(null,""));

    public static CliConfiguration InitializeSubsystem(this CliConfiguration configuration, CliSubsystem? subsystem)
        => subsystem is null
            ? configuration
            : subsystem.Initialize(configuration);

    public static PipelineContext TeardownSubsystem(this PipelineContext pipelineContext, CliSubsystem? subsystem)
    => subsystem is null
        ? pipelineContext
        : subsystem.TearDown(pipelineContext);
}
