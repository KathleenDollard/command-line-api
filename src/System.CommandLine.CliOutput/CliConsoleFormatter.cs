using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine.CliOutput
{
    public class CliConsoleFormatter : CliFormatter
    {
        public string InterColumnTableMargin { get; }
        public string LeftTableMargin { get; }
        public string RightTableMargin { get; }
        public int InterColumnTableMarginWidth { get; }
        public int LeftAndRightTableMarginWidth { get; }
        private int indentSize = 2;

        public CliConsoleFormatter(string interColumnTableMargin = "  ",
                                   string leftTableMargin = "",
                                   string rightTableMargin = "",
                                   int? interColumnTableMarginWidth = null,
                                   int? leftAndRightTableMarginWidth = null)
        {
            IndentWidth = 2;
            InterColumnTableMargin = interColumnTableMargin;
            LeftTableMargin = leftTableMargin;
            RightTableMargin = rightTableMargin;
            InterColumnTableMarginWidth = interColumnTableMarginWidth ?? interColumnTableMargin.Length;
            LeftAndRightTableMarginWidth = leftAndRightTableMarginWidth ?? leftTableMargin.Length + rightTableMargin.Length;
        }

        public override IEnumerable<string>? FormatHeading(CliHeading? heading, int maxWidth)
        => heading?.Value is null
            ? null
            : FormattingUtilities.WrapText(heading.Value, new string(' ', heading.IndentLevel * indentSize), maxWidth);

        public override IEnumerable<string>? FormatTable(CliTable? table, int maxWidth)
        {
            if (table is null) { return null; }

            var cells = table?.GetTableCells();
            if (cells is null || cells.GetLength(0) == 0)
            {
                return Enumerable.Empty<string>();
            }
            return GetTableOutput(cells, maxWidth: maxWidth, indentLevel: table!.IndentLevel);  // no idea why it missed the check above
        }

        public override IEnumerable<string>? FormatText(CliText? text, int maxWidth)
        => text?.Value is null
            ? null
            : FormattingUtilities.WrapText(text.Value, new string(' ', text.IndentLevel * indentSize), maxWidth);

        /// <summary>
        /// Returns list of string rows. This is expected to be called by formatters that will supply the margin characters.
        /// </summary>
        /// <param name="dataArray">The data to display</param>
        /// <param name="maxWidth">The maximum display width.</param>
        /// <param name="indentLevel">The level of indentation. This is multiplied byt the formatters IndentWidth to determine the left indent of the table</param>
        /// <param name="interColumnMargin">The text to enter between the margins. If this contains hidden characters, also include the interColumnMarginWidth.</param>
        /// <param name="leftMargin">The text to enter to the left of the table. If this contains hidden characters, also include the leftAndRightMarginWidth.</param>
        /// <param name="rightMargin">The text to enter to the right of the table. If this contains hidden characters, also include the leftAndRightMarginWidth.</param>
        /// <param name="interColumnMarginWidth">If hidden characters appearing the interColumnMargin, use this to override the default width.</param>
        /// <param name="leftAndRightMarginWidth">If hidden characters appearing the leftMargin or rightMargin, use this to override the default width.</param>
        /// <param name="headerWidths">If included, the minimum column width will be the header width.</param>
        /// <returns>Returns a 2 dimensional array with an padded IEnumerable&lt;string> for each
        /// cell in the table. Columns are padded to the maximum width needed by the data, 
        /// which might be less than the maximumWidth. Calculations use the expected intercolumn, 
        /// left, and right margins, but no chrome is output. This allows formatters to add 
        /// their own chrome, potentially with color.
        /// </returns>
        protected virtual IEnumerable<string> GetTableOutput(string[,] dataArray,
                                                     int maxWidth,
                                                     int indentLevel,
                                                     int[]? headerWidths = null)
        {
            var allCellText = dataArray;
            if (allCellText.Length == 0)
            {
                return Enumerable.Empty<string>();
            }
            int columnCount = allCellText.GetLength(1);
            headerWidths ??= new int[columnCount];

            var combinedMarginWidth = GetCombinedMarginWidths(columnCount, InterColumnTableMargin, LeftTableMargin, RightTableMargin, InterColumnTableMarginWidth, LeftAndRightTableMarginWidth);
            var usableWidth = maxWidth - combinedMarginWidth;
            usableWidth = usableWidth < 0 ? 0 : usableWidth;

            int[] currentColumnWidths = GetColumnMaxWidths(allCellText, headerWidths);

            var wrappedTableText = WrapColumns(allCellText, currentColumnWidths, usableWidth);
            var leftMargin = $"{new string(' ', indentLevel * IndentWidth)}{LeftTableMargin}";
            return PadTableOutput(wrappedTableText, currentColumnWidths, InterColumnTableMargin, leftMargin, RightTableMargin);
        }

        private static IEnumerable<string> PadTableOutput(IEnumerable<string>[,] wrappedCellText,
                                                  int[] currentColumnWidths,
                                                  string interColumnMargin,
                                                  string leftMargin,
                                                  string rightMargin)
        {
            var ret = new List<string>();
            for (var row = 0; row < wrappedCellText.GetLength(0); row++)
            {
                var maxLines = GetMaxNumberOfLinesInRow(wrappedCellText, row);
                for (int lineNumber = 0; lineNumber < maxLines; lineNumber++)
                {
                    var text = GetCellLine(wrappedCellText, currentColumnWidths, row, lineNumber);
                    ret.Add($"{leftMargin}{string.Join(interColumnMargin, text).TrimEnd()}{rightMargin}");
                }
            }
            return ret;

            static string[] GetCellLine(IEnumerable<string>[,] wrappedCellText, int[] currentColumnWidths, int row, int lineNumber)
            {
                int columnCount = wrappedCellText.GetLength(1);
                var ret = new string[columnCount];
                for (int col = 0; col < columnCount; col++)
                {
                    IEnumerable<string> lines = wrappedCellText[row, col];
                    ret[col] = lineNumber < lines.Count()
                        ? lines.Skip(lineNumber).First().PadRight(currentColumnWidths[col])
                        : new string(' ', currentColumnWidths[col]);
                }
                return ret;
            }
        }

        private static int GetCombinedMarginWidths(int columnCount,
                                           string interColumnMargin,
                                           string leftMargin,
                                           string rightMargin,
                                           int? interColumnMarginWidth = null,
                                           int? leftAndRightMarginWidth = null)
        {
            var interColumnWidth = interColumnMarginWidth ?? interColumnMargin.Length;
            var leftAndRightWidth = leftAndRightMarginWidth ?? leftMargin.Length + rightMargin.Length;
            return leftAndRightWidth + (interColumnWidth * (columnCount - 1));
        }


        private static int GetMaxNumberOfLinesInRow(IEnumerable<string>[,] wrappedCellText, int row)
        {
            var maxLines = 0;
            for (int col = 0; col < wrappedCellText.GetLength(1); col++)
            {
                var linesCount = wrappedCellText[row, col].Count();
                if (linesCount > maxLines)
                { maxLines = linesCount; }
            }
            return maxLines;
        }

        private static IEnumerable<string>[,] WrapColumns(string[,] allCellText, int[] currentColumnWidths, int usableWidth)
        {

            // The logic for wrapping is to use the current width or the wrapped width from left to right,
            // using the remaining space for the rightermost rows
            var wrappedCellText = new IEnumerable<string>[allCellText.GetLength(0), allCellText.GetLength(1)];
            var remainingWidth = usableWidth;
            var lastFairWidth = GetFairWidth(currentColumnWidths, remainingWidth, currentColumnWidths.Length);
            var remainingColumnCount = allCellText.GetLength(1);
            for (int col = 0; col < currentColumnWidths.Length; col++)
            {
                var requestedWidth = currentColumnWidths.Sum();
                var fairWidth = GetFairWidth(currentColumnWidths, remainingWidth, remainingColumnCount);
                if (currentColumnWidths[col] <= fairWidth)
                {
                    currentColumnWidths[col] = UpdateTextToCurrent(allCellText, wrappedCellText, col);
                }
                else
                {
                    currentColumnWidths[col] = UpdateTextToNewWidth(allCellText, wrappedCellText, col, fairWidth);
                }
                remainingColumnCount -= 1;
                remainingWidth -= currentColumnWidths[col];
            }
            return wrappedCellText;
        }

        private static int[] GetColumnMaxWidths(string[,] cells, int[] headerWidths)
        {
            var columnCount = cells.GetLength(1);
            var ret = new int[columnCount];
            for (int col = 0; col < cells.GetLength(1); col++)
            {
                var max = headerWidths[col];
                for (int row = 0; row < cells.GetLength(0); row++)
                {
                    if (cells[row, col].Length > max)
                    { max = cells[row, col].Length; }
                }
                ret[col] = max;
            }
            return ret;
        }

        private static int GetFairWidth(int[] currentColumnWidths, int remainingWidth, int remainingColumns)
        {
            // This would need to iterate a bit on with large number of columns to avoid skewing space to the right
            var tentative = remainingColumns == 0
                  ? remainingWidth
                  : remainingWidth / remainingColumns;
            var remainingNarrowColumns = currentColumnWidths
                        .Skip(currentColumnWidths.Length - remainingColumns)
                        .Select(x => (x < tentative, x));
            var remainingWideCount = remainingNarrowColumns.Count(t => !t.Item1);
            if (remainingWideCount == 0)
            {  // everything already fits
                return int.MaxValue;
            }
            var remainingNarrowExtra = remainingNarrowColumns.Where(t => t.Item1).Sum(t => tentative - t.Item2);
            remainingWidth += remainingNarrowExtra;
            return remainingWidth / remainingWideCount;
        }

        private static int UpdateTextToCurrent(string[,] allCellText, IEnumerable<string>[,] wrappedCellText, int currentColumn)
        {
            var maxWidth = 0;
            for (int row = 0; row < allCellText.GetLength(0); row++)
            {
                string text = allCellText[row, currentColumn];
                wrappedCellText[row, currentColumn] = new string[] { text };
                int length = text.Length;
                if (length > maxWidth)
                {
                    maxWidth = length;
                }
            }
            return maxWidth;
        }

        private static int UpdateTextToNewWidth(string[,] allCellText, IEnumerable<string>[,] wrappedCellText, int currentColumn, int fairWidth)
        {
            var maxColumnWidth = 0;
            for (int row = 0; row < allCellText.GetLength(0); row++)
            {
                var text = FormattingUtilities.WrapText(allCellText[row, currentColumn], "", fairWidth).ToArray();
                wrappedCellText[row, currentColumn] = text;
                var cellMaxWidth = text.Max(x => x.Length);
                if (cellMaxWidth > maxColumnWidth)
                { maxColumnWidth = cellMaxWidth; }
            }
            return maxColumnWidth;
        }

    }
}

