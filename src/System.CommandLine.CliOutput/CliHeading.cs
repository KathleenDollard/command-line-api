namespace System.CommandLine.CliOutput
{
    public class CliHeading : CliOutputUnit
    {
        public CliHeading(string value, int indentLevel = 0)
        {
            Value = value;
            IndentLevel = indentLevel;
        }

        public string Value { get; }
    }
}
