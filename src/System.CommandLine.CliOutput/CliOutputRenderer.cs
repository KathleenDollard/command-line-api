using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace System.CommandLine.CliOutput
{
    public abstract class CliOutputRenderer
    {
        private List<CliSection>? sections;

        public List<CliSection> Sections
        {
            get
            {
                sections ??= new();
                return sections;
            }
        }

        /// <summary>
        /// Provides the formatter for output. Derived classes can use this to select a formatter.
        /// </summary>
        /// <param name="outputContext"></param>
        /// <returns></returns>
        public virtual CliFormatter GetFormatter(CliOutputContext outputContext)
            => new CliConsoleFormatter(); 
        
        /// <summary>
        /// Writes help output for the specified command.
        /// </summary>
        public virtual void Write(CliOutputContext outputContext)
        {
            _ = outputContext ?? throw new ArgumentNullException(nameof(outputContext));


            var formatter = GetFormatter(outputContext);
            var dom = new List<CliOutputUnit>();
            foreach (var section in Sections)
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
