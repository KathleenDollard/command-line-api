namespace System.CommandLine.Subsystem
{
    public class CliExit(ParseResult parseResult)
    {
        public CliExit(ParseResult parseResult, bool handled, int exitCode)
            : this(parseResult)
        {
            Handled = handled;
            ExitCode = exitCode;
        }
        public ParseResult ParseResult { get; set; } = parseResult;

        public bool Handled { get; set; }
        public int ExitCode { get; }

        public static implicit operator int(CliExit cliExit) => cliExit.ExitCode;

    }
}
