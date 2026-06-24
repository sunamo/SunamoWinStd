namespace SunamoWinStd._sunamo.SunamoCollectionsChangeContent;

internal class CAChangeContent
{
    internal static List<string> ChangeContent0(ChangeContentArgsWinStd? args, List<string> list, Func<string, string> func)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = func.Invoke(list[i]);
        }

        RemoveNullOrEmpty(args, list);

        return list;
    }

    private static void RemoveNullOrEmpty(ChangeContentArgsWinStd? args, List<string> list)
    {
        if (args != null)
        {
            if (args.IsRemovingNull)
            {
                list.Remove(null!);
            }

            if (args.IsRemovingEmpty)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].Trim() == string.Empty)
                    {
                        list.RemoveAt(i);
                    }
                }
            }
        }
    }
}
