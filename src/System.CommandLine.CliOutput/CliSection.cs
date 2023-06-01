using System.Collections.Generic;

namespace System.CommandLine.CliOutput
{


    public abstract class CliSection
    {

        protected CliSection(CliFormatter formatter,
                             string header,
                             bool emitHeaderOnEmptyBody = false)
        {
            Formatter = formatter;
            Header = header;
            EmitHeaderOnEmptyBody = emitHeaderOnEmptyBody;
        }

        public CliFormatter Formatter { get; }
        public CliOutputContext OutputContext { get; }
        protected string Header { get; }
        public bool EmitHeaderOnEmptyBody { get; }


        public virtual IEnumerable<string>? GetOpening(CliOutputContext context)
        => new string[]
            {
                Heading(Header)
            };

        public virtual IEnumerable<string>? GetBody(CliOutputContext context) => null;

        public virtual IEnumerable<string>? GetClosing(CliOutputContext context) => null;

        public virtual string Heading(string? heading)
            => heading ?? string.Empty;

    }
}

