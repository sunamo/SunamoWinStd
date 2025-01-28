namespace SunamoWinStd._sunamo.SunamoCollections;

internal class CA
{
    internal static void InitFillWith<T>(List<T> datas, int pocet, T initWith)
    {
        for (int i = 0; i < pocet; i++)
        {
            datas.Add(initWith);
        }
    }

    internal static List<string> ToLower(List<string> slova)
    {
        for (int i = 0; i < slova.Count; i++)
        {
            slova[i] = slova[i].ToLower();
        }
        return slova;
    }

    static string Replace(string s, string from, string to)
    {
        return s.Replace(from, to);
    }

}