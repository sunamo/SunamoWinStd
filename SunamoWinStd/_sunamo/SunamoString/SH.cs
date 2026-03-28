namespace SunamoWinStd._sunamo.SunamoString;

/// <summary>
/// Internal string helper utility methods.
/// </summary>
internal class SH
{
    /// <summary>
    /// Converts the first character of the text to upper case (ref overload).
    /// </summary>
    /// <param name="text">The text to modify.</param>
    internal static void FirstCharUpper(ref string text)
    {
        text = FirstCharUpper(text);
    }

    /// <summary>
    /// Returns the text with the first character converted to upper case.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <returns>The text with the first character upper-cased.</returns>
    internal static string FirstCharUpper(string text)
    {
        if (text.Length == 1)
        {
            return text.ToUpper();
        }

        string remainder = text.Substring(1);
        return text[0].ToString().ToUpper() + remainder;
    }

    /// <summary>
    /// Wraps the text in double-quote characters.
    /// </summary>
    /// <param name="text">The text to wrap.</param>
    /// <param name="isWrappingWhitespaceOrEmpty">Whether to wrap even if the text is whitespace or empty.</param>
    /// <returns>The quoted text.</returns>
    internal static string WrapWithQm(string text, bool isWrappingWhitespaceOrEmpty = true)
    {
        return WrapWithChar(text, '"', isWrappingWhitespaceOrEmpty);
    }

    /// <summary>
    /// Wraps the text with the specified character.
    /// </summary>
    /// <param name="text">The text to wrap.</param>
    /// <param name="wrapChar">The character to wrap with.</param>
    /// <param name="isTrimming">Whether to trim the text before wrapping.</param>
    /// <param name="isWrappingWhitespaceOrEmpty">Whether to wrap even if the text is whitespace or empty.</param>
    /// <returns>The wrapped text, or empty string if the text is whitespace/empty and wrapping is disabled.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string WrapWithChar(string text, char wrapChar, bool isTrimming = false, bool isWrappingWhitespaceOrEmpty = true)
    {
        if (string.IsNullOrWhiteSpace(text) && !isWrappingWhitespaceOrEmpty)
        {
            return string.Empty;
        }

        return WrapWith(isTrimming ? text.Trim() : text, wrapChar.ToString());
    }

    /// <summary>
    /// Wraps the text with the specified wrapper string on both sides.
    /// </summary>
    /// <param name="text">The text to wrap.</param>
    /// <param name="wrapper">The string to prepend and append.</param>
    /// <returns>The wrapped text.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string WrapWith(string text, string wrapper)
    {
        return wrapper + text + wrapper;
    }

    /// <summary>
    /// Returns a string representation of the value, or "(null)" if the value is null.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>String representation prefixed with a space.</returns>
    internal static string NullToStringOrDefault(object? value)
    {
        return value == null ? " " + "(null)" : " " + value;
    }
}
