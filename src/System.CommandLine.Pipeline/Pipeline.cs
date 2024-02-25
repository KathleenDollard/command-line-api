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

        public void RunExtensionsBeforeParsing(CliConfiguration configuration, IReadOnlyList<string> arguments, string rawInput)
        {
            foreach (var extension in _extensions)
            {
                extension.PipelineExtension.BeforeParsing(configuration, arguments, rawInput);
            }
        }

        public void RunExtensionsAfterParsing(ParseResult parseResult)
        {
            foreach (var extension in _extensions)
            {
                extension.PipelineExtension.AfterParsing(parseResult);
            }
        }

        public PipelineResult Parse(CliConfiguration configuration, string rawInput)
            => Parse(configuration, CliParser.SplitCommandLine(rawInput).ToArray(), rawInput);

        public PipelineResult Parse(CliConfiguration configuration, string[] args, string rawInput)
        {
            RunExtensionsBeforeParsing(configuration, args, rawInput);
            var parseResult = CliParser.Parse(configuration.RootCommand,args, configuration);
            RunExtensionsAfterParsing(parseResult);
            return new PipelineResult(parseResult);
        }
    }
}
