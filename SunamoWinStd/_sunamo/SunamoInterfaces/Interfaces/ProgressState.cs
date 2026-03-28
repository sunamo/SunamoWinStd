namespace SunamoWinStd._sunamo.SunamoInterfaces.Interfaces;

/// <summary>
/// Tracks the registration state and count for progress reporting.
/// </summary>
internal class ProgressState
{
    /// <summary>
    /// Whether this progress state has been registered with a progress reporter.
    /// </summary>
    internal bool IsRegistered { get; set; } = false;

    /// <summary>
    /// The current count for progress tracking.
    /// </summary>
    internal int Count { get; set; } = 0;
}
