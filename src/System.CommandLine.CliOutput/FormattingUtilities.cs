using System.Collections.Generic;
using System.Text;

namespace System.CommandLine.CliOutput
{
    public static class FormattingUtilities
    {
        public static IEnumerable<string> WrapText(string text, string indent, int maxWidth)
        {
            if (text is null)
            {
                yield break;
            }

            maxWidth = maxWidth - indent.Length;

            //First handle existing new lines
            var parts = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (string part in parts)
            {
                if (part.Length <= maxWidth)
                {
                    yield return $"{indent}{part}";
                }
                else
                {
                    //Long item, wrap it based on the width
                    for (int i = 0; i < part.Length;)
                    {
                        if (part.Length - i < maxWidth)
                        {
                            yield return $"{indent}{part.Substring(i)}";
                            break;
                        }
                        else
                        {
                            int length = -1;
                            for (int j = 0; j + i < part.Length && j < maxWidth; j++)
                            {
                                if (char.IsWhiteSpace(part[i + j]))
                                {
                                    length = j + 1;
                                }
                            }
                            if (length == -1)
                            {
                                length = maxWidth;
                            }
                            yield return $"{indent}{part.Substring(i, length)}"; 

                            i += length;
                        }
                    }
                }
            }
        }

#if !NET6_0_OR_GREATER
        public static void AppendJoin(this StringBuilder sb, string separator, IEnumerable<string> parts)
        {
            sb.Append(string.Join(separator, parts));
        }
#endif
    }
}
