// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.CommandLine.Tests.Utility;
using System.IO;
using FluentAssertions;
using System.Linq;
using Xunit;
using System.CommandLine.Directives;

namespace System.CommandLine.Tests
{
    public class ResponseFileTests : IDisposable
    {
        private readonly List<FileInfo> _responseFiles = new();

        public void Dispose()
        {
            foreach (var responseFile in _responseFiles)
            {
                responseFile.Delete();
            }
        }

        private string CreateResponseFile(params string[] lines)
        {
            var responseFile = new FileInfo(Path.GetTempFileName());

            using (var writer = new StreamWriter(responseFile.FullName))
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
            }

            _responseFiles.Add(responseFile);

            return responseFile.FullName;
        }

        // Powderhouse: Changed test since then now need to explicitly add the response file handler
        [Fact]
        public void When_response_file_specified_it_loads_options_from_response_file()
        {
            var option = new CliOption<bool>("--flag");
            var rootCommand = new CliRootCommand { option };
            var configuration = new CliConfiguration(rootCommand)
            {
                ResponseFileTokenReplacer = ResponseSubsystem.DefaultTokenReplacer
            };

            var result = CliParser.Parse(rootCommand, $"@{CreateResponseFile("--flag")}", configuration);

            result.GetResult(option).Should().NotBeNull();
        }

