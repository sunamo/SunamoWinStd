namespace SunamoWinStd.Helpers;

/// <summary>
/// Clipboard helper for Windows using TextCopy library.
/// </summary>
public class ClipboardHelperWinStd
{
    private static ClipboardHelperWinStd? instance = null;
    /// <summary>
    /// Gets or creates the singleton instance. Must be a method because it is passed as Func.
    /// </summary>
    /// <returns>The singleton instance.</returns>
    public static ClipboardHelperWinStd Instance()
    {
        if (instance == null)
        {
            instance = new ClipboardHelperWinStd();
        }
        return instance;
    }
    /// <summary>
    /// Checks if the clipboard contains text. Always returns true for this implementation.
    /// </summary>
    /// <returns>True.</returns>
    public bool ContainsText()
    {
        return true;
    }

    /// <summary>
    /// Gets clipboard text split into lines.
    /// </summary>
    /// <returns>List of lines from clipboard text.</returns>
    public List<string> GetLines()
    {
        return SHGetLines.GetLines(GetText());
    }
    /// <summary>
    /// Gets the current clipboard text.
    /// </summary>
    /// <returns>Clipboard text or null.</returns>
    public string? GetText()
    {
        return ClipboardService.GetText();
    }
    /// <summary>
    /// Sets clipboard text from a list of lines joined by newlines.
    /// </summary>
    /// <param name="list">Lines to set.</param>
    public void SetLines(List<string> list)
    {
        SetText(string.Join(Environment.NewLine, list));
    }
    /// <summary>
    /// Sets clipboard text from a list joined by newlines.
    /// </summary>
    /// <param name="list">Items to set.</param>
    public void SetList(List<string> list)
    {
        SetText(string.Join(Environment.NewLine, list));
    }
    /// <summary>
    /// Sets clipboard text.
    /// </summary>
    /// <param name="text">Text to set.</param>
    public void SetText(string text)
    {
        ClipboardService.SetText(text);
    }
    /// <summary>
    /// Sets clipboard text from a StringBuilder.
    /// </summary>
    /// <param name="stringBuilder">StringBuilder containing the text.</param>
    public void SetText(StringBuilder stringBuilder)
    {
        SetText(stringBuilder.ToString());
    }
    /// <summary>
    /// Sets clipboard text (alternative method).
    /// </summary>
    /// <param name="text">Text to set.</param>
    public void SetText2(string text)
    {
        SetText(text);
    }
    /// <summary>
    /// Sets clipboard text (alternative method).
    /// </summary>
    /// <param name="text">Text to set.</param>
    public void SetText3(string text)
    {
        SetText(text);
    }
}
