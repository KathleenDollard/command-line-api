// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using FluentAssertions;
using Xunit;
using System.CommandLine.Parsing;
using System.CommandLine.RunnerExtension;
using System.CommandLine.VersionExtension;

namespace System.CommandLine.Extended.Tests
{
    public class RunnerTests
    {
        [Fact]
        public void Extension_runs_when_requested()
        {
            var rootCommand = new CliRootCommand
            {
                new CliOption<bool>("-x")
            };
            var configuration = new CliConfiguration(rootCommand);
            var versionOption = new VersionOption();
            configuration.AddExtension(versionOption);
            Runner runner = new Runner();
            configuration.AddExtension(runner);
            var result = CliParser.Parse(rootCommand, "-v", configuration );
            runner.Execute(result);

            versionOption.TempFlagForTest.Should().BeTrue();

        }

        [Fact]
        public void Extension_does_not_runs_when_not_requested()
        {
            var rootCommand = new CliRootCommand
            {
                new CliOption<bool>("-x")
            };
            var configuration = new CliConfiguration(rootCommand);
            var versionOption = new VersionOption();
            configuration.AddExtension(versionOption);
            Runner runner = new Runner();
            configuration.AddExtension(runner);
            var result = CliParser.Parse(rootCommand, "-x", configuration);
            runner.Execute(result);

            versionOption.TempFlagForTest.Should().BeFalse();

        }
    }
}