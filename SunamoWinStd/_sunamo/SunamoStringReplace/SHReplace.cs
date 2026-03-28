namespace SunamoWinStd._sunamo.SunamoStringReplace;

/// <summary>
/// Internal string replacement utility methods.
/// </summary>
internal class SHReplace
{
    /// <summary>
    /// Replaces all occurrences of the specified patterns in the text with the replacement string.
    /// Returns early if any pattern is null or empty.
    /// </summary>
    /// <param name="text">The source text.</param>
    /// <param name="replacement">The replacement string.</param>
    /// <param name="patterns">The patterns to replace.</param>
    /// <returns>The text with all patterns replaced.</returns>
    internal static string ReplaceAll(string text, string replacement, params string[] patterns)
    {
        foreach (var item in patterns)
        {
            if (string.IsNullOrEmpty(item))
            {
                return text;
            }
        }

        foreach (var item in patterns)
        {
            text = text.Replace(item, replacement);
        }
        return text;
    }
}
