using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace System.CommandLine.Help
{
    public class CliHelpBuilder : IHelpBuilder
    {
        /// <summary>
        /// Writes help output for the specified command.
        /// </summary>
        public virtual void Write(HelpContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            // KAD: Consider this: If the user explicitly typed a hidden command, should they be able to get help for deprecated or preview features?
            //if (context.Command.Hidden)
            //{
            //    return;
            //}

            var sections = context.CliConfiguration.HelpConfiguration.Sections;
            foreach (var section in sections)
            {
                IEnumerable<string> lines = new List<string>();
                var body = section.GetBody(context);
                if ((body == null || !body.Any()) && !section.EmitHeaderOnEmptyBody)
                { continue; }

                var opening = section.GetOpening(context);
                var closing = section.GetClosing(context);

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

                WriteOutput(lines, context);
            }

            context.Output.WriteLine();
            context.Output.WriteLine();
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
