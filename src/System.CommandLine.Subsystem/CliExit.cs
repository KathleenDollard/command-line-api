// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.



namespace System.CommandLine.Subsystem
{
    // TODO: Consider what info is needed after invocation. If it's the whole pipeline context, consider collapsing this with that class.
    public class CliExit(ParseResult parseResult)
    {
        public CliExit(PipelineContext pipelineContext) : this(pipelineContext.ParseResult, pipelineContext.AlreadyHandled, pipelineContext.ExitCode)
        { }

        private CliExit(ParseResult parseResult, bool handled, int exitCode)
            : this(parseResult)
        {
            ExitCode = exitCode;
            Handled = handled;
        }
        public ParseResult ParseResult { get; set; } = parseResult;

        public int ExitCode { get; }

        public static implicit operator int(CliExit cliExit) => cliExit.ExitCode;

        public bool Handled { get; }

        public static CliExit NotRun(ParseResult parseResult) => new CliExit(parseResult, false, 0);

        public static CliExit SuccessfullyHandled(ParseResult parseResult) => new CliExit(parseResult, true, 0);
    }
}
