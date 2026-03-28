namespace SunamoWinStd._sunamo.SunamoArgs;

/// <summary>
/// Configuration arguments for content change operations, controlling null/empty removal and argument order.
/// </summary>
internal class ChangeContentArgsWinStd
{
    /// <summary>
    /// Whether to remove null entries from the result.
    /// </summary>
    internal bool IsRemovingNull { get; set; } = false;

    /// <summary>
    /// Whether to remove empty (whitespace-only) entries from the result.
    /// </summary>
    internal bool IsRemovingEmpty { get; set; } = false;

    /// <summary>
    /// Whether to switch the order of the first and second arguments.
    /// </summary>
    internal bool IsSwitchingFirstAndSecondArg { get; set; } = false;
}
