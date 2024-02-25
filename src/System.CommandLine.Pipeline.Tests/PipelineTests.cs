using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.CommandLine.Pipeline.Tests
{
    public class PipelineTests
    {

/* These tests are from a version of invocation that is not currently working


        [Fact]
        public void Extension_runs_when_requested()
        {
            var rootCommand = new CliRootCommand
            {
                new CliOption<bool>("-x")
            };
            var configuration = new CliConfiguration(rootCommand);
            var versionOption = new VersionExtension.VersionExtension();
            var pipeline = new Pipeline();
            pipeline.AddExtension(versionOption);
            Runner runner = new Runner();
            configuration.AddExtension(runner);
            var result = CliParser.Parse(rootCommand, "-v", configuration);
            runner.Execute(result);

            versionOption.TempFlagForTest.Should().BeTrue();

        }

        [Fact(Skip = "Bug that causes recursion")]
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

        */
    }
}
