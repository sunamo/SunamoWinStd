namespace SunamoWinStd._sunamo.SunamoStringSplit;

internal class SHSplit
{
    internal static List<string> SplitByWhiteSpaces(string text)
    {
        WhitespaceCharService whitespaceCharService = new();
        return text.Split(whitespaceCharService.WhiteSpaceChars!.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
