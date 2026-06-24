namespace SunamoWinStd._sunamo.SunamoEnums.Enums;

// Message type classification. Error/Warning go to tbLastErrorOrWarning, others to tbLastOtherMessage.
// Must be here because of cl which is withoutDep.
internal enum TypeOfMessage
{
    Error,
    Warning,
    Information,
    // Returned if from text the value cannot be determined.
    Ordinal,
    Appeal,
    Success
}
