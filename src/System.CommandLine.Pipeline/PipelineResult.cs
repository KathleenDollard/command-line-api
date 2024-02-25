
namespace System.CommandLine.Pipeline
{
    public class PipelineResult(ParseResult parseResult)
    {
        public PipelineResult(ParseResult parseResult, bool handled, int exitCode)
            : this(parseResult)
        {
            Handled = handled;
            ExitCode = exitCode;
        }
        public ParseResult ParseResult { get; set; } = parseResult;

        public bool Handled { get; set; }

        public int ExitCode { get; }
    }
}