namespace SunamoWinStd._sunamo.SunamoStringGetLines;

internal class SHGetLines
{
    internal static List<string> GetLines(string? text)
    {
        if (text is null)
        {
            return new List<string>();
        }

        var lines = text.Split(new string[] { "\r\n", "\n\r" }, StringSplitOptions.None).ToList();
        SplitByUnixNewline(lines);
        return lines;
    }

    private static void SplitByUnixNewline(List<string> list)
    {
        SplitBy(list, "\r");
        SplitBy(list, "\n");
    }

    private static void SplitBy(List<string> list, string separator)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (separator == "\r")
            {
                var crlfSplit = list[i].Split(new string[] { "\r\n" }, StringSplitOptions.None);
                var lfcrSplit = list[i].Split(new string[] { "\n\r" }, StringSplitOptions.None);

                if (crlfSplit.Length > 1)
                {
                    ThrowEx.Custom("cannot contain any \\r\\n, pass already split by this pattern");
                }
                else if (lfcrSplit.Length > 1)
                {
                    ThrowEx.Custom("cannot contain any \\n\\r, pass already split by this pattern");
                }
            }

            var splitParts = list[i].Split(new string[] { separator }, StringSplitOptions.None);

            if (splitParts.Length > 1)
            {
                InsertOnIndex(list, splitParts.ToList(), i);
            }
        }
    }

    private static void InsertOnIndex(List<string> list, List<string> insertList, int index)
    {
        insertList.Reverse();

        list.RemoveAt(index);

        foreach (var item in insertList)
        {
            list.Insert(index, item);
        }
    }
}
