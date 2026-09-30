using System.Collections.Specialized;
using System.Windows.Forms;
using Clipboard = System.Windows.Forms.Clipboard;
using SunamoWinStd._sunamo.SunamoInterfaces.Interfaces;

namespace SunamoWinStd.Win;

/// <summary>
/// WinForms/raw Win32 clipboard implementation of <see cref="IClipboardHelper"/>.
/// Prefer <see cref="Helpers.ClipboardHelperWinStd"/> (TextCopy-based) for most projects;
/// use this one only when the lower-level Win32 clipboard access it provides is needed
/// (e.g. together with <see cref="IClipboardMonitor"/>, which
/// relies on the same W32 clipboard APIs). Only available on Windows (WinForms) targets.
/// </summary>
public class ClipboardHelperWin : IClipboardHelper
{
    /// <summary>Unicode text clipboard format.</summary>
    public const uint CF_UNICODETEXT = 13U;

    /// <summary>Display text clipboard format.</summary>
    public const uint CF_DSPTEXT = 0x0081;

    /// <summary>Locale clipboard format.</summary>
    public const uint CF_LOCALE = 16U;

    /// <summary>OEM text clipboard format.</summary>
    public const uint CF_OEMTEXT = 7U;

    /// <summary>ANSI text clipboard format.</summary>
    public const uint CF_TEXT = 1U;

    /// <summary>
    /// Optional monitor whose <see cref="IClipboardMonitor.AfterSet"/> is reset before writes,
    /// so the monitor does not treat this class's own writes as an external clipboard change.
    /// </summary>
    public IClipboardMonitor? ClipboardMonitor { get; set; }

    /// <summary>
    /// Shared default instance.
    /// </summary>
    public static ClipboardHelperWin Instance { get; private set; } = new();

    /// <summary>
    /// Gets the shared instance, creating it if necessary.
    /// </summary>
    /// <returns>The shared <see cref="ClipboardHelperWin"/> instance.</returns>
    public static IClipboardHelper CreateInstance()
    {
        return Instance;
    }

    /// <summary>
    /// Sets the clipboard text using the managed WinForms API.
    /// </summary>
    /// <param name="text">The text to set.</param>
    public void SetText(string text)
    {
        if (ClipboardMonitor is not null) ClipboardMonitor.AfterSet = null;
        Clipboard.SetText(text);
    }

    /// <summary>
    /// Reads Unicode text directly from the clipboard via the raw Win32 API, bypassing WinForms.
    /// </summary>
    /// <returns>The clipboard text, or null if it could not be read.</returns>
    public static string? GetTextW32()
    {
        if (!W32.IsClipboardFormatAvailable(CF_UNICODETEXT)) return null;

        try
        {
            try
            {
                if (!W32.OpenClipboard(IntPtr.Zero)) return null;
            }
            catch (Exception)
            {
                return null;
            }

            IntPtr dataHandle;
            try
            {
                dataHandle = W32.GetClipboardData(CF_UNICODETEXT);
                if (dataHandle == IntPtr.Zero) return null;
            }
            catch (Exception)
            {
                return null;
            }

            var dataPointer = IntPtr.Zero;
            try
            {
                dataPointer = W32.GlobalLock(dataHandle);
                if (dataPointer == IntPtr.Zero) return null;

                var size = (int)W32.GlobalSize(dataHandle);
                var buffer = new byte[size];
                Marshal.Copy(dataPointer, buffer, 0, size);
                return Encoding.Unicode.GetString(buffer).TrimEnd('\0');
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (dataPointer != IntPtr.Zero) W32.GlobalUnlock(dataHandle);
            }
        }
        finally
        {
            W32.CloseClipboard();
        }
    }

    /// <summary>
    /// Gets the clipboard text via <see cref="GetTextW32"/>.
    /// </summary>
    /// <returns>The clipboard text, or an empty string if it could not be read.</returns>
    public string GetText()
    {
        try
        {
            return GetTextW32() ?? string.Empty;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Sets the clipboard text via the raw Win32 API by retrying <c>OpenClipboard</c> a few times.
    /// Prefer <see cref="SetText(string)"/>; this exists for cases where the managed API fails silently.
    /// </summary>
    /// <param name="text">The text to set.</param>
    public void SetTextW32(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        for (var attempt = 0; attempt < 10; attempt++)
        {
            try
            {
                W32.OpenClipboard(IntPtr.Zero);
                break;
            }
            catch (Exception)
            {
                // Retry.
            }

            Thread.Sleep(10);
        }

        var textPointer = Marshal.StringToHGlobalUni(text);
        W32.SetClipboardData(CF_UNICODETEXT, textPointer);
        W32.CloseClipboard();
        Marshal.FreeHGlobal(textPointer);
    }

    /// <summary>
    /// Gets the clipboard text split into lines.
    /// </summary>
    /// <returns>The clipboard text split into lines.</returns>
    public List<string> GetLines()
    {
        return SHGetLines.GetLines(GetText());
    }

    /// <summary>
    /// Sets the clipboard text by joining the given items with a newline. Alias for <see cref="SetLines"/>.
    /// </summary>
    /// <param name="items">The items to join and set to the clipboard.</param>
    public void SetList(List<string> items)
    {
        SetLines(items);
    }

    /// <summary>
    /// Sets the clipboard text by joining the given lines with a newline.
    /// </summary>
    /// <param name="lines">The lines to join and set to the clipboard.</param>
    public void SetLines(List<string> lines)
    {
        SetText(string.Join(Environment.NewLine, lines));
    }

    /// <summary>
    /// Cuts the given files: puts them on the clipboard as a file drop list flagged for move
    /// (Explorer paste behaves as "cut" rather than "copy").
    /// </summary>
    /// <param name="filePaths">Full paths of the files to cut.</param>
    public void CutFiles(params string[] filePaths)
    {
        byte[] moveEffect = [2, 0, 0, 0];
        var dropEffectStream = new MemoryStream();
        dropEffectStream.Write(moveEffect, 0, moveEffect.Length);

        var filesToCut = new StringCollection();
        filesToCut.AddRange(filePaths);

        var data = new DataObject("Preferred DropEffect", dropEffectStream);
        data.SetFileDropList(filesToCut);

        Clipboard.Clear();
        Clipboard.SetDataObject(data, true);
    }

    /// <summary>
    /// Sets the clipboard text from a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The builder whose content is set to the clipboard.</param>
    public void SetText(StringBuilder stringBuilder)
    {
        SetText(stringBuilder.ToString());
    }

    /// <summary>
    /// Sets the clipboard text via <see cref="SetTextW32"/> on a dedicated STA thread.
    /// </summary>
    /// <param name="text">The text to set.</param>
    public void SetTextOnStaThread(string text)
    {
        if (ClipboardMonitor is not null) ClipboardMonitor.AfterSet = null;

        var thread = new Thread(() => SetTextW32(text));
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
    }

    /// <summary>
    /// Determines whether the clipboard currently contains text.
    /// </summary>
    /// <returns>True if the clipboard contains text.</returns>
    public bool ContainsText()
    {
        return Clipboard.ContainsText();
    }
}
