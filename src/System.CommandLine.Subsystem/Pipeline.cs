using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace System.CommandLine.Subsystem
{
    public class Pipeline
    {
        private readonly List<CliSubsystem> _extensions = new();

        public void AddExtension(CliSubsystem extension) => _extensions.Add(extension);
        public IEnumerable<CliSubsystem> Extensions => _extensions;

        public void InitializeExtensions(CliConfiguration configuration, IReadOnlyList<string> arguments, string rawInput)
        {
            foreach (var extension in _extensions)
            {
                extension.PipelineSupport?.Initialization(configuration, arguments, rawInput);
            }
        }

        public void TearDownExtensions(ParseResult parseResult)
        {
            foreach (var extension in _extensions)
            {
                extension.PipelineSupport?.TearDown(parseResult);
            }
        }

        public void ExecuteRequestedExtensions(PipelineContext pipelineContext)
        {
            foreach (var extension in _extensions)
            {
                if (!pipelineContext.AlreadyHandled || extension.PipelineSupport.RunsEvenIfAlreadyHandled)
                {
                 extension.PipelineSupport?.ExecuteIfNeeded(pipelineContext);
                }
            }
        }

        public ParseResult Parse(CliConfiguration configuration, string rawInput)
            => Parse(configuration, CliParser.SplitCommandLine(rawInput).ToArray(), rawInput);

        public ParseResult Parse(CliConfiguration configuration, string[] args, string rawInput)
        {
            InitializeExtensions(configuration, args, rawInput);
            var parseResult = CliParser.Parse(configuration.RootCommand, args, configuration);
            TearDownExtensions(parseResult);
            return parseResult;
        }

        public PipelineContext Execute(CliConfiguration configuration, string rawInput)
            => Execute(configuration, CliParser.SplitCommandLine(rawInput).ToArray(), rawInput);

        public PipelineContext Execute(CliConfiguration configuration, string[] args, string rawInput)
        {
            ParseResult parseResult = Parse(configuration, args, rawInput);
            var pipelineContext = new PipelineContext(parseResult, new ConsoleHack());
            ExecuteRequestedExtensions(pipelineContext);
            return pipelineContext;
        }
    }
}
