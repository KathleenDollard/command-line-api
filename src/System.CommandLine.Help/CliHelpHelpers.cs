using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine.Help
{
    public static class CliHelpHelpers
    {
        public static IEnumerable<string> WrapText(string text, int maxWidth)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                yield break;
            }

            //First handle existing new lines
            var parts = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (string part in parts)
            {
                if (part.Length <= maxWidth)
                {
                    yield return part;
                }
                else
                {
                    //Long item, wrap it based on the width
                    for (int i = 0; i < part.Length;)
                    {
                        if (part.Length - i < maxWidth)
                        {
                            yield return part.Substring(i);
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
                            yield return part.Substring(i, length);

                            i += length;
                        }
                    }
                }
            }
        }

        public static IEnumerable<string> WrapAndIndentText(string? text, int maxWidth, int indent)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Enumerable.Empty<string>();
            }

            IEnumerable<string> parts = WrapText(text!, maxWidth);
            var ret = new List<string>();
            foreach (var part in parts)
            {
                ret.Add($"{GetIndentString(indent)}{part}");
            }
            return ret;
        }


        private static string GetIndentString(int indent)
        => new string(' ', indent);

        public static IEnumerable<string>? WriteTwoColumns(IEnumerable<TwoColumnHelpRow> items, int maxWidth, int indent)
        {
            if (!items.Any())
            {
                return null;
            }

            int windowWidth = maxWidth;

            int firstColumnWidth = items.Select(x => x.FirstColumnText.Length).Max();
            int secondColumnWidth = items.Select(x => x.SecondColumnText.Length).Max();

            if (firstColumnWidth + secondColumnWidth + indent + indent > windowWidth)
            {
                int firstColumnMaxWidth = windowWidth / 2 - indent;
                if (firstColumnWidth > firstColumnMaxWidth)
                {
                    firstColumnWidth = items.SelectMany(x => WrapText(x.FirstColumnText, firstColumnMaxWidth).Select(x => x.Length)).Max();
                }
                secondColumnWidth = windowWidth - firstColumnWidth - indent - indent;
            }

            var ret = new List<string>();
            for (var i = 0; i < items.Count(); i++)
            {
                var helpItem = items.Skip(i).FirstOrDefault();
                if (helpItem is null) { continue; }

                IEnumerable<string> firstColumnParts = WrapText(helpItem.FirstColumnText, firstColumnWidth);
                IEnumerable<string> secondColumnParts = WrapText(helpItem.SecondColumnText, secondColumnWidth);

                string indentString = GetIndentString(indent);
                foreach (var (first, second) in ZipWithEmpty(firstColumnParts, secondColumnParts))
                {
                    var firstText = ($"{indentString}{first}");
                    var secondText = "";
                    if (!string.IsNullOrWhiteSpace(second))
                    {
                        int padSize = firstColumnWidth - first.Length;
                        string padding = "";
                        if (padSize > 0)
                        {
                            padding = new string(' ', padSize);
                        }

                        secondText = ($"{padding}{indentString}{second}");
                    }

                    ret.Add($"{firstText}{secondText}");
                }
            }
            return ret;

            static IEnumerable<(string, string)> ZipWithEmpty(IEnumerable<string> first, IEnumerable<string> second)
            {
                using var enum1 = first.GetEnumerator();
                using var enum2 = second.GetEnumerator();
                bool hasFirst = false, hasSecond = false;
                while ((hasFirst = enum1.MoveNext()) | (hasSecond = enum2.MoveNext()))
                {
                    yield return (hasFirst ? enum1.Current : "", hasSecond ? enum2.Current : "");
                }
            }
        }

        public static void WriteLines(IEnumerable<string>? output, HelpContext context)
        {
            if (output is null)
            { return; }
            foreach (var line in output)
            {
                context.Writer.WriteLine(line);
            }
        }

        public static string IndentText(string text, int indent)
        => $"{GetIndentString(indent)}{text}";

        public static void WriteBlankLine(HelpContext context)
        {
            context.Writer.WriteLine();
        }

        internal static IEnumerable<string>? WrapAndIndentText(object value, int maxWidth, int indent)
        {
            throw new NotImplementedException();
        }
    }
}
