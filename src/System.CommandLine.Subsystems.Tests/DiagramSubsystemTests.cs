﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using System.CommandLine.Directives;
using System.CommandLine.Parsing;
using Xunit;

namespace System.CommandLine.Subsystems.Tests
{
    public class DiagramSubsystemTests
    {

        [Theory]
        [ClassData(typeof(TestData.Diagram))]
        // TODO: Not sure why these tests are passing
        public void Version_is_activated_only_when_requested(string input, bool expectedIsActive, int expectedSkipArgs)
        {
            CliRootCommand rootCommand = [new CliCommand("x")];
            var configuration = new CliConfiguration(rootCommand);
            var directiveSupport = new SharedDirectiveSupport();
            var subsystem = new DiagramSubsystem(directiveSupport);
            var args = CliParser.SplitCommandLine(input).ToList().AsReadOnly();

            Subsystem.Initialize(subsystem, configuration, args);

            var parseResult = CliParser.Parse(rootCommand, input, configuration);
            var isActive = Subsystem.GetIsActivated(subsystem, parseResult);

            isActive.Should().Be(expectedIsActive);
        }
    }
}
