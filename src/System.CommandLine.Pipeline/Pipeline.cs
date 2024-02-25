using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace System.CommandLine.Pipeline
{
    public class Pipeline
    {
        private readonly List<Extension> _extensions = new();

        public void AddExtension(Extension extension) => _extensions.Add(extension);
        public IEnumerable<Extension> Extensions => _extensions;

        public void InitializeExtensions(CliConfiguration configuration, IReadOnlyList<string> arguments, string rawInput)
        {
            foreach (var extension in _extensions)
            {
                extension.PipelineExtension.Initialization(configuration, arguments, rawInput);
            }
        }

        public void TearDownExtensions(ParseResult parseResult)
        {
            foreach (var extension in _extensions)
            {
                extension.PipelineExtension.TearDown(parseResult);
            }
        }

        public void ExecuteRequestedExtensions(PipelineResult pipelineResult)
        {
            bool handled = false;
            foreach (var extension in _extensions)
            {
                handled = extension.PipelineExtension.ExecuteIfNeeded(pipelineResult.ParseResult);
                if (handled)
                {
                    break;
                }
            }
            pipelineResult.Handled = handled;
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

        public PipelineResult Execute(CliConfiguration configuration, string rawInput)
            => Execute(configuration, CliParser.SplitCommandLine(rawInput).ToArray(), rawInput);

        public PipelineResult Execute(CliConfiguration configuration, string[] args, string rawInput)
        {
            var pipelineResult = new PipelineResult(Parse(configuration, args, rawInput));
            ExecuteRequestedExtensions(pipelineResult);
            return pipelineResult;
        }
    }
}
