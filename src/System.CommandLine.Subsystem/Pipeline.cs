// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Parsing;
using System.CommandLine.Subsystem;

namespace System.CommandLine
{
    public class Pipeline
    {
        //private readonly List<CliSubsystem> _otherExtensions = new();
        //public IEnumerable<CliSubsystem> OtherExtensions => _otherExtensions;

        public HelpSubsystem? Help { get; set; }
        public VersionSubsystem? Version { get; set; }
        public ErrorReportingSubsystem? ErrorReporting { get; set; }
        public CompletionsSubsystem? Completions { get; set; }

        //public void AddOtherExtension(CliSubsystem extension) => _otherExtensions.Add(extension);

        public virtual void InitializeExtensions(CliConfiguration configuration) 
            => configuration.InitializeSubsystem(Help)
                .InitializeSubsystem(Version)
                .InitializeSubsystem(ErrorReporting)
                .InitializeSubsystem(Completions);

        public void TearDownExtensions(PipelineContext pipelineContext)
            => pipelineContext.TeardownSubsystem(Help)
                .TeardownSubsystem(Version)
                .TeardownSubsystem(ErrorReporting)
                .TeardownSubsystem(Completions);

        protected virtual void ExecuteRequestedExtensions(PipelineContext pipelineContext) 
            => pipelineContext.ExecuteSubSystemIfNeeded(Help)
                .ExecuteSubSystemIfNeeded(Version)
                .ExecuteSubSystemIfNeeded(ErrorReporting)
                .ExecuteSubSystemIfNeeded(Completions);

        public ParseResult Parse(CliConfiguration configuration, string rawInput)
            => Parse(configuration, CliParser.SplitCommandLine(rawInput).ToArray());

        public ParseResult Parse(CliConfiguration configuration, string[] args)
        {
            InitializeExtensions(configuration);
            var parseResult = CliParser.Parse(configuration.RootCommand, args, configuration);
            return parseResult;
        }

        public CliExit Execute(CliConfiguration configuration, string rawInput, ConsoleHack? consoleHack = null)
            => Execute(configuration, CliParser.SplitCommandLine(rawInput).ToArray(), rawInput, consoleHack);

        public CliExit Execute(CliConfiguration configuration, string[] args, string rawInput, ConsoleHack? consoleHack = null)
            => Execute(Parse(configuration, args), rawInput, consoleHack);

        public CliExit Execute(ParseResult parseResult, string rawInput, ConsoleHack? consoleHack = null)
        {
            var pipelineContext = new PipelineContext(parseResult, rawInput, consoleHack ?? new ConsoleHack());
            ExecuteRequestedExtensions(pipelineContext);
            return new CliExit(pipelineContext);
        }

    }
}
