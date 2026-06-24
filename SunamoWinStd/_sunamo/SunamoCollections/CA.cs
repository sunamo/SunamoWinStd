namespace SunamoWinStd._sunamo.SunamoCollections;

internal class CA
{
    internal static List<string> ToLower(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = list[i].ToLower();
        }
        return list;
    }
}
