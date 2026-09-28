using System.Collections.Specialized;
using System.Windows.Forms;

namespace SunamoWinStd.Win;

/// <summary>
/// Runs WinForms <see cref="Clipboard"/> reads on a dedicated STA thread, so they can be
/// safely called from a non-STA thread (WinForms clipboard access requires STA).
/// Only available on Windows (WinForms) targets.
/// </summary>
public sealed class ClipboardAsync
{
    private string clipboardText = string.Empty;

    private void ReadText(object? format)
    {
        try
        {
            clipboardText = format is null ? Clipboard.GetText() : Clipboard.GetText((TextDataFormat)format);
        }
        catch (Exception)
        {
            clipboardText = string.Empty;
        }
    }

    /// <summary>
    /// Gets the clipboard text on a dedicated STA thread.
    /// </summary>
    /// <returns>The clipboard text, or an empty string if it could not be read.</returns>
    public string GetText()
    {
        var instance = new ClipboardAsync();
        var staThread = new Thread(instance.ReadText);
        staThread.SetApartmentState(ApartmentState.STA);
        staThread.Start();
        staThread.Join();
        return instance.clipboardText;
    }

    /// <summary>
    /// Gets the clipboard text in the given format on a dedicated STA thread.
    /// </summary>
    /// <param name="format">The clipboard text format to read.</param>
    /// <returns>The clipboard text, or an empty string if it could not be read.</returns>
    public string GetText(TextDataFormat format)
    {
        var instance = new ClipboardAsync();
        var staThread = new Thread(instance.ReadText);
        staThread.SetApartmentState(ApartmentState.STA);
        staThread.Start(format);
        staThread.Join();
        return instance.clipboardText;
    }

    private bool containsText;

    private void ReadContainsText(object? format)
    {
        try
        {
            containsText = format is null ? Clipboard.ContainsText() : Clipboard.ContainsText((TextDataFormat)format);
        }
        catch (Exception)
        {
            containsText = false;
        }
    }

    /// <summary>
    /// Determines whether the clipboard currently contains text, on a dedicated STA thread.
    /// </summary>
    /// <returns>True if the clipboard contains text.</returns>
    public bool ContainsText()
    {
        var instance = new ClipboardAsync();
        var staThread = new Thread(instance.ReadContainsText);
        staThread.SetApartmentState(ApartmentState.STA);
        staThread.Start();
        staThread.Join();
        return instance.containsText;
    }

    /// <summary>
    /// Determines whether the clipboard currently contains text in the given format, on a dedicated STA thread.
    /// </summary>
    /// <param name="format">The clipboard text format to check.</param>
    /// <returns>True if the clipboard contains text in that format.</returns>
    public bool ContainsText(TextDataFormat format)
    {
        var instance = new ClipboardAsync();
        var staThread = new Thread(instance.ReadContainsText);
        staThread.SetApartmentState(ApartmentState.STA);
        staThread.Start(format);
        staThread.Join();
        return instance.containsText;
    }

    private bool containsFileDropList;

    private void ReadContainsFileDropList(object? format)
    {
        try
        {
            containsFileDropList = Clipboard.ContainsFileDropList();
        }
        catch (Exception)
        {
            containsFileDropList = false;
        }
    }

    /// <summary>
    /// Determines whether the clipboard currently contains a file drop list, on a dedicated STA thread.
    /// </summary>
    /// <returns>True if the clipboard contains a file drop list.</returns>
    public bool ContainsFileDropList()
    {
        var instance = new ClipboardAsync();
        var staThread = new Thread(instance.ReadContainsFileDropList);
        staThread.SetApartmentState(ApartmentState.STA);
        staThread.Start();
        staThread.Join();
        return instance.containsFileDropList;
    }

    private StringCollection? fileDropList;

    private void ReadFileDropList()
    {
        try
        {
            fileDropList = Clipboard.GetFileDropList();
        }
        catch (Exception)
        {
            fileDropList = null;
        }
    }

    /// <summary>
    /// Gets the clipboard's file drop list on a dedicated STA thread.
    /// </summary>
    /// <returns>The file drop list, or null if it could not be read.</returns>
    public StringCollection? GetFileDropList()
    {
        var instance = new ClipboardAsync();
        var staThread = new Thread(instance.ReadFileDropList);
        staThread.SetApartmentState(ApartmentState.STA);
        staThread.Start();
        staThread.Join();
        return instance.fileDropList;
    }
}
