namespace SunamoWinStd._sunamo;

internal class WhitespaceCharService
{
    internal List<char>? WhiteSpaceChars;

    internal void ConvertWhiteSpaceCodesToChars()
    {
        if (WhiteSpaceChars != null)
        {
            return;
        }
        WhiteSpaceChars = new List<char>(WhiteSpacesCodes.Count);
        foreach (var item in WhiteSpacesCodes)
        {
            var text = char.ConvertFromUtf32(item);
            var charValue = Convert.ToChar(text);
            WhiteSpaceChars.Add(charValue);
        }
    }

    internal readonly List<int> WhiteSpacesCodes = new(new[]
    {
        9, 10, 11, 12, 13, 32, 133, 160, 5760, 6158, 8192, 8193, 8194, 8195, 8196, 8197, 8198, 8199, 8200, 8201, 8202,
        8232, 8233, 8239, 8287, 12288
    });
}
