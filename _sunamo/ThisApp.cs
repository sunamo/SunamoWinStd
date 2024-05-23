namespace SunamoWinStd;
public class ThisApp
{
    public static void Success(string v, params string[] o)
    {
        SetStatus(TypeOfMessage.Success, v, o);
    }

    public static void Info(string v, params string[] o)
    {
        SetStatus(TypeOfMessage.Information, v, o);
    }

    public static void Error(string v, params string[] o)
    {
        SetStatus(TypeOfMessage.Error, v, o);
    }

    public static void Warning(string v, params string[] o)
    {
        SetStatus(TypeOfMessage.Warning, v, o);
    }

    public static void Ordinal(string v, params string[] o)
    {
        SetStatus(TypeOfMessage.Ordinal, v, o);
    }

    public static void Appeal(string v, params string[] o)
    {
        SetStatus(TypeOfMessage.Appeal, v, o);
    }

    public static void SetStatus(TypeOfMessage st, string status, params string[] args)
    {
        var format = /*string.Format*/ string.Format(status, args);
        if (format.Trim() != string.Empty)
        {
            if (StatusSetted == null)
            {
                // For unit tests
                //////////DebugLogger.Instance.WriteLine(st + ": " + format);
            }
            else
            {
                StatusSetted(st, format);
            }
        }
    }

    public static event Action<TypeOfMessage, string> StatusSetted;
}
