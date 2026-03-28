namespace SunamoWinStd._sunamo.SunamoArgs;

/// <summary>
/// Arguments for process invocation, specifying the working directory.
/// </summary>
internal class InvokeProcessArgs
{
    /// <summary>
    /// The working directory for the invoked process. Null uses the default.
    /// </summary>
    internal string? WorkingDirectory { get; set; } = null;
}
