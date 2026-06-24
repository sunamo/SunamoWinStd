namespace SunamoWinStd._sunamo.SunamoStringReplace;

internal class SHReplace
{
    internal static string ReplaceAll(string text, string replacement, params string[] patterns)
    {
        foreach (var item in patterns)
        {
            if (string.IsNullOrEmpty(item))
            {
                return text;
            }
        }

        foreach (var item in patterns)
        {
            text = text.Replace(item, replacement);
        }
        return text;
    }
}
