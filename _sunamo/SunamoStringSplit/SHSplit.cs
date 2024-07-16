namespace SunamoWinStd._sunamo.SunamoStringSplit;

internal class SHSplit
{
    internal static List<string> SplitByWhiteSpaces(string innerText)
    {
        return innerText.Split(AllChars.whiteSpacesChars.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
