// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Parsing;

namespace System.CommandLine.Subsystem
{
    public class Pipeline
    {
        private readonly List<CliSubsystem> _otherExtensions = new();
        public IEnumerable<CliSubsystem> OtherExtensions => _otherExtensions;

        public HelpSubsystem? Help { get; set; }
        public VersionSubsystem? Version { get; set; }
        public ErrorReportingSubsystem? ErrorReporting { get; set; }
        public CompletionsSubsystem? Completions { get; set; }

        public void AddOtherExtension(CliSubsystem extension) => _otherExtensions.Add(extension);

        public void InitializeExtensions(CliConfiguration configuration)
        {
            foreach (var extension in _otherExtensions)
            {
                extension.Initialize(configuration);
            }
        }

        public void TearDownExtensions(ParseResult parseResult)
        {
            foreach (var extension in _otherExtensions)
            {
                extension.TearDown(parseResult);
            }
        }

        public void ExecuteRequestedExtensions(PipelineContext pipelineContext)
        {
            foreach (var extension in _otherExtensions)
            {
                //if (!pipelineContext.AlreadyHandled || extension.RunsEvenIfAlreadyHandled)
                if (!pipelineContext.AlreadyHandled )
                {
                    extension.ExecuteIfNeeded(pipelineContext);
                }
            }
        }

        public ParseResult Parse(CliConfiguration configuration, string rawInput)
            => Parse(configuration, CliParser.SplitCommandLine(rawInput).ToArray(), rawInput);

        public ParseResult Parse(CliConfiguration configuration, string[] args, string rawInput)
        {
            InitializeExtensions(configuration);
            var parseResult = CliParser.Parse(configuration.RootCommand, args, configuration);
            TearDownExtensions(parseResult);
            return parseResult;
        }

        public CliExit Execute(CliConfiguration configuration, string rawInput, ConsoleHack? consoleHack = null)
            => Execute(configuration, CliParser.SplitCommandLine(rawInput).ToArray(), rawInput, consoleHack);

        public CliExit Execute(CliConfiguration configuration, string[] args, string rawInput, ConsoleHack? consoleHack = null)
            => Execute(Parse(configuration, args, rawInput), consoleHack);

        public CliExit Execute(ParseResult parseResult, ConsoleHack? consoleHack = null)
        {
            var pipelineContext = new PipelineContext(parseResult, consoleHack ?? new ConsoleHack());
            ExecuteRequestedExtensions(pipelineContext);
            return new CliExit(pipelineContext);
        }

    }
}
