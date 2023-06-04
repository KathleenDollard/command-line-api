namespace System.CommandLine.CliOutput
{
    public class CliText : CliOutputUnit
    {
        public CliText(string value, int indentLevel = 0)
        {
            Value = value;
            IndentLevel = indentLevel;
        }

        public string Value { get; }
    }
}
