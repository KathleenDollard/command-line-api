using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine.CliOutput
{
    public abstract class CliTable : CliOutputUnit
    {
        public abstract string[,] GetTableCells();

    }


    public class CliTable<T> : CliTable
    {
        public CliTable(int columnCount, IEnumerable<T>? data)
        {
            ColumnCount = columnCount;
            Headers = new Func<T, string>[columnCount];
            Footers = new Func<T, string>[columnCount];
            Body = new Func<T, string>[columnCount];
            Data = data ?? Enumerable.Empty<T>();
        }

        public int ColumnCount { get; }
        public Func<T, string>[] Headers { get; }
        public Func<T, string>[] Footers { get; }
        public Func<T, string>[] Body { get; }
        public IEnumerable<T> Data { get; }

        public override string[,] GetTableCells()
        {
            int rowCount = Data.Count();
            var ret = new string[rowCount, ColumnCount];
            var row = 0;
            foreach (var item in Data)
            {
                for (int col = 0; col < ColumnCount; col++)
                {
                    ret[row, col] = Body[col](item);
                }
                row++;
            }
            return ret;
        }

        ///// <summary>
        ///// Returns list of string rows. This is expected to be called by formatters that will supply the margin characters.
        ///// </summary>
        ///// <param name="dataArray">The data to display</param>
        ///// <param name="maxWidth">The maximum display width.</param>
        ///// <param name="interColumnMargin">The text to enter between the margins. If this contains hidden characters, also include the interColumnMarginWidth.</param>
        ///// <param name="leftMargin">The text to enter to the left of the table. If this contains hidden characters, also include the leftAndRightMarginWidth.</param>
        ///// <param name="rightMargin">The text to enter to the right of the table. If this contains hidden characters, also include the leftAndRightMarginWidth.</param>
        ///// <param name="interColumnMarginWidth">If hidden characters appearing the interColumnMargin, use this to override the default width.</param>
        ///// <param name="leftAndRightMarginWidth">If hidden characters appearing the leftMargin or rightMargin, use this to override the default width.</param>
        ///// <param name="headerWidths">If included, the minimum column width will be the header width.</param>
        ///// <returns>Returns a 2 dimensional array with an padded IEnumerable&lt;string> for each
        ///// cell in the table. Columns are padded to the maximum width needed by the data, 
        ///// which might be less than the maximumWidth. Calculations use the expected intercolumn, 
        ///// left, and right margins, but no chrome is output. This allows formatters to add 
        ///// their own chrome, potentially with color.
        ///// </returns>
        //public IEnumerable<string> GetOutput(int maxWidth,
        //                                     string interColumnMargin,
        //                                     string leftMargin,
        //                                     string rightMargin,
        //                                     int? interColumnMarginWidth = null,
        //                                     int? leftAndRightMarginWidth = null,
        //                                     int[]? headerWidths = null)
        //=> GetPaddedArrayOutput(GetTableCells(), maxWidth, interColumnMargin, leftMargin, rightMargin,
        //    interColumnMarginWidth, leftAndRightMarginWidth, headerWidths);
    }
}
