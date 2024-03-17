using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.IO;
using FluentAssertions;
using FluentAssertions.Equivalency;
using System.Linq;
using FluentAssertions.Common;
using Xunit;
using Xunit.Abstractions;


namespace System.CommandLine.Tests
{
    public partial class TokenizerTests
    {

        [Fact]
        public void The_tokenizer_can_handle_single_option()
        {
            var option = new CliOption<string>("--hello");
            var command = new CliRootCommand { option };
            IReadOnlyList<string> args = ["--hello", "world"];
            List<CliToken> tokens = null;
            List<string> errors = null;
            Tokenizer.Tokenize(args, command, new CliConfiguration(command), false, true, out tokens, out errors);

            tokens
                .Skip(1)
                .Select(t => t.Value)
                .Should()
                .BeEquivalentTo("--hello", "world");

            errors.Should().BeNull();
        }

        [Fact]
        public void Location_stack_is_correct()
        {
            var option = new CliOption<string>("--hello");
            var command = new CliRootCommand { option };
            IReadOnlyList<string> args = ["--hello", "world"];
            List<CliToken> tokens = null;
            List<string> errors = null;
            Tokenizer.Tokenize(args,
                               command,
                               new CliConfiguration(command),
                               false,
                               true,
                               out tokens,
                               out errors);
            var locations = tokens
                            .Skip(1)
                            .Select(t => t.Location.ToString())
                            .ToList();

            errors.Should().BeNull();
            tokens.Count.Should().Be(3);
            locations.Count.Should().Be(2);
            locations[0].Should().Be("User [-1, 8, 0]; User [0, 7, 0]");
            locations[1].Should().Be("User [-1, 8, 0]; User [1, 5, 0]");
        }
    }
}
