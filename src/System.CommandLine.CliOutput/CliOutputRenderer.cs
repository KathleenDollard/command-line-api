using System.Collections.Generic;
using System.IO;
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
            var dom = new List<CliOutputUnit>();
            foreach (var section in sections)
            {
                var body = section.GetBody(outputContext);
                bool hasBody = body is not null && body.Any();
                if (hasBody || section.EmitHeaderWhenNoData)
                {
                    IEnumerable<CliOutputUnit>? opening = section.GetOpening(outputContext);
                    if (opening is not null && opening.Any())
                    { dom.AddRange(opening); }
                }
                if (hasBody)
                {
                    dom.AddRange(body!);  // hasBody does the null check
                }
                if (hasBody || section.EmitHeaderWhenNoData)
                {
                    IEnumerable<CliOutputUnit>? closing = section.GetClosing(outputContext);
                    if (closing is not null && closing.Any())
                    { dom.AddRange(closing); }
                }
            }

            WriteLines(outputContext.Writer, outputContext.Formatter.GetOutput(dom, outputContext.MaxWidth));

            //    if (opening is not null)
            //    {
            //        if (section.EmitHeaderOnEmptyBody || (body is not null && body.Any()))
            //        {
            //            lines = lines.Concat(opening);
            //        }
            //    }
            //    if (body is not null && body.Any())
            //    {
            //        lines = lines.Concat(body);
            //    }

            //    if (closing is not null)
            //    {
            //        if (section.EmitHeaderOnEmptyBody || (body is not null && body.Any()))
            //        {
            //            lines = lines.Concat(closing);
            //        }
            //    }

            //    WriteOutput(lines, outputContext);
            //}

            //outputContext.Output.WriteLine();
            //outputContext.Output.WriteLine();

        }

        private void WriteLines(TextWriter writer, IEnumerable<string>? output)
        {
            if (output is null)
            { return; }
            foreach (var line in output)
            {
                writer.WriteLine(line);
            }
        }

    }
}
