namespace SunamoWinStd._sunamo.SunamoExceptions;

/// <summary>
/// Provides formatted exception message generation for various error conditions.
/// </summary>
internal sealed partial class Exceptions
{
    /// <summary>
    /// Formats the caller name as a prefix for error messages.
    /// Returns empty string if the caller name is null or whitespace.
    /// </summary>
    /// <param name="callerName">The caller's type and method name.</param>
    /// <returns>Formatted prefix string ending with ": " or empty string.</returns>
    internal static string FormatCallerPrefix(string callerName)
    {
        return string.IsNullOrWhiteSpace(callerName) ? string.Empty : callerName + ": ";
    }

    /// <summary>
    /// Builds a human-readable text from an exception and optionally its inner exceptions.
    /// </summary>
    /// <param name="exception">The exception to describe.</param>
    /// <param name="isIncludingInner">Whether to include inner exception messages.</param>
    /// <returns>Formatted exception text.</returns>
    internal static string TextOfExceptions(Exception exception, bool isIncludingInner = true)
    {
        if (exception == null) return string.Empty;
        StringBuilder stringBuilder = new();
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

    /// <summary>
    /// Determines the place of the exception in the call stack and returns the type name, method name, and stack trace.
    /// </summary>
    /// <param name="isFillAlsoFirstTwo">Whether to also fill the type and method name from the first non-ThrowEx frame.</param>
    /// <returns>Tuple of (typeName, methodName, stackTraceText).</returns>
    internal static Tuple<string, string, string> PlaceOfException(bool isFillAlsoFirstTwo = true)
    {
        StackTrace stackTrace = new();
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

    /// <summary>
    /// Extracts the type name and method name from a stack frame string.
    /// </summary>
    /// <param name="stackFrame">The stack frame text (e.g. "at Namespace.Type.Method()").</param>
    /// <param name="typeName">Output: the extracted type name.</param>
    /// <param name="methodName">Output: the extracted method name.</param>
    internal static void TypeAndMethodName(string stackFrame, out string typeName, out string methodName)
    {
        var afterAt = stackFrame.Split("at ")[1].Trim();
        var fullMethodPath = afterAt.Split("(")[0];
        var parts = fullMethodPath.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        methodName = parts[^1];
        parts.RemoveAt(parts.Count - 1);
        typeName = string.Join(".", parts);
    }

    /// <summary>
    /// Gets the name of the calling method at the specified stack depth.
    /// </summary>
    /// <param name="depth">The stack frame depth to inspect.</param>
    /// <returns>The calling method name, or an error message if not available.</returns>
    internal static string CallingMethod(int depth = 1)
    {
        StackTrace stackTrace = new();
        var methodBase = stackTrace.GetFrame(depth)?.GetMethod();
        if (methodBase == null)
        {
            return "Method name cannot be get";
        }
        var methodName = methodBase.Name;
        return methodName;
    }

    /// <summary>
    /// Generates an error message when a string argument is null, empty, or whitespace.
    /// </summary>
    /// <param name="callerName">The caller's type and method name.</param>
    /// <param name="argumentName">The name of the argument being checked.</param>
    /// <param name="argumentValue">The value of the argument being checked.</param>
    /// <param name="isDisallowingWhitespaceOnly">Whether whitespace-only values should also be reported.</param>
    /// <returns>Error message or null if the argument is valid.</returns>
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

    /// <summary>
    /// Builds the additional parameter info text from the inner and outer string builders.
    /// </summary>
    /// <returns>Formatted additional parameters text.</returns>
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

    /// <summary>
    /// Generates a custom error message with a caller prefix.
    /// </summary>
    /// <param name="callerName">The caller's type and method name.</param>
    /// <param name="message">The error message.</param>
    /// <returns>Formatted error message.</returns>
    internal static string? Custom(string callerName, string message)
    {
        return FormatCallerPrefix(callerName) + message;
    }

    /// <summary>
    /// Generates a "not implemented method" error message.
    /// </summary>
    /// <param name="callerName">The caller's type and method name.</param>
    /// <returns>Formatted error message.</returns>
    internal static string? NotImplementedMethod(string callerName)
    {
        return FormatCallerPrefix(callerName) + "Not implemented method.";
    }

    /// <summary>
    /// Generates an error message if the specified directory does not exist.
    /// </summary>
    /// <param name="callerName">The caller's type and method name.</param>
    /// <param name="fullPath">The directory path to check.</param>
    /// <returns>Error message or null if the directory exists.</returns>
    internal static string? DirectoryExists(string callerName, string fullPath)
    {
        return Directory.Exists(fullPath)
        ? null
        : FormatCallerPrefix(callerName) + " " + "does not exists" + ": " + fullPath;
    }

    /// <summary>
    /// Generates an error message if the parameter value appears to be URL-encoded.
    /// </summary>
    /// <param name="callerName">The caller's type and method name.</param>
    /// <param name="parameterValue">The parameter value to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Error message or null if the parameter is valid.</returns>
    internal static string? InvalidParameter(string callerName, string parameterValue, string parameterName)
    {
        return parameterValue != WebUtility.UrlDecode(parameterValue)
        ? FormatCallerPrefix(callerName) + parameterValue + " is url encoded " + parameterName
        : null;
    }

    /// <summary>
    /// Generates a "not implemented case" error message for the specified value.
    /// </summary>
    /// <param name="callerName">The caller's type and method name.</param>
    /// <param name="notImplementedName">The case value that is not implemented.</param>
    /// <returns>Formatted error message.</returns>
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
