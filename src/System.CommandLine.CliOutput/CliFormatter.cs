using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System.CommandLine.CliOutput
{
    public abstract class CliFormatter
    {
        public abstract IEnumerable<string>? FormatTable(CliTable? table, int maxWidth);
        public abstract IEnumerable<string>? FormatHeading(CliHeading? heading, int maxWidth);
        public abstract IEnumerable<string>? FormatText(CliText? text, int maxWidth);
        public virtual IEnumerable<string>? Format(CliOutputUnit? value, int maxWidth)
        => value switch
        {
            CliTable table => FormatTable(table, maxWidth),
            CliHeading heading => FormatHeading(heading, maxWidth),
            CliText text => FormatText(text, maxWidth),
            _ => throw new NotImplementedException()
        };

        public int IndentWidth { get; set; }

        public IEnumerable<string>? GetOutput(IEnumerable<CliOutputUnit> dom, int maxWidth)
        {
            IEnumerable<string> output = new List<string>();
            foreach (var unit in dom)
            {
                IEnumerable<string>? unitOutput = null;
                unitOutput = Format(unit, maxWidth);

                if (unitOutput is not null)
                {
                    output = output.Concat(unitOutput);
                }
            }
            return output.Any()
                ? output
                : null;
        }


    }
}