namespace SunamoWinStd._sunamo.SunamoStringGetLines;

/// <summary>
/// Internal helper for splitting text into lines by various newline characters.
/// </summary>
internal class SHGetLines
{
    /// <summary>
    /// Splits the text into lines using all common newline character combinations.
    /// </summary>
    /// <param name="text">The text to split into lines.</param>
    /// <returns>List of lines.</returns>
    internal static List<string> GetLines(string? text)
    {
        if (text == null)
        {
            return new List<string>();
        }

        var lines = text.Split(new string[] { "\r\n", "\n\r" }, StringSplitOptions.None).ToList();
        SplitByUnixNewline(lines);
        return lines;
    }

    /// <summary>
    /// Further splits lines by standalone CR and LF characters.
    /// </summary>
    /// <param name="list">The list to split further.</param>
    private static void SplitByUnixNewline(List<string> list)
    {
        SplitBy(list, "\r");
        SplitBy(list, "\n");
    }

    /// <summary>
    /// Splits list entries that contain the specified separator into multiple entries.
    /// </summary>
    /// <param name="list">The list of strings to process.</param>
    /// <param name="separator">The separator to split by.</param>
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

    /// <summary>
    /// Replaces the element at the specified index with the reversed contents of the insert list.
    /// </summary>
    /// <param name="list">The target list to modify.</param>
    /// <param name="insertList">The list of elements to insert.</param>
    /// <param name="index">The index at which to perform the replacement.</param>
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
