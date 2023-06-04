using System.Collections.Generic;

namespace System.CommandLine.CliOutput
{
    public abstract class CliOutputConfiguration
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

        public virtual CliFormatter GetFormatter(CliOutputContext outputContext)
            => new CliConsoleFormatter();
    }
}
