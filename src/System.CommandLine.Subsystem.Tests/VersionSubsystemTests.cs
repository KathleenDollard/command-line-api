// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using FluentAssertions;
using Xunit;
using System.CommandLine.Subsystem;
using System.Xml;

namespace System.CommandLine.Subsystem.Tests
{
    public class VersionSubsystemTests
    {
        private static readonly string? version = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly())
                                                 ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                                 ?.InformationalVersion;

        private readonly string newLine = Environment.NewLine;

        [Fact]
        public void Outputs_assembly_version()
        {
            var consoleHack = new ConsoleHack().RedirectToBuffer(true);
            var versionSubsystem = new VersionSubsystem();
            Subsystem.Execute(versionSubsystem, new PipelineContext(null, "", consoleHack));
            consoleHack.GetBuffer().Trim().Should().Be(version);
        }

        [Fact]
        public void Outputs_specified_version()
        {
            var consoleHack = new ConsoleHack().RedirectToBuffer(true);
            var versionSubsystem = new VersionSubsystem
            {
                SpecificVersion = "42"
            };
            Subsystem.Execute(versionSubsystem, new PipelineContext(null, "", consoleHack));
            consoleHack.GetBuffer().Trim().Should().Be("42");
        }

        [Fact]
        public void Outputs_assembly_version_when_specified_version_set_to_null()
        {
            var consoleHack = new ConsoleHack().RedirectToBuffer(true);
            var versionSubsystem = new VersionSubsystem
            {
                SpecificVersion = null
            };
            Subsystem.Execute(versionSubsystem, new PipelineContext(null, "", consoleHack));
            consoleHack.GetBuffer().Trim().Should().Be(version);
        }

        [Fact]
        public void Console_output_can_be_tested()
        {
            CliConfiguration configuration = new(new CliRootCommand())
            { };

            var consoleHack = new ConsoleHack().RedirectToBuffer(true);
            var versionSubsystem = new VersionSubsystem();
            Subsystem.Execute(versionSubsystem, new PipelineContext(null, "",consoleHack));
            consoleHack.GetBuffer().Trim().Should().Be(version);
        }


        [Fact]
        public void When_the_version_option_is_specified_then_the_version_is_written_to_standard_out()
        {
            var configuration = new CliConfiguration(new CliRootCommand());
            var pipeline = new Pipeline();
            var consoleHack = new ConsoleHack().RedirectToBuffer(true);
            pipeline.Version = new VersionSubsystem();

            var exit = pipeline.Execute(configuration, "-v", consoleHack);

            exit.ExitCode.Should().Be(0);
            exit.Handled.Should().BeTrue();
            consoleHack.GetBuffer().Should().Be($"{version}{newLine}");

        }


