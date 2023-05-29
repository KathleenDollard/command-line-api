using System;
using System.Collections.Generic;
using System.CommandLine.Help.Formatting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.CommandLine.Tests.Help
{
    public class TableTests
    {

        private readonly string indent = new string(' ', 2);

        [Fact]
        public void ShortStringsDoNotWrap()
        {
            var initial = new string[,]
            {
                { "1", "2", "3", "4" }
            };
            var expected = new List<string>
            {
                "1  2  3  4"
            };
            var actual = Table.GetPaddedArrayOutput(initial, leftMargin: "", rightMargin: "", maxWidth: 100, interColumnMargin: indent);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WrappingRemovesWhitespace()
        {
            var initial = new string[,]
            {
                {"1", "This is a sufficiently long string that it should wrap" }
            };
            var expected = new List<string>
            {
                "1  This is a sufficiently long ",
                "   string that it should wrap  "
            };
            var actual = Table.GetPaddedArrayOutput(initial, leftMargin: "", rightMargin: "", maxWidth: 31, interColumnMargin: indent);

            Assert.Equal(expected, actual);
        }


        [Fact]
        public void WrappingOccursAtWordBoundary()
        {
            var initial = new string[,]
            {
               {"1", "This is a sufficiently long string that it should wrap" }
            };
            var expected = new List<string>
                {
                "1  This is a sufficiently ",
                "   long string that it    ",
                "   should wrap            "
            };
            var actual = Table.GetPaddedArrayOutput(initial, leftMargin: "", rightMargin: "", maxWidth: 28, interColumnMargin: indent);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SpaceReleasedByWrappingIsReused()
        {
            var initial = new string[,]
           {
                 {"1", "This is a sufficiently long string that it should wrap", "1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16" }
            };
            var expected = new List<string>
            {
                "1  This is a sufficiently   1 2 3 4 5 6 7 8 9 10 11 12 ",
                "   long string that it      13 14 15 16                ",
                "   should wrap                                         "
            };
            var actual = Table.GetPaddedArrayOutput(initial, leftMargin: "", rightMargin: "", maxWidth: 55, interColumnMargin: indent);

            Assert.Equal(expected, actual);
        }


        [Fact]
        public void ColumnsNotExpandedPastTheirSize()
        {
            var initial = new string[,]
             {
                {"1", "This is sufficiently long", "1 2 3 4 5 6 7 8 9 10 11" }
            };

            var expected = new List<string>
            {
                "1  This is            1 2 3 4 5 6 7 8 9 10 ",
                "   sufficiently long  11                   ",
            };
            var actual = Table.GetPaddedArrayOutput(initial, leftMargin: "", rightMargin: "", maxWidth: 43, interColumnMargin: indent);

            Assert.Equal(expected, actual);
        }
    }
}
