using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.CommandLine.CliOutput
{
    public class CliDataOutputUnit : CliOutputUnit
    {
        public CliDataOutputUnit(IEnumerable<object> value, int indentLevel = 0)
        {
            Value = value;
            IndentLevel = indentLevel;
        }

        public IEnumerable<object> Value { get; }
    }
}
