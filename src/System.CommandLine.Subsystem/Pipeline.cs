﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Parsing;

namespace System.CommandLine.Subsystem
{
    public class Pipeline
    {
        private readonly List<CliSubsystem> _extensions = new();

        public void AddExtension(CliSubsystem extension) => _extensions.Add(extension);
        public IEnumerable<CliSubsystem> Extensions => _extensions;

        public void InitializeExtensions(CliConfiguration configuration)
        {
            foreach (var extension in _extensions)
            {
                extension.PipelineSupport?.Initialization(configuration);
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
