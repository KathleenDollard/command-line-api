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

        public void ExecuteRequestedExtensions(CliExit cliExit)
        {
            foreach (var extension in _extensions)
            {
                extension.PipelineSupport?.ExecuteIfNeeded(cliExit.ParseResult);
                if (cliExit.Handled)
                {
                    break;
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

        public CliExit Execute(CliConfiguration configuration, string rawInput)
            => Execute(configuration, CliParser.SplitCommandLine(rawInput).ToArray(), rawInput);

        public CliExit Execute(CliConfiguration configuration, string[] args, string rawInput)
        {
            var cliExit = new CliExit(Parse(configuration, args, rawInput));
            ExecuteRequestedExtensions(cliExit);
            return cliExit;
        }
    }
}
