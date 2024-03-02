// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.CommandLine.Subsystem
{
    public class Subsystem
    {
        public static void Initialize(CliSubsystem subsystem, CliConfiguration configuration)
            => subsystem.Initialize(configuration);

        public static CliExit Execute(CliSubsystem subsystem, PipelineContext pipelineContext)
            => subsystem.Execute(pipelineContext);

        public static bool GetIsActivated(CliSubsystem subsystem, ParseResult parseResult)
            => subsystem.GetIsActivated(parseResult);

        public static CliExit ExecuteIfNeeded(CliSubsystem subsystem, PipelineContext pipelineContext)
            => subsystem.ExecuteIfNeeded(pipelineContext);

        public static CliExit ExecuteIfNeeded(CliSubsystem subsystem, ParseResult parseResult, ConsoleHack consoleHack)
            => subsystem.ExecuteIfNeeded(parseResult, consoleHack);

        public static CliExit ExecuteIfNeeded(CliSubsystem subsystem, ParseResult parseResult, ConsoleHack consoleHack, PipelineContext? pipelineContext = null)
            => subsystem.ExecuteIfNeeded(parseResult, consoleHack, pipelineContext);

    }
}
