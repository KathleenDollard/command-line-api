using System.Collections.Generic;
using System.CommandLine.CliOutput;
using System.Linq;

namespace System.CommandLine.Help
{
    public class CliHelpBuilder : CliOutputRenderer, IHelpBuilder
    {
        // This temporarily satisfies IHelpBuilder
        public virtual void Write(HelpContext helpContext)
        {
            _ = helpContext ?? throw new ArgumentNullException(nameof(helpContext));

             base.Write(helpContext);
        }

        private static void WriteOutput(IEnumerable<string> lines, HelpContext context)
        {

            if (lines.Any())
            {
                foreach (var line in lines)
                {
                    context.Writer.WriteLine(line);
                }
                context.Writer.WriteLine();
            }
        }

    }
}
