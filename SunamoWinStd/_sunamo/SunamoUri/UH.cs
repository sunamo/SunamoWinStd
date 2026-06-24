namespace SunamoWinStd._sunamo.SunamoUri;

internal class UH
{
    internal static string AppendHttpIfNotExists(string url)
    {
        string result = url;
        if (!url.StartsWith("http"))
        {
            result = "http://" + url;
        }

        return result;
    }

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