        [Fact]
        public void When_response_file_is_specified_it_loads_options_with_arguments_from_response_file()
        {
            var responseFile = CreateResponseFile(
                "--flag",
                "--flag2",
                "123");

            var optionOne = new CliOption<bool>("--flag");

            var optionTwo = new CliOption<int>("--flag2");
            var rootCommand = new CliRootCommand
                         {
                             optionOne,
                             optionTwo
                         };
            var configuration = new CliConfiguration(rootCommand)
            {
                ResponseFileTokenReplacer = ResponseSubsystem.DefaultTokenReplacer
            };

            var result = CliParser.Parse(rootCommand, $"@{responseFile}", configuration);

            result.GetResult(optionOne).Should().NotBeNull();
            result.GetValue(optionTwo).Should().Be(123);
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void When_response_file_is_specified_it_loads_command_arguments_from_response_file()
        {
            var responseFile = CreateResponseFile(
                "one",
                "two",
                "three");

            var rootCommand = new CliRootCommand
            {
                new CliArgument<string[]>("arg")
            };
            var configuration = new CliConfiguration(rootCommand)
            {
                ResponseFileTokenReplacer = ResponseSubsystem.DefaultTokenReplacer
            };

            var result = CliParser.Parse(rootCommand, $"@{responseFile}", configuration);

            result.CommandResult
                  .GetStringTokens()
                  .Should()
                  .BeEquivalentSequenceTo("one", "two", "three");
        }

        [Fact]
        public void Response_file_can_provide_subcommand_arguments()
        {
            var responseFile = CreateResponseFile(
                "one",
                "two",
                "three");

            var rootCommand = new CliRootCommand
                         {
                             new CliCommand("subcommand")
                             {
                                 new CliArgument<string[]>("arg")
                             }
                         };
            var configuration = new CliConfiguration(rootCommand)
            {
                ResponseFileTokenReplacer = ResponseSubsystem.DefaultTokenReplacer
            };

            var result = CliParser.Parse(rootCommand, $"subcommand @{responseFile}", configuration);

            result.CommandResult
                  .GetStringTokens()
                  .Should()
                  .BeEquivalentSequenceTo("one", "two", "three");
        }

        [Fact]
        public void Response_file_can_provide_subcommand()
        {
            var responseFile = CreateResponseFile("subcommand");

            var rootCommand = new CliRootCommand
                         {
                             new CliCommand("subcommand")
                             {
                                 new CliArgument<string[]>("arg")
                             }
                         };
            var configuration = new CliConfiguration(rootCommand)
            {
                ResponseFileTokenReplacer = ResponseSubsystem.DefaultTokenReplacer
            };

            var result = CliParser.Parse(rootCommand, $"@{responseFile} one two three", configuration);

            result.CommandResult
                  .GetStringTokens()
                  .Should()
                  .BeEquivalentSequenceTo("one", "two", "three");
        }

        [Fact]
        public void When_response_file_is_specified_it_loads_subcommand_arguments_from_response_file()
        {
            var responseFile = CreateResponseFile(
                "one",
                "two",
                "three");

            var rootCommand = new CliRootCommand
                         {
                             new CliCommand("subcommand")
                             {
                                 new CliArgument<string[]>("arg")
                             }
                         };
            var configuration = new CliConfiguration(rootCommand)
            {
                ResponseFileTokenReplacer = ResponseSubsystem.DefaultTokenReplacer
            };

            var result = CliParser.Parse(rootCommand, $"subcommand @{responseFile}", configuration);

            result.CommandResult
                  .GetStringTokens()
                  .Should()
                  .BeEquivalentSequenceTo("one", "two", "three");
        }

        [Fact]
        public void Response_file_can_contain_blank_lines()
        {
            var responseFile = CreateResponseFile(
                "--flag",
                "",
                "123");

            var option = new CliOption<int>("--flag");

            var rootCommand = new CliRootCommand
                {
                    option
                };
            var configuration = new CliConfiguration(rootCommand)
            {
                ResponseFileTokenReplacer = ResponseSubsystem.DefaultTokenReplacer
            };

            var result = CliParser.Parse(rootCommand, $"@{responseFile}", configuration);

            result.GetValue(option).Should().Be(123);
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Response_file_can_contain_comments_which_are_ignored_when_loaded()
        {
            var optionOne = new CliOption<bool>("--flag");
            var optionTwo = new CliOption<bool>("--flag2");

            var responseFile = CreateResponseFile(
                "# comment one",
                "--flag",
                "# comment two",
                "#",
                " # comment two",
                "--flag2");

            var rootCommand = new CliRootCommand
            {
                optionOne,
                optionTwo
            };
            var configuration = new CliConfiguration(rootCommand)
            {
                ResponseFileTokenReplacer = ResponseSubsystem.DefaultTokenReplacer
            };

            var result = CliParser.Parse(rootCommand,$"@{responseFile}", configuration);

            result.GetResult(optionOne).Should().NotBeNull();
            result.GetResult(optionTwo).Should().NotBeNull();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void When_response_file_does_not_exist_then_error_is_returned()
        {
            var optionOne = new CliOption<bool>("--flag");
            var optionTwo = new CliOption<bool>("--flag2");

            var rootCommand = new CliRootCommand
                         {
                             optionOne,
                             optionTwo
                         };
            var configuration = new CliConfiguration(rootCommand)
            {
                ResponseFileTokenReplacer = ResponseSubsystem.DefaultTokenReplacer
            };

            var result = CliParser.Parse(rootCommand, "@nonexistent.rsp", configuration);

            result.GetResult(optionOne).Should().BeNull();
            result.GetResult(optionTwo).Should().BeNull();
            result.Errors.Should().HaveCount(1);
            result.Errors.Single().Message.Should().Be("Response file not found 'nonexistent.rsp'.");
        }

        [Fact]
        public void When_response_filepath_is_not_specified_then_error_is_returned()
        {
            var optionOne = new CliOption<bool>("--flag");
            var optionTwo = new CliOption<bool>("--flag2");

            var rootCommand = new CliRootCommand
                         {
                             optionOne,
                             optionTwo
                         };
            var configuration = new CliConfiguration(rootCommand)
            {
                ResponseFileTokenReplacer = ResponseSubsystem.DefaultTokenReplacer
            };

            var result = CliParser.Parse(rootCommand, "@");

            result.GetResult(optionOne).Should().BeNull();
            result.GetResult(optionTwo).Should().BeNull();
            result.Errors.Should().HaveCount(1);
            result.Errors
                  .Single()
                  .Message
                  .Should()
                  .Be("Unrecognized command or argument '@'.");
        }

        [Fact]
        public void When_response_file_cannot_be_read_then_specified_error_is_returned()
        {
            var nonexistent = Path.GetTempFileName();
            var optionOne = new CliOption<bool>("--flag");
            var optionTwo = new CliOption<bool>("--flag2");

            using (File.Open(nonexistent, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                var rootCommand = new CliRootCommand
                             {
                                 optionOne,
                                 optionTwo
                             };
                var configuration = new CliConfiguration(rootCommand)
                {
                    ResponseFileTokenReplacer = ResponseSubsystem.DefaultTokenReplacer
                };

                var result = CliParser.Parse(rootCommand, $"@{nonexistent}", configuration);

                result.GetResult(optionOne).Should().BeNull();
                result.GetResult(optionTwo).Should().BeNull();
                result.Errors.Should().HaveCount(1);
                result.Errors.Single().Message.Should().StartWith($"Error reading response file '{nonexistent}'");
            }
        }

        [Theory]
        [InlineData("--flag \"first value\" --flag2 123")]
        [InlineData("--flag:\"first value\" --flag2:123")]
        [InlineData("--flag=\"first value\" --flag2=123")]
        public void When_response_file_parse_as_space_separated_returns_expected_values(string input)
        {
            var responseFile = CreateResponseFile(input);

            var optionOne = new CliOption<string>("--flag");
            var optionTwo = new CliOption<int>("--flag2");

            var rootCommand = new CliRootCommand
            {
                optionOne,
                optionTwo
            };
            var configuration = new CliConfiguration(rootCommand)
            {
                ResponseFileTokenReplacer = ResponseSubsystem.DefaultTokenReplacer
            };

            var result = CliParser.Parse(rootCommand, $"@{responseFile}", configuration);

            result.GetValue(optionOne).Should().Be("first value");
            result.GetValue(optionTwo).Should().Be(123);
        }

        [Fact]
        public void When_response_file_processing_is_disabled_then_it_returns_response_file_name_as_argument()
        {
            
            CliRootCommand command = new ()
            {
                new CliArgument<List<string>>("arg")
            };
            CliConfiguration configuration = new(command)
            {
                ResponseFileTokenReplacer = null
            };

            var result = CliParser.Parse(command, "@file.rsp", configuration);
            var commandResult = result.CommandResult;

            commandResult
                .GetStringTokens()
                .Should()
                .Contain("@file.rsp");

            ValueResult valueResult = commandResult.ValueResults
                            .Single();
            valueResult
                .ValueSymbol
                .Should()
                .BeOfType<CliArgument<List<string>>>();
            valueResult
                .GetValue<List<string>>()
                .Should()
                .Contain("@file.rsp");
            result.Errors.Should().HaveCount(0);
        }

        [Fact]
        public void Response_files_can_refer_to_other_response_files()
        {
            var file3 = CreateResponseFile("--three", "3");
            var file2 = CreateResponseFile($"@{file3}", "--two", "2");
            var file1 = CreateResponseFile("--one", "1", $"@{file2}");

            var option1 = new CliOption<int>("--one");
            var option2 = new CliOption<int>("--two");
            var option3 = new CliOption<int>("--three");

            var rootCommand = new CliRootCommand
                          {
                              option1,
                              option2,
                              option3
                          };
            var configuration = new CliConfiguration(rootCommand)
            {
                ResponseFileTokenReplacer = ResponseSubsystem.DefaultTokenReplacer
            };

            var result = CliParser.Parse(rootCommand, $"@{file1}", configuration);

            result.GetValue(option1).Should().Be(1);
            result.GetValue(option1).Should().Be(1);
            result.GetValue(option2).Should().Be(2);
            result.GetValue(option3).Should().Be(3);
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void When_response_file_options_or_arguments_contain_trailing_spaces_they_are_ignored()
        {
            var responseFile = CreateResponseFile("--option1 ", "value1 ", "--option2\t", "2\t");

            var option1 = new CliOption<string>("--option1");
            var option2 = new CliOption<int>("--option2");

            var rootCommand = new CliRootCommand { option1, option2 };
            var configuration = new CliConfiguration(rootCommand)
            {
                ResponseFileTokenReplacer = ResponseSubsystem.DefaultTokenReplacer
            };

            var result = CliParser.Parse(rootCommand, $"@{responseFile}", configuration);

            result.GetValue(option1).Should().Be("value1");
            result.GetValue(option2).Should().Be(2);
        }

        [Fact]
        public void When_response_file_options_or_arguments_contain_leading_spaces_they_are_ignored()
        {
            var responseFile = CreateResponseFile(" --option1", " value1", "\t--option2", "\t2");

            var option1 = new CliOption<string>("--option1");
            var option2 = new CliOption<int>("--option2");

            var rootCommand = new CliRootCommand { option1, option2 };
            var configuration = new CliConfiguration(rootCommand)
            {
                ResponseFileTokenReplacer = ResponseSubsystem.DefaultTokenReplacer
            };

            var result = CliParser.Parse(rootCommand, $"@{responseFile}", configuration);

            result.GetValue(option1).Should().Be("value1");
            result.GetValue(option2).Should().Be(2);
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void When_response_file_options_or_arguments_contain_trailing_and_leading_spaces_they_are_ignored()
        {
            var responseFile = CreateResponseFile(" --option1 ", " value1 ", "\t--option2\t", "\t2\t");

            var option1 = new CliOption<string>("--option1");
            var option2 = new CliOption<int>("--option2");

            var rootCommand = new CliRootCommand { option1, option2 };
            var configuration = new CliConfiguration(rootCommand)
            {
                ResponseFileTokenReplacer = ResponseSubsystem.DefaultTokenReplacer
            };

            var result = CliParser.Parse(rootCommand, $"@{responseFile}", configuration);

            result.GetValue(option1).Should().Be("value1");
            result.GetValue(option2).Should().Be(2);
            result.Errors.Should().BeEmpty();
        }
    }
}
