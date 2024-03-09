// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using System.CommandLine.Parsing;
using Xunit;

namespace System.CommandLine.Subsystems.Tests;

public class PipelineTests
{
    private static (Pipeline pipeline, CliConfiguration configuration, ConsoleHack consoleHack) StandardObjects(VersionSubsystem versionSubsystem)
    {
        var configuration = new CliConfiguration(new CliRootCommand { new CliOption<bool>("-x") });
        var pipeline = new Pipeline
        {
            Version = versionSubsystem
        };
        var consoleHack = new ConsoleHack().RedirectToBuffer(true);
        return (pipeline, configuration, consoleHack);
    }

    [Theory]
    [ClassData(typeof(TestData.Version))]
    public void Subsystem_runs_in_pipeline_only_when_requested(string input, bool shouldRun)
    {
        var (pipeline, configuration, consoleHack) = StandardObjects(new VersionSubsystem());

        var exit = pipeline.Execute(configuration, input, consoleHack);

        exit.ExitCode.Should().Be(0);
        exit.Handled.Should().Be(shouldRun);
        if (shouldRun)
        {
            consoleHack.GetBuffer().Trim().Should().Be(TestData.AssemblyVersionString);
        }
    }

    [Theory]
    [ClassData(typeof(TestData.Version))]
    public void Subsystem_runs_with_explicit_parse_only_when_requested(string input, bool shouldRun)
    {
        var (pipeline, configuration, consoleHack) = StandardObjects(new VersionSubsystem());

        var result = pipeline.Parse(configuration, input);
        var exit = pipeline.Execute(result, input, consoleHack);

        exit.ExitCode.Should().Be(0);
        exit.Handled.Should().Be(shouldRun);
        if (shouldRun)
        {
            consoleHack.GetBuffer().Trim().Should().Be(TestData.AssemblyVersionString);
        }
    }

    [Theory]
    [ClassData(typeof(TestData.Version))]
    public void Subsystem_runs_initialize_and_teardown_when_requested(string input, bool shouldRun)
    {
        AlternateSubsystems.VersionWithInitializeAndTeardown versionSubsystem = new AlternateSubsystems.VersionWithInitializeAndTeardown();
        var (pipeline, configuration, consoleHack) = StandardObjects(versionSubsystem);

        var exit = pipeline.Execute(configuration, input, consoleHack);

        exit.ExitCode.Should().Be(0);
        exit.Handled.Should().Be(shouldRun);
        versionSubsystem.InitializationWasRun.Should().BeTrue();
        versionSubsystem.ExecutionWasRun.Should().Be(shouldRun);
        versionSubsystem.TeardownWasRun.Should().BeTrue();
    }


    [Theory]
    [ClassData(typeof(TestData.Version))]
    public void Subsystem_works_without_runner(string input, bool shouldRun)
    {
        VersionSubsystem versionSubsystem = new VersionSubsystem();
        var (_, configuration, _) = StandardObjects(versionSubsystem);

        Subsystem.Initialize(versionSubsystem, configuration);
        // This approach might be taken if someone is using a subsystem just for initialization
        var parseResult = CliParser.Parse(configuration.RootCommand, input, configuration);
        bool value = parseResult.GetValue<bool>("--version");

        parseResult.Errors.Should().BeEmpty();
        value.Should().Be(shouldRun);
    }

    [Theory]
    [ClassData(typeof(TestData.Version))]
    public void Subsystem_works_without_runner_style2(string input, bool shouldRun)
    {
        VersionSubsystem versionSubsystem = new VersionSubsystem();
        var (_, configuration, consoleHack) = StandardObjects(versionSubsystem);
        var expectedVersion = shouldRun
                    ? TestData.AssemblyVersionString
                    : "";

        // Someone might use this approach if they wanted to do something with the ParseResult
        Subsystem.Initialize(versionSubsystem, configuration);
        var parseResult = CliParser.Parse(configuration.RootCommand, input, configuration);
        var exit = Subsystem.ExecuteIfNeeded(versionSubsystem, parseResult, input, consoleHack);

        exit.ExitCode.Should().Be(0);
        exit.Handled.Should().Be(shouldRun);
        consoleHack.GetBuffer().Trim().Should().Be(expectedVersion);
    }


    [Theory]
    [InlineData("-xy", false)]
    [InlineData("--versionx", false)]
    public void Subsystem_runs_when_requested_even_when_there_are_errors(string input, bool shouldRun)
    {
        VersionSubsystem versionSubsystem = new VersionSubsystem();
        var (_, configuration, _) = StandardObjects(versionSubsystem);

        Subsystem.Initialize(versionSubsystem, configuration);
        // This approach might be taken if someone is using a subsystem just for initialization
        var parseResult = CliParser.Parse(configuration.RootCommand, input, configuration);
        bool value = parseResult.GetValue<bool>("--version");

        parseResult.Errors.Should().NotBeEmpty();
        value.Should().Be(shouldRun);
    }

    [Fact]
    public void Standard_pipeline_contains_expected_subsystems()
    {
        var pipeline = new StandardPipeline();
        pipeline.Version.Should().BeOfType<VersionSubsystem>();
        pipeline.Help.Should().BeOfType<HelpSubsystem>();
        pipeline.ErrorReporting.Should().BeOfType<ErrorReportingSubsystem>();
        pipeline.Completions.Should().BeOfType<CompletionsSubsystem>();
    }

    [Fact]
    public void Normal_pipeline_contains_no_subsystems()
    {
        var pipeline = new Pipeline();
        pipeline.Version.Should().BeNull();
        pipeline.Help.Should().BeNull();
        pipeline.ErrorReporting.Should().BeNull();
        pipeline.Completions.Should().BeNull();
    }


    [Fact]
    public void Subsystems_can_access_each_others_data()
    {
        // TODO: Explore a mechanism that doesn't require the reference to retrieve data, this shows that it is awkward
        var consoleHack = new ConsoleHack().RedirectToBuffer(true);
        var symbol = new CliOption<bool>("-x");

        var pipeline = new StandardPipeline
        {
            Version = new AlternateSubsystems.VersionThatUsesHelpData(symbol)
        };
        if (pipeline.Help is null) throw new InvalidOperationException();
        var rootCommand = new CliRootCommand
        {
            symbol.With(pipeline.Help.Description, "Testing")
        };
        pipeline.Execute(new CliConfiguration(rootCommand), "-v", consoleHack);
        consoleHack.GetBuffer().Trim().Should().Be($"Testing");
    }
}
