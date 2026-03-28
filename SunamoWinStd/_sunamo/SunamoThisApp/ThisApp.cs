namespace SunamoWinStd._sunamo.SunamoThisApp;

/// <summary>
/// Provides application-level status reporting through a centralized event.
/// </summary>
internal class ThisApp
{
    /// <summary>
    /// Reports a success status message.
    /// </summary>
    /// <param name="text">The format string for the message.</param>
    /// <param name="args">Arguments for the format string.</param>
    internal static void Success(string text, params string[] args)
    {
        SetStatus(TypeOfMessage.Success, text, args);
    }

    /// <summary>
    /// Reports an error status message.
    /// </summary>
    /// <param name="text">The format string for the message.</param>
    /// <param name="args">Arguments for the format string.</param>
    internal static void Error(string text, params string[] args)
    {
        SetStatus(TypeOfMessage.Error, text, args);
    }

    /// <summary>
    /// Reports a warning status message.
    /// </summary>
    /// <param name="text">The format string for the message.</param>
    /// <param name="args">Arguments for the format string.</param>
    internal static void Warning(string text, params string[] args)
    {
        SetStatus(TypeOfMessage.Warning, text, args);
    }

    /// <summary>
    /// Sets the application status by formatting the text and raising the StatusSet event.
    /// </summary>
    /// <param name="typeOfMessage">The type of status message.</param>
    /// <param name="text">The format string for the message.</param>
    /// <param name="args">Arguments for the format string.</param>
    internal static void SetStatus(TypeOfMessage typeOfMessage, string text, params string[] args)
    {
        var formattedText = string.Format(text, args);
        if (formattedText.Trim() != string.Empty)
        {
            StatusSet?.Invoke(typeOfMessage, formattedText);
        }
    }

    /// <summary>
    /// Event raised when the application status changes.
    /// </summary>
    internal static event Action<TypeOfMessage, string>? StatusSet;
}
