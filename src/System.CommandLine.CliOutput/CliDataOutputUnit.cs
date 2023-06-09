using System.Collections.Generic;

namespace System.CommandLine.CliOutput
{
    // KAD: this class is intended to support Json, etc output parallel to Table/text. Work was temporarily halted ont that.
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
