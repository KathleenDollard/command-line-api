using System;
using System.Collections.Generic;
using System.CommandLine.CliOutput;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace System.CommandLine.Help.Formatting
{
    public class CliConsoleFormatter : CliFormatter
    {
        public CliConsoleFormatter()
        {
            IndentWidth = 2;
        }

        public override IEnumerable<string> FormatTable<T>(Table<T>? table, int maxWidth) 
        {
            if (table is null || !table.Data.Any())
            {
                return Enumerable.Empty<string>();
            }
            var indent = new string(' ', IndentWidth);
            return table.GetOutput(maxWidth, leftMargin: indent, rightMargin: "", interColumnMargin: indent);
        }
    }
}

