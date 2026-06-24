namespace SunamoWinStd._sunamo.SunamoExceptions;

internal sealed partial class Exceptions
{
    internal static string FormatCallerPrefix(string callerName)
        => string.IsNullOrWhiteSpace(callerName) ? string.Empty : callerName + ": ";

    internal static string TextOfExceptions(Exception exception, bool isIncludingInner = true)
    {
        if (exception == null) return string.Empty;
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("Exception:");
        stringBuilder.AppendLine(exception.Message);
        if (isIncludingInner)
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                stringBuilder.AppendLine(exception.Message);
            }
        var result = stringBuilder.ToString();
        return result;
    }

    internal static Tuple<string, string, string> PlaceOfException(bool isFillAlsoFirstTwo = true)
    {
        var stackTrace = new StackTrace();
        var stackTraceText = stackTrace.ToString();
        var lines = stackTraceText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
        lines.RemoveAt(0);
        var i = 0;
        string typeName = string.Empty;
        string methodName = string.Empty;
        for (; i < lines.Count; i++)
        {
            var line = lines[i];
            if (isFillAlsoFirstTwo)
                if (!line.StartsWith("   at ThrowEx"))
                {
                    TypeAndMethodName(line, out typeName, out methodName);
                    isFillAlsoFirstTwo = false;
                }
            if (line.StartsWith("at System."))
            {
                lines.Add(string.Empty);
                lines.Add(string.Empty);
                break;
            }
        }
        return new Tuple<string, string, string>(typeName, methodName, string.Join(Environment.NewLine, lines));
    }

    internal static void TypeAndMethodName(string stackFrame, out string typeName, out string methodName)
    {
        var afterAt = stackFrame.Split("at ")[1].Trim();
        var fullMethodPath = afterAt.Split("(")[0];
        var parts = fullMethodPath.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        methodName = parts[^1];
        parts.RemoveAt(parts.Count - 1);
        typeName = string.Join(".", parts);
    }

    internal static string CallingMethod(int depth = 1)
    {
        var stackTrace = new StackTrace();
        var methodBase = stackTrace.GetFrame(depth)?.GetMethod();
        if (methodBase == null)
        {
            return "Method name cannot be get";
        }
        var methodName = methodBase.Name;
        return methodName;
    }

    internal static string? IsNullOrWhitespace(string callerName, string argumentName, string argumentValue, bool isDisallowingWhitespaceOnly)
    {
        string additionalParamsText;
        if (argumentValue == null)
        {
            additionalParamsText = AddParams();
            return FormatCallerPrefix(callerName) + argumentName + " is null" + additionalParamsText;
        }
        if (argumentValue == string.Empty)
        {
            additionalParamsText = AddParams();
            return FormatCallerPrefix(callerName) + argumentName + " is empty (without trim)" + additionalParamsText;
        }
        if (isDisallowingWhitespaceOnly && argumentValue.Trim() == string.Empty)
        {
            additionalParamsText = AddParams();
            return FormatCallerPrefix(callerName) + argumentName + " is empty (with trim)" + additionalParamsText;
        }
        return null;
    }

    private static readonly StringBuilder additionalInfoInnerStringBuilder = new();
    private static readonly StringBuilder additionalInfoStringBuilder = new();

    internal static string AddParams()
    {
        additionalInfoStringBuilder.Insert(0, Environment.NewLine);
        additionalInfoStringBuilder.Insert(0, "Outer:");
        additionalInfoStringBuilder.Insert(0, Environment.NewLine);
        additionalInfoInnerStringBuilder.Insert(0, Environment.NewLine);
        additionalInfoInnerStringBuilder.Insert(0, "Inner:");
        additionalInfoInnerStringBuilder.Insert(0, Environment.NewLine);
        var outerParamsText = additionalInfoStringBuilder.ToString();
        var innerParamsText = additionalInfoInnerStringBuilder.ToString();
        return outerParamsText + innerParamsText;
    }

    internal static string? Custom(string callerName, string message)
        => FormatCallerPrefix(callerName) + message;

    internal static string? NotImplementedMethod(string callerName)
        => FormatCallerPrefix(callerName) + "Not implemented method.";

    internal static string? DirectoryExists(string callerName, string fullPath)
        => Directory.Exists(fullPath)
            ? null
            : FormatCallerPrefix(callerName) + " " + "does not exists" + ": " + fullPath;

    internal static string? InvalidParameter(string callerName, string parameterValue, string parameterName)
        => parameterValue != WebUtility.UrlDecode(parameterValue)
            ? FormatCallerPrefix(callerName) + parameterValue + " is url encoded " + parameterName
            : null;

    internal static string? NotImplementedCase(string callerName, object notImplementedName)
    {
        var forMessage = string.Empty;
        if (notImplementedName != null)
        {
            forMessage = " for ";
            if (notImplementedName.GetType() == typeof(Type))
                forMessage += ((Type)notImplementedName).FullName;
            else
                forMessage += notImplementedName.ToString();
        }
        return FormatCallerPrefix(callerName) + "Not implemented case" + forMessage + " . internal program error. Please contact developer" +
        ".";
    }
}
