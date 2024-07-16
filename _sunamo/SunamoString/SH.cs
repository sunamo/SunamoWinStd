namespace SunamoWinStd._sunamo.SunamoString;

internal class SH
{
    internal static void FirstCharUpper(ref string nazevPP)
    {
        nazevPP = FirstCharUpper(nazevPP);
    }


    internal static string FirstCharUpper(string nazevPP)
    {
        if (nazevPP.Length == 1)
        {
            return nazevPP.ToUpper();
        }

        string sb = nazevPP.Substring(1);
        return nazevPP[0].ToString().ToUpper() + sb;
    }

    internal static string WrapWithQm(string commitMessage, bool alsoIfIsWhitespaceOrEmpty = true)
    {
        return WrapWithChar(commitMessage, AllChars.qm, alsoIfIsWhitespaceOrEmpty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string WrapWithChar(string value, char v, bool _trimWrapping = false, bool alsoIfIsWhitespaceOrEmpty = true)
    {
        if (string.IsNullOrWhiteSpace(value) && !alsoIfIsWhitespaceOrEmpty)
        {
            return string.Empty;
        }

        // TODO: Make with StringBuilder, because of WordAfter and so
        return WrapWith(_trimWrapping ? value.Trim() : value, v.ToString());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string WrapWith(string value, string h)
    {
        return h + value + h;
    }

    internal static string GetTextBetweenSimple(string p, string after, string before, bool throwExceptionIfNotContains = true)
    {
        int dxOfFounded = int.MinValue;
        var t = GetTextBetween(p, after, before, out dxOfFounded, 0, throwExceptionIfNotContains);
        return t;
    }

    internal static string GetTextBetween(string p, string after, string before, out int dxOfFounded, int startSearchingAt, bool throwExceptionIfNotContains = true)
    {
        string vr = null;
        dxOfFounded = p.IndexOf(after, startSearchingAt);
        int p3 = p.IndexOf(before, dxOfFounded + after.Length);
        bool b2 = dxOfFounded != -1;
        bool b3 = p3 != -1;
        if (b2 && b3)
        {
            dxOfFounded += after.Length;
            p3 -= 1;
            // When I return between ( ), there must be +1
            var length = p3 - dxOfFounded + 1;
            if (length < 1)
            {
                // Takhle to tu bylo předtím ale logicky je to nesmysl.
                //return p;
            }
            vr = p.Substring(dxOfFounded, length).Trim();
        }
        else
        {
            if (throwExceptionIfNotContains)
            {
                ThrowEx.NotContains(p, after, before);
            }
            else
            {
                // 24-1-21 return null instead of p
                return null;
                //vr = p;
            }
        }

        return vr.Trim();
    }

    internal static string JoinNL(List<string> l)
    {
        StringBuilder sb = new();
        foreach (var item in l) sb.AppendLine(item);
        var r = string.Empty;
        r = sb.ToString();
        return r;
    }
    internal static List<string> SplitCharMore(string s, params char[] dot)
    {
        return s.Split(dot, StringSplitOptions.RemoveEmptyEntries).ToList();
    }
    internal static List<string> SplitMore(string s, params string[] dot)
    {
        return s.Split(dot, StringSplitOptions.RemoveEmptyEntries).ToList();
    }
    internal static List<string> SplitNone(string text, params string[] deli)
    {
        return text.Split(deli, StringSplitOptions.None).ToList();
    }
    internal static string NullToStringOrDefault(object n)
    {
        
        return n == null ? " " + Consts.nulled : AllStrings.space + n;
    }
    internal static string TrimEnd(string name, string ext)
    {
        while (name.EndsWith(ext)) return name.Substring(0, name.Length - ext.Length);
        return name;
    }
}