namespace SunamoWinStd._sunamo.SunamoStringSplit;

/// <summary>
/// Internal string splitting utility methods.
/// </summary>
internal class SHSplit
{
    /// <summary>
    /// Splits the text by all Unicode whitespace characters, removing empty entries.
    /// </summary>
    /// <param name="text">The text to split.</param>
    /// <returns>List of non-empty parts.</returns>
    internal static List<string> SplitByWhiteSpaces(string text)
    {
        WhitespaceCharService whitespaceCharService = new();
        return text.Split(whitespaceCharService.WhiteSpaceChars!.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
