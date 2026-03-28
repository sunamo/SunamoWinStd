namespace SunamoWinStd._sunamo;

/// <summary>
/// Provides a list of Unicode whitespace characters derived from their code points.
/// </summary>
internal class WhitespaceCharService
{
    /// <summary>
    /// List of whitespace characters converted from Unicode code points.
    /// Populated by <see cref="ConvertWhiteSpaceCodesToChars"/>.
    /// </summary>
    internal List<char>? WhiteSpaceChars;

    /// <summary>
    /// Converts all whitespace code points to their character representations.
    /// Only runs once; subsequent calls are no-ops.
    /// </summary>
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

    /// <summary>
    /// Unicode code points for all recognized whitespace characters.
    /// </summary>
    internal readonly List<int> WhiteSpacesCodes = new(new[]
    {
        9, 10, 11, 12, 13, 32, 133, 160, 5760, 6158, 8192, 8193, 8194, 8195, 8196, 8197, 8198, 8199, 8200, 8201, 8202,
        8232, 8233, 8239, 8287, 12288
    });
}
