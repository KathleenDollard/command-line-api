using System.Collections.Generic;
using System.CommandLine.CliOutput;
using System.Linq;

namespace System.CommandLine.Help
{
    // KAD: This class only exists because of current intuition that it may be needed. Delete if not used for meaningful stuff when finalizing design.
    public class CliHelpBuilder : CliOutputRenderer
    {
        public CliHelpBuilder()
        {
            {
                Sections.AddRange(new CliSection[]
                {
                    new CliHelpSynopsis(),
                    new CliHelpUsage(),
                    new CliHelpArguments(),
                    new CliHelpOptions(),
                    new CliHelpSubcommands(),
                });
            }
        }

        // This temporarily satisfies IHelpBuilder
        public virtual void Write(HelpContext helpContext)
        {
            _ = helpContext ?? throw new ArgumentNullException(nameof(helpContext));

            base.Write(helpContext);
        }


        /// <summary>
        /// Provides the formatter for output. Derived classes can use this to select a formatter.
        /// </summary>
        /// <param name="outputContext"></param>
        /// <returns></returns>
        public override CliFormatter GetFormatter(CliOutputContext outputContext)
            => outputContext switch
            {
                HelpContext helpContext =>
                    ((helpContext.CliConfiguration[HelpConfiguration.Key] as HelpConfiguration)?.Formatter)
                        ?? base.GetFormatter(outputContext),
                _ => base.GetFormatter(outputContext)
            };
    }
}
