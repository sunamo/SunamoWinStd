namespace SunamoWinStd.Helpers;

/// <summary>
/// Represents the clipboard helper win std type.
/// </summary>
public class ClipboardHelperWinStd
{
    private static ClipboardHelperWinStd? instance = null;

    // Must be a method because it is passed as Func.
    /// <summary>
    /// Instance.
    /// </summary>
    /// <returns>The instance result.</returns>
    public static ClipboardHelperWinStd Instance()
    {
        instance ??= new ClipboardHelperWinStd();
        return instance;
    }

    /// <summary>
    /// Contains text.
    /// </summary>
    /// <returns>The contains text result.</returns>
    public bool ContainsText()
    {
        return true;
    }

    /// <summary>
    /// Get lines.
    /// </summary>
    /// <returns>The get lines result.</returns>
    public List<string> GetLines()
    {
        return SHGetLines.GetLines(GetText());
    }

    /// <summary>
    /// Get text.
    /// </summary>
    /// <returns>The get text result.</returns>
    public string? GetText()
    {
        return ClipboardService.GetText();
    }

    /// <summary>
    /// Set lines.
    /// </summary>
    /// <param name="list">The list.</param>
    public void SetLines(List<string> list)
    {
        SetText(string.Join(Environment.NewLine, list));
    }

    /// <summary>
    /// Set list.
    /// </summary>
    /// <param name="list">The list.</param>
    public void SetList(List<string> list)
    {
        SetText(string.Join(Environment.NewLine, list));
    }

    /// <summary>
    /// Set text.
    /// </summary>
    /// <param name="text">The text.</param>
    public void SetText(string text)
    {
        ClipboardService.SetText(text);
    }

    /// <summary>
    /// Set text.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    public void SetText(StringBuilder stringBuilder)
    {
        SetText(stringBuilder.ToString());
    }

    /// <summary>
    /// Set text 2.
    /// </summary>
    /// <param name="text">The text.</param>
    public void SetText2(string text)
    {
        SetText(text);
    }

    /// <summary>
    /// Set text 3.
    /// </summary>
    /// <param name="text">The text.</param>
    public void SetText3(string text)
    {
        SetText(text);
    }
}
