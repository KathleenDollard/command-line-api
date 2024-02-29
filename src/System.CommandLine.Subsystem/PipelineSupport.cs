// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace System.CommandLine.Subsystem
{
    public class PipelineSupport(int category, bool runsEvenIfAlreadyHandled = false)
    {
        public static readonly int CategoryAfterRunner = 6 * spread; public static readonly int CategoryAfterValidation = 3 * spread;
        public static readonly int CategoryBeforeFinishing = 9 * spread;
        public static readonly int CategoryBeforeInvocation = 8 * spread;
        public static readonly int CategoryBeforeRunner = 4 * spread;
        public static readonly int CategoryBeforeValidation = 0;
        public static readonly int CategoryBeforeVersion = 4 * spread;
        public static readonly int CategoryFinishing = 10 * spread;
        public static readonly int CategoryRunner = 5 * spread;
        public static readonly int CategoryValidation = 2 * spread;
        public static readonly int CategoryVersion = 7 * spread;
        private static readonly int spread = 100;

        /// <summary>
        /// The order in which extensions will be run. This should be entered as one of the 
        /// category constants, or an incremented offset from those constants to ensure things 
        /// run in the correct order.
        /// </summary>
        public int Category { get; } = category;

        internal virtual bool RunsEvenIfAlreadyHandled { get; } = runsEvenIfAlreadyHandled;

        public virtual CliExit Execute(PipelineContext pipelineContext) => CliExit.NotRun(pipelineContext.ParseResult);

        public CliExit ExecuteIfNeeded(PipelineContext pipelineContext)
            => ExecuteIfNeeded(pipelineContext.ParseResult, pipelineContext.ConsoleHack, pipelineContext);

        public CliExit ExecuteIfNeeded(ParseResult parseResult, ConsoleHack consoleHack)
            => ExecuteIfNeeded(parseResult,consoleHack,null);

        protected CliExit ExecuteIfNeeded(ParseResult parseResult, ConsoleHack consoleHack, PipelineContext? pipelineContext = null)
            => GetIsActivated(parseResult)
                ? Execute(pipelineContext ?? new PipelineContext(parseResult, consoleHack))
                : CliExit.NotRun(parseResult);


        /// <summary>
        /// Indicates to invocation patterns that the extension should be run.
        /// </summary>
        /// <remarks>
        /// This may be explicitly set, such as a directive like Diagram, or it may explore the result
        /// </remarks>
        /// <param name="result">The parse result.</param>
        /// <returns></returns>
        public virtual bool GetIsActivated(ParseResult parseResult) => false;

        /// <summary>
        /// Runs before parsing to prepare the parser. Since it always runs, slow code that is only needed when the extension 
        /// runs as part of invocation should be delayed to BeforeRun(). Default behavior is to do nothing.
        /// </summary>
        /// <remarks>
        /// Use cases:
        /// * Add to the CLI, such as adding version option
        /// * Early setup of extension internal data, such as reading a file that contains defaults
        /// * Licensing if early exit is needed
        /// </remarks>
        /// <param name="rootCommand">The CLI declaration</param>
        /// <param name="arguments">The string arguments as parsed by .NET.</param>
        /// <param name="rawInput">The string as passed from the terminal shell. It may differ in whitespace from what the user entered.</param>
        /// <param name="configuration">The CLI configuration</param>
        /// <returns>True if parsing should continue</returns> // there might be a better design that supports a message
        // TODO: Because of this and similar usage, consider combining CLI declaration and config. ArgParse calls this the parser, which I like
        public virtual bool Initialization(CliConfiguration configuration) => true;

        public virtual bool TearDown(ParseResult result) => true;

    }

    public class PipelineSupport<TSubSystem>(int category, bool runsEvenIfAlreadyHandled = false) : PipelineSupport(category, runsEvenIfAlreadyHandled)
        where TSubSystem : CliSubsystem<TSubSystem>
    {
        protected internal TSubSystem Subsystem { get; set; }
#pragma warning restore IDE0075 // Simplify conditional expression

        //TODO: Should there be an explicit cleanup method, or should we rely on derived classes implementing IDispose


    }
}
