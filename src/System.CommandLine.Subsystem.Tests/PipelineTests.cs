﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using System.CommandLine.Parsing;
using System.Reflection;
using Xunit;

namespace System.CommandLine.Subsystem.Tests
{
    public class PipelineTests
    {

        private static readonly string? version = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly())
                                                 ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                                 ?.InformationalVersion;


        [Fact]
        public void Extension_runs_in_pipeline_when_requested()
        {
            var rootCommand = new CliRootCommand
            {
                new CliOption<bool>("-x")
            };
            var configuration = new CliConfiguration(rootCommand);
            var versionSubsystem = new VersionSubsystem();
            var pipeline = new Pipeline();
            var consoleHack = new ConsoleHack().RedirectToBuffer(true);
            pipeline.Version = versionSubsystem;

            var exit = pipeline.Execute(configuration, "-v", consoleHack);

            exit.ExitCode.Should().Be(0);
            exit.Handled.Should().BeTrue();
            consoleHack.GetBuffer().Trim().Should().Be(version);
        }

        [Fact]
        public void Extension_does_not_run_when_not_requested()
        {
            var rootCommand = new CliRootCommand
            {
                new CliOption<bool>("-x")
            };
            var configuration = new CliConfiguration(rootCommand);
            var versionSubsystem = new VersionSubsystem();
            var pipeline = new Pipeline();
            var consoleHack = new ConsoleHack().RedirectToBuffer(true);
            pipeline.Version = versionSubsystem;
            var input = "-x";

            var result = pipeline.Parse(configuration, input);
            var exit = pipeline.Execute(result, input,consoleHack);

            exit.ExitCode.Should().Be(0);
            exit.Handled.Should().BeFalse();
            consoleHack.GetBuffer().Trim().Should().Be("");

        }

        [Fact]
        public void Extension_does_runs_with_explicit_parse_when_requested()
        {
            var rootCommand = new CliRootCommand
            {
                new CliOption<bool>("-x")
            };
            var configuration = new CliConfiguration(rootCommand);
            var versionSubsystem = new VersionSubsystem();
            var pipeline = new Pipeline();
            var consoleHack = new ConsoleHack().RedirectToBuffer(true);
            pipeline.Version = versionSubsystem;
            var input = "-v";

            var result = pipeline.Parse(configuration, input);
            var exit = pipeline.Execute(result, input, consoleHack);

            exit.ExitCode.Should().Be(0);
            exit.Handled.Should().BeTrue();
            consoleHack.GetBuffer().Trim().Should().Be(version);

        }

        [Fact]
        public void Extension_does_not_run_with_explicit_parse_when_not_requested()
        {
            var rootCommand = new CliRootCommand
            {
                new CliOption<bool>("-x")
            };
            var configuration = new CliConfiguration(rootCommand);
            var versionSubsystem = new VersionSubsystem();
            var pipeline = new Pipeline();
            var consoleHack = new ConsoleHack().RedirectToBuffer(true);
            pipeline.Version = versionSubsystem;
            var input = "-x";

            var result = pipeline.Parse(configuration, input);
            var exit = pipeline.Execute(result, input, consoleHack);

            exit.ExitCode.Should().Be(0);
            exit.Handled.Should().BeFalse();
            consoleHack.GetBuffer().Trim().Should().Be("");

        }

        [Fact]
        public void Extension_can_be_used_without_runner()
        {
            var rootCommand = new CliRootCommand
            { };
            var configuration = new CliConfiguration(rootCommand);
            var versionSubsystem = new VersionSubsystem();
            var consoleHack = new ConsoleHack().RedirectToBuffer(true);
            Subsystem.Initialize(versionSubsystem, configuration);

            // TODO: I do not know why anyone would do this, but I do not see a reason to work to block it
            var parseResult = CliParser.Parse(rootCommand, "-v", configuration);
            var input = "--version";
            if (parseResult.GetValue<bool>(input))
            {
                // TODO: Add an execute overload to avoid checking activated twice
                var exit = Subsystem.ExecuteIfNeeded(versionSubsystem, parseResult, input, consoleHack);
                exit.Should().NotBeNull();
                exit.ExitCode.Should().Be(0);
                exit.Handled.Should().BeTrue();
            }

            consoleHack.GetBuffer().Trim().Should().Be(version);
        }


        [Fact]
        public void Extension_can_be_used_without_runner_style2()
        {
            var rootCommand = new CliRootCommand
            { };
            var configuration = new CliConfiguration(rootCommand);
            var versionSubsystem = new VersionSubsystem();
            var consoleHack = new ConsoleHack().RedirectToBuffer(true);
            Subsystem.Initialize(versionSubsystem, configuration);
            var input = "-v";

            var parseResult = CliParser.Parse(rootCommand, input, configuration);
            var exit = Subsystem.ExecuteIfNeeded(versionSubsystem, parseResult, input, consoleHack);

            exit.ExitCode.Should().Be(0);
            exit.Handled.Should().BeTrue();
            consoleHack.GetBuffer().Trim().Should().Be(version);
        }
    }
}