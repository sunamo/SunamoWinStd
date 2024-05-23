namespace SunamoWinStd;

public class UH
{
    public static string AppendHttpIfNotExists(string p)
    {
        string p2 = p;
        if (!p.StartsWith("http"))
        {
            p2 = "http://" + p;
        }

        return p2;
    }

    public static string AppendHttpsIfNotExists(string p)
    {
        string p2 = p;
        if (!p.StartsWith("https"))
        {
            p2 = "https://" + p;
        }

        return p2;
    }
}
