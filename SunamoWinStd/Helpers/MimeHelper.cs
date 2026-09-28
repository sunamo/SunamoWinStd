namespace SunamoWinStd.Helpers;

/// <summary>
/// Detects a file's MIME type from its content using the Windows URL Moniker API.
/// </summary>
public class MimeHelper
{
    /// <summary>
    /// Gets the MIME type of a file by sniffing its first bytes with <c>FindMimeFromData</c>.
    /// </summary>
    /// <param name="filename">Path of the file to inspect.</param>
    /// <returns>The detected MIME type, or "unknown/unknown" if it could not be determined.</returns>
    public static string GetMimeFromFile(string filename)
    {
        if (!File.Exists(filename)) throw new FileNotFoundException(filename + " not found", filename);

        var buffer = new byte[256];
        using (var fileStream = new FileStream(filename, FileMode.Open))
        {
            var bytesToRead = fileStream.Length >= 256 ? 256 : (int)fileStream.Length;
            fileStream.Read(buffer, 0, bytesToRead);
        }

        try
        {
            W32.FindMimeFromData(0, null!, buffer, 256, null!, 0, out var mimeTypePointerValue, 0);
            var mimeTypePointer = new IntPtr(mimeTypePointerValue);
            var mime = Marshal.PtrToStringUni(mimeTypePointer);
            Marshal.FreeCoTaskMem(mimeTypePointer);
            return mime ?? "unknown/unknown";
        }
        catch (Exception)
        {
            return "unknown/unknown";
        }
    }
}
