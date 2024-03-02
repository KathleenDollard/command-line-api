// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using System.Reflection;
using Xunit;

namespace System.CommandLine.Subsystem.Tests;

public class StandardPipelineTests
{

    private static readonly string? version = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly())
                                             ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                             ?.InformationalVersion;


    [Fact]
    public void Standard_pipeline_includes_expected_subsystems()
    {
        var pipeline = new StandardPipeline();

        pipeline.Help.Should().BeOfType<HelpSubsystem>();
        pipeline.Version.Should().BeOfType<VersionSubsystem>();
        pipeline.ErrorReporting.Should().BeOfType<ErrorReportingSubsystem>();
        pipeline.Completions.Should().BeOfType<CompletionsSubsystem>();
    }
}
