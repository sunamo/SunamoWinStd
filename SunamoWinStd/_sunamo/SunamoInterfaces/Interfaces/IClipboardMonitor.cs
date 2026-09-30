namespace SunamoWinStd._sunamo.SunamoInterfaces.Interfaces;

/// <summary>
/// Interface for clipboard monitoring operations.
/// </summary>
public interface IClipboardMonitor
{
    /// <summary>
    /// Whether after copy to clipboard from any source allow monitoring.
    /// </summary>
    bool? AfterSet { get; set; }

    /// <summary>
    /// Permanently blocks clipboard monitoring.
    /// </summary>
    bool PermanentlyBlock { get; set; }
}