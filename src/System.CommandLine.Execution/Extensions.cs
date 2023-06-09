using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.CommandLine.Invocation
{
    public static class Extensions
    {  
        /// <summary>
        /// Invokes the appropriate command handler for a parsed command line input.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel an invocation.</param>
        /// <returns>A task whose result can be used as a process exit code.</returns>
        public static Task<int> InvokeAsync(this ParseResult parseResult, CancellationToken cancellationToken = default)
            => InvocationPipeline.InvokeAsync(parseResult, cancellationToken);

        /// <summary>
        /// Invokes the appropriate command handler for a parsed command line input.
        /// </summary>
        /// <returns>A value that can be used as a process exit code.</returns>
        public static int Invoke(this ParseResult parseResult) => InvocationPipeline.Invoke(parseResult);
 
        /// <summary>
        /// Parses a command line string value and invokes the handler for the indicated command.
        /// </summary>
        /// <returns>The exit code for the invocation.</returns>
        /// <remarks>The command line string input will be split into tokens as if it had been passed on the command line.</remarks>
        public static int Invoke(this CliConfiguration cliConfiguration, string commandLine)
            => cliConfiguration.RootCommand.Parse(commandLine, cliConfiguration).Invoke();

        /// <summary>
        /// Parses a command line string array and invokes the handler for the indicated command.
        /// </summary>
        /// <returns>The exit code for the invocation.</returns>
        public static int Invoke(this CliConfiguration cliConfiguration, string[] args)
            => cliConfiguration.RootCommand.Parse(args, cliConfiguration).Invoke();

        /// <summary>
        /// Parses a command line string value and invokes the handler for the indicated command.
        /// </summary>
        /// <returns>The exit code for the invocation.</returns>
        /// <remarks>The command line string input will be split into tokens as if it had been passed on the command line.</remarks>
        public static Task<int> InvokeAsync(this CliConfiguration cliConfiguration, string commandLine, CancellationToken cancellationToken = default)
            => cliConfiguration.RootCommand.Parse(commandLine, cliConfiguration).InvokeAsync(cancellationToken);

        /// <summary>
        /// Parses a command line string array and invokes the handler for the indicated command.
        /// </summary>
        /// <returns>The exit code for the invocation.</returns>
        public static Task<int> InvokeAsync(this CliConfiguration cliConfiguration, string[] args, CancellationToken cancellationToken = default)
            => cliConfiguration.RootCommand.Parse(args, cliConfiguration).InvokeAsync(cancellationToken);
   }
}
