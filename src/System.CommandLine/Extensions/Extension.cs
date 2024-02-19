using System.Collections.Generic;

namespace System.CommandLine.Extensions
{
    public class Extension
    {
        private static readonly int spread = 100;
        public static readonly int CategoryBeforeValidation = 0;
        public static readonly int CategoryValidation = 2 * spread;
        public static readonly int CategoryAfterValidation = 3 * spread;
        public static readonly int CategoryBeforeHelp = 4 * spread;
        public static readonly int CategoryHelp = 6 * spread;
        public static readonly int CategoryBeforeInvocation = 8 * spread;
        public static readonly int CategoryBeforeFinishing = 10 * spread;
        public static readonly int CategoryFinishing = 12 * spread;

        protected Extension(string name, int category)
        {
            Name = name;
            Category = category;
        }

        /// <summary>
        /// The name of the extension. 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The order in which extensions will be run. This should be entered as one of the 
        /// category constants, or an incremented offset from those constants to ensure things 
        /// run in the correct order.
        /// </summary>
        public int Category { get; }

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
        public virtual bool BeforeParsing(CliCommand rootCommand, IReadOnlyList<string> arguments, string rawInput, CliConfiguration configuration) => true;

        /// <summary>
        /// Indicates to invocation patterns that the extension should be run.
        /// </summary>
        /// <remarks>
        /// This may be explicitly set, such as a directive like Diagram, or it may explore the result
        /// </remarks>
        /// <param name="result">The parse result.</param>
        /// <returns></returns>
        public virtual bool GetIsActivated(ParseResult result) => false;

        /// <summary>
        /// Runs before any extensions run to provide any setup. This should generally 
        /// check whether the extension is activated, unless it supplies info to other 
        /// extensions. Default behavior is to do nothing.
        /// </summary>
        /// <remarks>
        /// Running this before any extension is to support supplying info to other extensions.
        /// </remarks>
        /// <param name="result">The parse result.</param>
        /// <returns>Whether CLI execution has been handled. If this true, other extensions will not be run.</returns>
        public virtual bool AfterParsing(ParseResult result) => true;

        /// <summary>
        /// Runs before any extensions run. This should generally check whether the extension is activated, 
        /// nless it supplies info to other extensions. Default behavior is to do nothing.
        /// </summary>
        /// <remarks>
        /// Not all extensions may execute. Some may supply information to other extensions?
        /// </remarks>
        /// <param name="result">The parse result.</param>
        /// <returns>Whether CLI execution has been handled. If this true, other extensions will not be run.</returns>
        public virtual bool Execute(ParseResult result) => true;

        //TODO: Should there be an explicit cleanup method, or should we rely on derived classes implementing IDispose

    }
}
