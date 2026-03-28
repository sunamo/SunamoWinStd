namespace SunamoWinStd._sunamo.SunamoEnums.Enums;

/// <summary>
///     Message type classification. Error/Warning go to tbLastErrorOrWarning, others to tbLastOtherMessage.
///     Must be here because of cl which is withoutDep.
/// </summary>
internal enum TypeOfMessage
{
    /// <summary>
    ///     Error message, displayed in tbLastErrorOrWarning.
    /// </summary>
    Error,

    /// <summary>
    ///     Warning message, displayed in tbLastErrorOrWarning.
    /// </summary>
    Warning,
    /// <summary>
    ///     Informational message.
    /// </summary>
    Information,

    /// <summary>
    ///     Returned if from text the value cannot be determined.
    /// </summary>
    Ordinal,
    /// <summary>
    ///     Appeal type message.
    /// </summary>
    Appeal,
    /// <summary>
    ///     Success message.
    /// </summary>
    Success
}