// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.CommandLine.Subsystem
{
    // TODO: Consider what info is needed after invocation. If it's the whole pipeline context, consider collapsing this with that class.
    public class CliExit(ParseResult parseResult)
    {
        public CliExit(ParseResult parseResult, bool handled, int exitCode)
            : this(parseResult)
        {
            ExitCode = exitCode;
        }
        public ParseResult ParseResult { get; set; } = parseResult;

        public int ExitCode { get; }

        public static implicit operator int(CliExit cliExit) => cliExit.ExitCode;

    }
}
