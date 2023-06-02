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

            var sections = helpContext.CliConfiguration.HelpConfiguration.GetSections(helpContext);
            foreach (var section in sections)
            {
                IEnumerable<string> lines = new List<string>();
                var body = section.GetBody(helpContext);
                if ((body == null || !body.Any()) && !section.EmitHeaderOnEmptyBody)
                { continue; }

                var opening = section.GetOpening(helpContext);
                var closing = section.GetClosing(helpContext);

                if (opening is not null)
                {
                    if (section.EmitHeaderOnEmptyBody || (body is not null && body.Any()))
                    {
                        lines = lines.Concat(opening);
                    }
                }
                if (body is not null && body.Any())
                {
                    lines = lines.Concat(body);
                }

                if (closing is not null)
                {
                    if (section.EmitHeaderOnEmptyBody || (body is not null && body.Any()))
                    {
                        lines = lines.Concat(closing);
                    }
                }

                WriteOutput(lines, helpContext);
            }

            helpContext.Output.WriteLine();
            helpContext.Output.WriteLine();
        }

        private static void WriteOutput(IEnumerable<string> lines, HelpContext context)
        {

            if (lines.Any())
            {
                foreach (var line in lines)
                {
                    context.Output.WriteLine(line);
                }
                context.Output.WriteLine();
            }
        }

    }
}
