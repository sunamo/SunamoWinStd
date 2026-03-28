namespace SunamoWinStd._sunamo.SunamoArgs;

/// <summary>
/// Arguments for PowerShell command invocation, controlling output and caching behavior.
/// </summary>
internal class PsInvokeArgs
{
    /// <summary>
    /// Default instance with all default settings.
    /// </summary>
    internal static readonly PsInvokeArgs Def = new PsInvokeArgs();

    /// <summary>
    /// Whether to display a progress bar during execution.
    /// </summary>
    internal bool IsWritingProgressBar { get; set; } = false;

    /// <summary>
    /// Whether to write output immediately to status. Earlier was false.
    /// </summary>
    internal bool IsImmediatelyWritingToStatus { get; set; } = false;

    /// <summary>
    /// Commands to prepend before every PowerShell command.
    /// </summary>
    internal List<string>? AddBeforeEveryCommand { get; set; } = null;

    /// <summary>
    /// If the file exists, performs load and thus speeds up execution.
    /// If the file does not exist, executes commands and saves.
    /// Does not work with the last modification date in any way.
    /// </summary>
    internal string? PathToSaveLoadPsOutput { get; set; } = null;
}
