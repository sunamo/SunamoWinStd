namespace SunamoWinStd._sunamo;

internal class UH
{
    internal static string AppendHttpIfNotExists(string p)
    {
        string p2 = p;
        if (!p.StartsWith("http"))
        {
            p2 = "http://" + p;
        }

        return p2;
    }

    internal static string AppendHttpsIfNotExists(string p)
    {
        string p2 = p;
        if (!p.StartsWith("https"))
        {
            p2 = "https://" + p;
        }

        return p2;
    }
}
