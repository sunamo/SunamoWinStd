namespace SunamoWinStd._sunamo.SunamoArgs;

internal class PsInvokeArgs
{
    internal static readonly PsInvokeArgs Def = new PsInvokeArgs();

    internal bool IsWritingProgressBar { get; set; } = false;

    // Whether to write output immediately to status. Earlier was false.
    internal bool IsImmediatelyWritingToStatus { get; set; } = false;

    internal List<string>? AddBeforeEveryCommand { get; set; } = null;

    // If the file exists, performs load and thus speeds up execution.
    // If the file does not exist, executes commands and saves.
    // Does not work with the last modification date in any way.
    internal string? PathToSaveLoadPsOutput { get; set; } = null;
}
