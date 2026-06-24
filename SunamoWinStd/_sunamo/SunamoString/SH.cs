namespace SunamoWinStd._sunamo.SunamoString;

internal class SH
{
    internal static void FirstCharUpper(ref string text)
    {
        text = FirstCharUpper(text);
    }

    internal static string FirstCharUpper(string text)
    {
        if (text.Length == 1)
        {
            return text.ToUpper();
        }

        string remainder = text.Substring(1);
        return text[0].ToString().ToUpper() + remainder;
    }

    internal static string WrapWithQm(string text, bool isWrappingWhitespaceOrEmpty = true)
        => WrapWithChar(text, '"', isWrappingWhitespaceOrEmpty);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string WrapWithChar(string text, char wrapChar, bool isTrimming = false, bool isWrappingWhitespaceOrEmpty = true)
    {
        if (string.IsNullOrWhiteSpace(text) && !isWrappingWhitespaceOrEmpty)
        {
            return string.Empty;
        }

        return WrapWith(isTrimming ? text.Trim() : text, wrapChar.ToString());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string WrapWith(string text, string wrapper)
        => wrapper + text + wrapper;

    internal static string NullToStringOrDefault(object? value)
        => value is null ? " " + "(null)" : " " + value;
}
