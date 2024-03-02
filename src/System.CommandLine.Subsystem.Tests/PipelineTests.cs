// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using System.CommandLine.Parsing;
using System.Reflection;
using Xunit;

namespace System.CommandLine.Subsystem.Tests
{
    public class PipelineTests
    {

        private static readonly string version = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly())
                                                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                                .InformationalVersion;


        [Fact]
        public void Extension_runs_in_pipeline_when_requested()
        {
            var rootCommand = new CliRootCommand
            {
                new CliOption<bool>("-x")
            };
            var configuration = new CliConfiguration(rootCommand);
            var versionSubsystem = new Version();
            var pipeline = new Pipeline();
            var consoleHack = new ConsoleHack().RedirectToBuffer(true);
            pipeline.AddExtension(versionSubsystem);

            var exit = pipeline.Execute(configuration, "-v", consoleHack);

            exit.ExitCode.Should().Be(0);
            exit.Handled.Should().BeTrue();
            consoleHack.GetBuffer().Trim().Should().Be(version);
        }

        // TODO: Does this test do what it says it does
        [Fact]
        public void Extension_does_not_runs_with_explicit_parse_when_requested()
        {
            var rootCommand = new CliRootCommand
            {
                new CliOption<bool>("-x")
            };
            var configuration = new CliConfiguration(rootCommand);
            var versionSubsystem = new Version();
            var pipeline = new Pipeline();
            var consoleHack = new ConsoleHack().RedirectToBuffer(true);
            pipeline.AddExtension(versionSubsystem);

            var result = pipeline.Parse(configuration, "-v");
            var exit = pipeline.Execute(result, consoleHack);

            exit.ExitCode.Should().Be(0);
            exit.Handled.Should().BeTrue();
            consoleHack.GetBuffer().Trim().Should().Be(version);

        }

        // TODO: Does this test do what it says it does
        [Fact]
        public void Extension_does_not_runs_when_not_requested()
        {
            var rootCommand = new CliRootCommand
            {
                new CliOption<bool>("-x")
            };
            var configuration = new CliConfiguration(rootCommand);
            var versionSubsystem = new Version();
            var pipeline = new Pipeline();
            var consoleHack = new ConsoleHack().RedirectToBuffer(true);
            pipeline.AddExtension(versionSubsystem);

            var result = pipeline.Parse(configuration, "-v");
            var exit = pipeline.Execute(result, consoleHack);

            exit.ExitCode.Should().Be(0);
            exit.Handled.Should().BeTrue();
            consoleHack.GetBuffer().Trim().Should().Be(version);

        }

        [Fact]
        public void Extension_can_be_used_without_runner()
        {
            var rootCommand = new CliRootCommand
            { };
            var configuration = new CliConfiguration(rootCommand);
            var versionSubsystem = new Version();
            var consoleHack = new ConsoleHack().RedirectToBuffer(true);
            Subsystem.Initialize(versionSubsystem,configuration);

            var parseResult = CliParser.Parse(rootCommand, "-v", configuration);
            CliExit? exit = null;
            if (parseResult.GetValue<bool>("--version"))
            {
                // TODO: Add an execute overload to avoid checking activated twice
                exit = Subsystem.ExecuteIfNeeded(versionSubsystem, parseResult, consoleHack);
            }

            exit.Should().NotBeNull();
            exit.ExitCode.Should().Be(0);
            exit.Handled.Should().BeTrue();
            consoleHack.GetBuffer().Trim().Should().Be(version);
        }


        [Fact]
        public void Extension_can_be_used_without_runner_style2()
        {
            var rootCommand = new CliRootCommand
            { };
            var configuration = new CliConfiguration(rootCommand);
            var versionSubsystem = new Version();
            var consoleHack = new ConsoleHack().RedirectToBuffer(true);
            Subsystem.Initialize(versionSubsystem, configuration);

            var parseResult = CliParser.Parse(rootCommand, "-v", configuration);
            var exit = Subsystem.ExecuteIfNeeded(versionSubsystem, parseResult, consoleHack);

            exit.ExitCode.Should().Be(0);
            exit.Handled.Should().BeTrue();
            consoleHack.GetBuffer().Trim().Should().Be(version);
        }
    }
}
