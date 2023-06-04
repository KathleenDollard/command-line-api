// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.CommandLine.CliOutput;
using System.IO;
using System.Runtime.InteropServices;

namespace System.CommandLine.Help
{
    /// <summary>
    /// Supports formatting command line help.
    /// </summary>
    public class HelpContext : CliOutputContext
    {
        /// <param name="command">The command for which help is being formatted.</param>
        /// <param name="cliConfiguration">The configuration for the current CLI tree</param>
        /// <param name="writer">A text writer to write output to.</param>
        /// <param name="maxWidth">The maximum width of the displayed output</param>
        /// <param name="formatter">The CliFormatter that will be used. If null, the Console formatter will be used.</param>
        /// <param name="parseResult">The result of the current parse operation.</param>
        public HelpContext(CliConfiguration cliConfiguration,
                           int maxWidth,
                           TextWriter writer,
                           ParseResult? parseResult = null,
                           CliCommand? command = null)
            : base( cliConfiguration.HelpConfiguration, maxWidth, writer)
        {
            CliConfiguration = cliConfiguration;
            ParseResult = parseResult ?? ParseResult.Empty();
            CliConfiguration = cliConfiguration;
            Command = command ?? ParseResult.CommandResult.Command;
        }

        /// <summary>
        /// Access to the CliConfiguration for use in help
        /// </summary>
        public CliConfiguration CliConfiguration { get; }

        /// <summary>
        /// The result of the current parse operation.
        /// </summary>
        public ParseResult ParseResult { get; }


        /// <summary>
        /// The command for which help is being formatted.
        /// </summary>
        // It is not clear when this would be used other than testing - where it is rather difficult to
        // create a parseResult without excess scope sharing. And it is also a convenience.
        public CliCommand Command { get; }

        // KAD: Remove when old HelpBuilder is removed
        internal bool WasSectionSkipped { get; set; }

    }
}