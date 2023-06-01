using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine.CliOutput
{
    public class CliOutputRenderer
    {
        /// <summary>
        /// Writes help output for the specified command.
        /// </summary>
        public virtual void Write(CliOutputContext outputContext)
        {
            _ = outputContext ?? throw new ArgumentNullException(nameof(outputContext));

            // KAD: Consider this: If the user explicitly typed a hidden command, should they be able to get help for deprecated or preview features?
            //if (context.Command.Hidden)
            //{
            //    return;
            //}

            var sections = outputContext.GetSections();
            foreach (var section in sections)
            {
                IEnumerable<string> lines = new List<string>();
                var body = section.GetBody(outputContext);
                if ((body == null || !body.Any()) && !section.EmitHeaderOnEmptyBody)
                { continue; }

                var opening = section.GetOpening(outputContext);
                var closing = section.GetClosing(outputContext);

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

                WriteOutput(lines, outputContext);
            }

            outputContext.Output.WriteLine();
            outputContext.Output.WriteLine();
        }

        private static void WriteOutput(IEnumerable<string> lines, CliOutputContext outputContext)
        {

            if (lines.Any())
            {
                foreach (var line in lines)
                {
                    outputContext.Output.WriteLine(line);
                }
                outputContext.Output.WriteLine();
            }
        }

    }
}
