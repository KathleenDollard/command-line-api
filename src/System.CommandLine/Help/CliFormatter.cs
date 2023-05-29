using System.Collections.Generic;

namespace System.CommandLine.Help.Formatting
{
    public abstract class CliFormatter
    {
        public abstract IEnumerable<string> FormatTable<T>(Table<T> table, int maxWidth);
       
    }
}