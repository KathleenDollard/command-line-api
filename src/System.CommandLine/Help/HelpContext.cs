// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace System.CommandLine.Help
{
    /// <summary>
    /// Supports formatting command line help.
    /// </summary>
    public class HelpContext
    {
        /// <param name="command">The command for which help is being formatted.</param>
        /// <param name="output">A text writer to write output to.</param>
        /// <param name="parseResult">The result of the current parse operation.</param>
        public HelpContext(
            CliCommand command,
            CliConfiguration cliConfiguration,
            int maxWidth,
            TextWriter output,
            ParseResult? parseResult = null)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            MaxWidth = maxWidth <= 0
                 ? int.MaxValue
                 : maxWidth;
            Output = output ?? throw new ArgumentNullException(nameof(output));
            ParseResult = parseResult ?? ParseResult.Empty();
            CliConfiguration = cliConfiguration;
        }

        /// <summary>
        /// The result of the current parse operation.
        /// </summary>
        public ParseResult ParseResult { get; }

        /// <summary>
        /// The command for which help is being formatted.
        /// </summary>
        public CliCommand Command { get; }
        public int MaxWidth { get; }

        /// <summary>
        /// A text writer to write output to.
        /// </summary>
        public TextWriter Output { get; }

        internal bool WasSectionSkipped { get; set; }

        public CliConfiguration CliConfiguration { get; }

        public string FormatterName { get; set; }
    }
}