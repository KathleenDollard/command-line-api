namespace System.CommandLine.Help;

public class CliHelpFormatting
{
    public CliHelpFormatting(int indent = 0, int maxWidth = 0)
    {
        Indent = indent == 0 ? 2 : indent;
        MaxWidth = maxWidth;
    }

    public int Indent { get; }
    public int MaxWidth { get; }

    /// <summary>
    /// Dispalys the heading. This can be overridden to provide custom formatting
    /// </summary>
    /// <param name="heading"></param>
    public virtual string Heading(string? heading)
        => heading = heading ?? string.Empty;

}
