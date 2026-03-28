namespace SunamoWinStd._sunamo.SunamoUri;

/// <summary>
/// Internal URI helper utility methods.
/// </summary>
internal class UH
{
    /// <summary>
    /// Prepends "http://" to the URL if it does not already start with "http".
    /// </summary>
    /// <param name="url">The URL to process.</param>
    /// <returns>The URL with "http://" prefix ensured.</returns>
    internal static string AppendHttpIfNotExists(string url)
    {
        string result = url;
        if (!url.StartsWith("http"))
        {
            result = "http://" + url;
        }

        return result;
    }

    /// <summary>
    /// Prepends "https://" to the URL if it does not already start with "https".
    /// </summary>
    /// <param name="url">The URL to process.</param>
    /// <returns>The URL with "https://" prefix ensured.</returns>
    internal static string AppendHttpsIfNotExists(string url)
    {
        string result = url;
        if (!url.StartsWith("https"))
        {
            result = "https://" + url;
        }

        return result;
    }
}
