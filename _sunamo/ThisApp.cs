﻿using SunamoTypeOfMessage;

namespace SunamoWinStd._sunamo;
internal class ThisApp
{
    internal static void Success(string v, params string[] o)
    {
        SetStatus(TypeOfMessage.Success, v, o);
    }

    internal static void Info(string v, params string[] o)
    {
        SetStatus(TypeOfMessage.Information, v, o);
    }

    internal static void Error(string v, params string[] o)
    {
        SetStatus(TypeOfMessage.Error, v, o);
    }

    internal static void Warning(string v, params string[] o)
    {
        SetStatus(TypeOfMessage.Warning, v, o);
    }

    internal static void Ordinal(string v, params string[] o)
    {
        SetStatus(TypeOfMessage.Ordinal, v, o);
    }

    internal static void Appeal(string v, params string[] o)
    {
        SetStatus(TypeOfMessage.Appeal, v, o);
    }

    internal static void SetStatus(TypeOfMessage st, string status, params string[] args)
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

    internal static event Action<TypeOfMessage, string> StatusSetted;
}
