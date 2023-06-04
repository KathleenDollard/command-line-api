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
        public HelpContext(CliCommand command,
                           CliConfiguration cliConfiguration,
                           int maxWidth,
                           TextWriter writer,
                           ParseResult? parseResult = null,
                           CliFormatter? formatter = null)
            : base(maxWidth, writer, formatter ?? GetFormatter(parseResult))
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            ParseResult = parseResult ?? ParseResult.Empty();
            CliConfiguration = cliConfiguration;
        }

        private static CliFormatter? GetFormatter(ParseResult? parseResult)
        {
            if (parseResult is not null)
            {
                var formatterFactory = parseResult.Configuration.HelpConfiguration.GetFormatter;
                if (formatterFactory is not null)
                {
                    return formatterFactory(parseResult);
                }
            }
            return null;
        }

        public override IEnumerable<CliSection> GetSections()
            => CliConfiguration.HelpConfiguration.GetSections(this);

        /// <summary>
        /// The result of the current parse operation.
        /// </summary>
        public ParseResult ParseResult { get; }

        /// <summary>
        /// The command for which help is being formatted.
        /// </summary>
        public CliCommand Command { get; }

        internal bool WasSectionSkipped { get; set; }

        public CliConfiguration CliConfiguration { get; }

    }
}