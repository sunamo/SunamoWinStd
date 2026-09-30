namespace SunamoWinStd._sunamo.SunamoInterfaces.Interfaces;

/// <summary>
/// Interface for reading and writing the system clipboard.
/// </summary>
public interface IClipboardHelper
{
    /// <summary>
    /// Gets the text currently on the clipboard.
    /// </summary>
    /// <returns>The clipboard text, or an empty string if it could not be read.</returns>
    string GetText();

    /// <summary>
    /// Gets the clipboard text split into lines.
    /// </summary>
    /// <returns>The clipboard text split into lines.</returns>
    List<string> GetLines();

    /// <summary>
    /// Sets the clipboard text.
    /// </summary>
    /// <param name="text">The text to set.</param>
    void SetText(string text);

    /// <summary>
    /// Sets the clipboard text from a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The builder whose content is set to the clipboard.</param>
    void SetText(StringBuilder stringBuilder);

    /// <summary>
    /// Sets the clipboard text by joining the given lines with a newline.
    /// </summary>
    /// <param name="lines">The lines to join and set to the clipboard.</param>
    void SetLines(List<string> lines);

    /// <summary>
    /// Sets the clipboard text by joining the given items with a newline. Alias for <see cref="SetLines"/>.
    /// </summary>
    /// <param name="items">The items to join and set to the clipboard.</param>
    void SetList(List<string> items);

    /// <summary>
    /// Determines whether the clipboard currently contains text.
    /// </summary>
    /// <returns>True if the clipboard contains text.</returns>
    bool ContainsText();
}
