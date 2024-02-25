// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;

namespace System.CommandLine.Extended.Tests
{
    public class RunnerTests
    {

        private void Sample_1()
        {
            var rootCommand = new CliRootCommand
            { };
            var configuration = new CliConfiguration(rootCommand);
            // TODO: Throughout consider Add returning the thing that was added. This would avoid the extra line here
            var versionOption = new VersionExtension.VersionExtension();
            var pipeline = new Pipeline.Pipeline();
            pipeline.AddExtension(new VersionExtension.VersionExtension());
            var result = pipeline.Parse(configuration, "");

            var handled = versionOption.PipelineExtension.ExecuteIfNeeded(result.ParseResult);

            handled.Should().BeTrue();
            versionOption.PipelineExtension.TempFlagForTest.Should().BeTrue();

        }
        /*
        private void Sample_2()
        {
            var rootCommand = new CliRootCommand
            { };
            var configuration = new CliConfiguration(rootCommand);
            var pipeline = new Pipeline.Pipeline();
            pipeline.AddExtension(new VersionExtension.VersionExtension());
            var result = pipeline.Parse(configuration, "");
            var runner = new Runner();
            var result = CliParser.Parse(rootCommand, "-v", configuration);
            var handled = runner.Execute(result);

            handled.Should().BeTrue();

        }


        private void Sample_3()
        {
            var rootCommand = new CliRootCommand
            { };
            var configuration = new CliConfiguration(rootCommand);
            configuration.AddExtension(new VersionExtension.VersionExtension());
            var handled = Runner.Execute(rootCommand, "-v", configuration);

            handled.Should().BeTrue();

        }


        [Fact]
        public void Extension_runs_when_requested()
        {
            var rootCommand = new CliRootCommand
            {
                new CliOption<bool>("-x")
            };
            var configuration = new CliConfiguration(rootCommand);
            var versionOption = new VersionExtension.VersionExtension();
            configuration.AddExtension(versionOption);
            Runner runner = new Runner();
            configuration.AddExtension(runner);
            var result = CliParser.Parse(rootCommand, "-v", configuration);
            runner.Execute(result);

            versionOption.TempFlagForTest.Should().BeTrue();

        }

        [Fact(Skip ="Bug that causes recursion")]
        public void Extension_does_not_runs_when_not_requested()
        {
            var rootCommand = new CliRootCommand
            {
                new CliOption<bool>("-x")
            };
            var configuration = new CliConfiguration(rootCommand);
            var versionOption = new VersionExtension.VersionExtension();
            configuration.AddExtension(versionOption);
            Runner runner = new Runner();
            configuration.AddExtension(runner);
            var result = CliParser.Parse(rootCommand, "-x", configuration);
            runner.Execute(result);

            versionOption.TempFlagForTest.Should().BeFalse();

        }


        [Fact]
        public void Extension_can_be_used_without_runner()
        {
            var rootCommand = new CliRootCommand
            { };
            var configuration = new CliConfiguration(rootCommand);
            var versionOption = new VersionExtension.VersionExtension();
            configuration.AddExtension(versionOption);
            var result = CliParser.Parse(rootCommand, "-v", configuration);

            if (versionOption.GetIsActivated(result))
            {
                versionOption.Execute(result);
            }

            versionOption.TempFlagForTest.Should().BeTrue();

        }


        [Fact]
        public void Extension_can_be_used_without_runner_style2()
        {
            var rootCommand = new CliRootCommand
            { };
            var configuration = new CliConfiguration(rootCommand);
            var versionOption = new VersionExtension.VersionExtension();
            configuration.AddExtension(versionOption);
            var result = CliParser.Parse(rootCommand, "-v", configuration);

            var handled = versionOption.ExecuteIfNeeded(result);

            handled.Should().BeTrue();
            versionOption.TempFlagForTest.Should().BeTrue();

        }


        [Fact]
        public void Extension_can_be_used_with_explicit_call_to_runner()
        {
            var rootCommand = new CliRootCommand
            { };
            var configuration = new CliConfiguration(rootCommand);
            var versionOption = new VersionExtension.VersionExtension();
            configuration.AddExtension(versionOption);
            var result = CliParser.Parse(rootCommand, "-v", configuration);

            var runner = new Runner();
            runner.Execute(result);

            versionOption.TempFlagForTest.Should().BeTrue();

        }
        */
    }
}