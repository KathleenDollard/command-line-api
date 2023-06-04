using System.Collections.Generic;
using System.CommandLine.CliOutput;
using Xunit;

namespace System.CommandLine.Tests.Help
{
    public class TableTests
    {

        private readonly string indent = new string(' ', 2);

        [Fact]
        public void ShortStringsDoNotWrap()
        {
            var table = new CliTable<string[]>(4,
                new List<string[]>
                    {
                        new  string[]{ "1", "2", "3", "4" }
                    });
            var expected = new List<string>
            {
                "1  2  3  4"
            };
            CliFormatter consoleFormatter = new CliConsoleFormatter(leftTableMargin: "", rightTableMargin: "", interColumnTableMargin: indent);
            var actual = consoleFormatter.FormatTable(table, maxWidth: 100);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WrappingRemovesWhitespace()
        {
            var table = new CliTable<string[]>(4,
               new List<string[]>
            {
                   new string[] { "1", "This is a sufficiently long string that it should wrap" }
            });
            var expected = new List<string>
            {
                "1  This is a sufficiently long ",
                "   string that it should wrap  "
            };
            CliFormatter consoleFormatter = new CliConsoleFormatter(leftTableMargin: "", rightTableMargin: "", interColumnTableMargin: indent);
            var actual = consoleFormatter.FormatTable(table, maxWidth: 31);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WrappingOccursAtWordBoundary()
        {
            var table = new CliTable<string[]>(4,
               new List<string[]>
            {
               new string[]{"1", "This is a sufficiently long string that it should wrap" }
            });
            var expected = new List<string>
                {
                "1  This is a sufficiently ",
                "   long string that it    ",
                "   should wrap            "
            };
            CliFormatter consoleFormatter = new CliConsoleFormatter(leftTableMargin: "", rightTableMargin: "", interColumnTableMargin: indent);
            var actual = consoleFormatter.FormatTable(table, maxWidth: 28);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SpaceReleasedByWrappingIsReused()
        {
            var table = new CliTable<string[]>(4,
              new List<string[]>

          {
                new string[] {"1", "This is a sufficiently long string that it should wrap", "1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16" }
            });
            var expected = new List<string>
            {
                "1  This is a sufficiently   1 2 3 4 5 6 7 8 9 10 11 12 ",
                "   long string that it      13 14 15 16                ",
                "   should wrap                                         "
            };
            CliFormatter consoleFormatter = new CliConsoleFormatter(leftTableMargin: "", rightTableMargin: "", interColumnTableMargin: indent);
            var actual = consoleFormatter.FormatTable(table, maxWidth: 55);

            Assert.Equal(expected, actual);
        }


        [Fact]
        public void ColumnsNotExpandedPastTheirSize()
        {
            var table = new CliTable<string[]>(4,
             new List<string[]>

           {
               new string[] {"1", "This is sufficiently long", "1 2 3 4 5 6 7 8 9 10 11" }
            });

            var expected = new List<string>
            {
                "1  This is            1 2 3 4 5 6 7 8 9 10 ",
                "   sufficiently long  11                   ",
            };
            CliFormatter consoleFormatter = new CliConsoleFormatter(leftTableMargin: "", rightTableMargin: "", interColumnTableMargin: indent);
            var actual = consoleFormatter.FormatTable(table, maxWidth: 43);

            Assert.Equal(expected, actual);
        }
    }
}
