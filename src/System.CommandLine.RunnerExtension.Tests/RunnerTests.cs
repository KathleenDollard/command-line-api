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
        public void When_runner_extension_is_used_an_extension_runs()
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
    }
}