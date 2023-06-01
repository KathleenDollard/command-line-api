using System.Collections.Generic;

namespace System.CommandLine.CliOutput
{
    public abstract class CliFormatter
    {
        public abstract IEnumerable<string> FormatTable<T>(Table<T>? table, int maxWidth);

        public int IndentWidth { get; set; }
       
    }
}