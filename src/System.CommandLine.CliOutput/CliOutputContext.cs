using System.Collections.Generic;
using System.IO;

namespace System.CommandLine.CliOutput;

public abstract class CliOutputContext

{
    public CliOutputContext( int maxWidth, TextWriter output)
    {
        MaxWidth = maxWidth <= 0
             ? int.MaxValue
             : maxWidth;
        Writer = output ?? throw new ArgumentNullException(nameof(output));
    }

    public int MaxWidth { get; }


    /// <summary>
    /// A text writer to write output to.
    /// </summary>
    public TextWriter Writer { get; }

}