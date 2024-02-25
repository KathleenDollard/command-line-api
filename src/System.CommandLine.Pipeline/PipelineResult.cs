
namespace System.CommandLine.Pipeline
{
    public class PipelineResult(ParseResult parseResult)
    {
        public ParseResult ParseResult { get; set; } = parseResult;

        public int ExitCode { get; }
    }
}