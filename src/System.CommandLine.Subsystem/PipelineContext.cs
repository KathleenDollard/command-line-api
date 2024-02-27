// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.CommandLine.Subsystem
{
    public class PipelineContext(ParseResult parseResult, ConsoleHack consoleHack)
    {
        public ParseResult ParseResult { get; } = parseResult;
        public ConsoleHack ConsoleHack { get; } = consoleHack;

        public bool AlreadyHandled { get; set; }

    }
}
