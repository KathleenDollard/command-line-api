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


            var sections = GetSections(outputContext);
            var formatter = GetFormatter(outputContext);
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

            WriteLines(outputContext.Writer, formatter.GetOutput(dom, outputContext.MaxWidth));
        }

        protected virtual CliFormatter GetFormatter(CliOutputContext outputContext)
        => outputContext.OutputConfiguration.GetFormatter(outputContext);

        protected virtual IEnumerable<CliSection> GetSections(CliOutputContext outputContext)
            => outputContext.OutputConfiguration.Sections;

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
