namespace SunamoWinStd._sunamo.SunamoThisApp;

internal class ThisApp
{
    internal static void Success(string text, params string[] args)
    {
        SetStatus(TypeOfMessage.Success, text, args);
    }

    internal static void Error(string text, params string[] args)
    {
        SetStatus(TypeOfMessage.Error, text, args);
    }

    internal static void Warning(string text, params string[] args)
    {
        SetStatus(TypeOfMessage.Warning, text, args);
    }

    internal static void SetStatus(TypeOfMessage typeOfMessage, string text, params string[] args)
    {
        var formattedText = string.Format(text, args);
        if (formattedText.Trim() != string.Empty)
        {
            StatusSet?.Invoke(typeOfMessage, formattedText);
        }
    }

    internal static event Action<TypeOfMessage, string>? StatusSet;
}
