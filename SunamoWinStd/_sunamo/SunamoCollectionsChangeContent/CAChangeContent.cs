namespace SunamoWinStd._sunamo.SunamoCollectionsChangeContent;

/// <summary>
/// Internal collection content change utility methods.
/// </summary>
internal class CAChangeContent
{
    /// <summary>
    /// Applies a transformation function to every element in the list and optionally removes null/empty entries.
    /// </summary>
    /// <param name="args">Configuration for null/empty removal. Can be null.</param>
    /// <param name="list">The list to transform.</param>
    /// <param name="func">The transformation function to apply to each element.</param>
    /// <returns>The transformed list.</returns>
    internal static List<string> ChangeContent0(ChangeContentArgsWinStd? args, List<string> list, Func<string, string> func)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = func.Invoke(list[i]);
        }

        RemoveNullOrEmpty(args, list);

        return list;
    }

    /// <summary>
    /// Removes null and/or empty entries from the list based on the configuration.
    /// </summary>
    /// <param name="args">Configuration specifying which entries to remove.</param>
    /// <param name="list">The list to clean up.</param>
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
