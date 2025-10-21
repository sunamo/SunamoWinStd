// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoWinStd._sunamo.SunamoUri;

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
