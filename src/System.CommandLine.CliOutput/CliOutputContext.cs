using System.Collections.Generic;
using System.IO;

namespace System.CommandLine.CliOutput;

public abstract class CliOutputContext

{
    public CliOutputContext(CliOutputConfiguration outputConfiguration, int maxWidth, TextWriter output)
    {
        MaxWidth = maxWidth <= 0
             ? int.MaxValue
             : maxWidth;
        OutputConfiguration = outputConfiguration;
        Writer = output ?? throw new ArgumentNullException(nameof(output));
    }

    public int MaxWidth { get; }
    public CliOutputConfiguration OutputConfiguration { get; }

    /// <summary>
    /// A text writer to write output to.
    /// </summary>
    public TextWriter Writer { get; }

}