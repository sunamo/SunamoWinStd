namespace SunamoWinStd._sunamo.SunamoCollections;

internal class CA
{

    internal static List<string> ToLower(List<string> slova)
    {
        for (int i = 0; i < slova.Count; i++)
        {
            slova[i] = slova[i].ToLower();
        }
        return slova;
    }


}