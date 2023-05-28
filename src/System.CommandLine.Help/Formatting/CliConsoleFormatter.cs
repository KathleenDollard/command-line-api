using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace System.CommandLine.Help.Formatting
{
    public class CliConsoleFormatter
    {
        // Redesign this so there is a fixed width font formatter into a 3-D array (or a 2D of string[]) in Table
        // The console formatter should just do the outputting of this. Not sure what use this would be without padding.
        // Ask Patrik about accommodating ANSI codes and making a fancy formatter. 
        // Also, add start and end spacing as well as column spacing and change the calculations to accommodate
   
        public static IEnumerable<string> FormatTable<T>(Table<T> table, int maxWidth) 
        {
            var indent = new string(' ', 2);
            return table.GetPaddedTableOutput(maxWidth, indent, indent, indent);

        }
    }
}

