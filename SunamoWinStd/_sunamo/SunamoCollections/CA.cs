namespace SunamoWinStd._sunamo.SunamoCollections;

/// <summary>
/// Internal collection utility methods.
/// </summary>
internal class CA
{
    /// <summary>
    /// Converts all strings in the list to lower case in-place.
    /// </summary>
    /// <param name="list">The list of strings to convert.</param>
    /// <returns>The same list with all entries converted to lower case.</returns>
    internal static List<string> ToLower(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = list[i].ToLower();
        }
        return list;
    }
}