        // TODO: Add functional tests from previous version
        /*


                [Fact]
                public async Task When_the_version_option_is_specified_then_invocation_is_short_circuited()
                {
                    var wasCalled = false;
                    var rootCommand = new CliRootCommand();
                    rootCommand.SetAction((_) => wasCalled = true);

                    CliConfiguration configuration = new(rootCommand)
                    {
                        Output = new StringWriter()
                    };

                    await configuration.InvokeAsync("--version");

                    wasCalled.Should().BeFalse();
                }

                [Fact]
                public void When_the_version_option_is_specified_then_the_version_is_parsed()
                {
                    ParseResult parseResult = CliParser.Parse (
                        new CliRootCommand(),
                        [ "--version"]);

                    parseResult.Errors.Should().BeEmpty();
                    parseResult.GetValue(configuration.RootCommand.Options.OfType<VersionOption>().Single()).Should().BeTrue();
                }

                [Fact]
                public async Task Version_option_appears_in_Version()
                {
                    CliConfiguration configuration = new(new CliRootCommand())
                    {
                        Output = new StringWriter()
                    };

                    await configuration.InvokeAsync("--Version");

                    configuration.Output
                           .ToString()
                           .Should()
                           .Match("*Options:*--version*Show version information*");
                }

                [Fact]
                public async Task When_the_version_option_is_specified_and_there_are_default_options_then_the_version_is_written_to_standard_out()
                {
                    var rootCommand = new CliRootCommand
                    {
                        new CliOption<bool>("-x")
                        {
                            DefaultValueFactory = (_) => true
                        },
                    };
                    rootCommand.SetAction((_) => { });

                    CliConfiguration configuration = new(rootCommand)
                    {
                        Output = new StringWriter()
                    };

                    await configuration.InvokeAsync("--version");

                    configuration.Output.ToString().Should().Be($"{version}{NewLine}");
                }

                [Fact]
                public async Task When_the_version_option_is_specified_and_there_are_default_arguments_then_the_version_is_written_to_standard_out()
                {
                    CliRootCommand rootCommand = new()
                    {
                        new CliArgument<bool>("x") { DefaultValueFactory =(_) => true },
                    };
                    rootCommand.SetAction((_) => { });

                    CliConfiguration configuration = new(rootCommand)
                    {
                        Output = new StringWriter()
                    };

                    await configuration.InvokeAsync("--version");

                    configuration.Output.ToString().Should().Be($"{version}{NewLine}");
                }


                [Fact]
                public void When_version_extension_is_used_the_version_option_is_added_to_the_root()
                {
                    var rootCommand = new CliRootCommand
                    {
                        new CliOption<bool>("-x")
                    };
                    var configuration = new CliConfiguration(rootCommand);
                    var pipeline = new Pipeline.Pipeline();
                    pipeline.AddExtension(new VersionExtension.VersionExtension());
                    var result = pipeline.Parse(configuration, "");

                    rootCommand.Options.Should().NotBeNull();
                    rootCommand.Options
                        .Count(x => x.Name == "--version")
                        .Should()
                        .Be(1);

                }


                [Theory]
                [InlineData("--version", "-x", Skip = SkipValidationTests)]
                [InlineData("--version", "subcommand", Skip = SkipValidationTests)]
                public void Version_is_not_valid_with_other_tokens(params string[] commandLine)
                {
                    var subcommand = new CliCommand("subcommand");
                    var rootCommand = new CliRootCommand
                    {
                        subcommand,
                        new CliOption<bool>("-x")
                    };

                    var result = CliParser.Parse(rootCommand, commandLine);

                    result.Errors.Should().Contain(e => e.Message == "--version option cannot be combined with other arguments.");
                }

                [Fact]
                public void Version_option_is_not_added_to_subcommands()
                {
                    var childCommand = new CliCommand("subcommand");

                    var rootCommand = new CliRootCommand
                    {
                        childCommand
                    };
                    var configuration = new CliConfiguration(rootCommand);
                    var pipeline = new Pipeline.Pipeline();
                    pipeline.AddExtension(new VersionExtension.VersionExtension());
                    var result = pipeline.Parse(configuration, "");

                    childCommand.Options.Should().NotBeNull();
                    childCommand.Options
                        .Count(x => x.Name == "--version")
                        .Should()
                        .Be(0);
                }

                 // TODO: determine if this was just to check aliases or there is a use case for the CLI author, rather than the extension author, changing the option
                 [Fact]
                  public void Version_can_specify_additional_alias()
                  {
                      var versionOption = new VersionOption("-version", "-v");
                      CliRootCommand rootCommand = [versionOption];

                      var parseResult = CliParser.Parse(rootCommand, ["-version"]);
                      var versionSpecified = parseResult.GetValue(versionOption);
                      versionSpecified.Should().BeTrue();

                      parseResult = CliParser.Parse(rootCommand, ["-v"]);
                      versionSpecified = parseResult.GetValue(versionOption);
                      versionSpecified.Should().BeTrue();
                  }


                  [Fact(Skip = SkipValidationTests)]
                  public void Version_is_not_valid_with_other_tokens_uses_custom_alias()
                  {
                      var childCommand = new CliCommand("subcommand");
                      var rootCommand = new CliRootCommand
                      {
                          childCommand
                      };

                      rootCommand.Options[0] = new VersionOption("-v");

                      var result = CliParser.Parse(rootCommand, ["-v", "subcommand"]);

                      result.Errors.Should().ContainSingle(e => e.Message == "-v option cannot be combined with other arguments.");
                  }
                      */
    }
}