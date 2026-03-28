namespace SunamoWinStd._sunamo.SunamoExceptions;

/// <summary>
/// Provides methods that check conditions and throw exceptions when they fail.
/// </summary>
internal partial class ThrowEx
{
    /// <summary>
    /// Throws a custom exception with the text from the given exception.
    /// </summary>
    /// <param name="exception">The exception to extract text from.</param>
    /// <param name="isReallyThrowing">Whether to actually throw the exception.</param>
    /// <returns>True if the exception condition was met.</returns>
    internal static bool Custom(Exception exception, bool isReallyThrowing = true)
    { return Custom(Exceptions.TextOfExceptions(exception), isReallyThrowing); }

    /// <summary>
    /// Throws a custom exception with the specified message.
    /// </summary>
    /// <param name="message">The primary error message.</param>
    /// <param name="isReallyThrowing">Whether to actually throw the exception.</param>
    /// <param name="secondMessage">An optional additional message to append.</param>
    /// <returns>True if the exception condition was met.</returns>
    internal static bool Custom(string message, bool isReallyThrowing = true, string secondMessage = "")
    {
        string joined = string.Join(" ", message, secondMessage);
        string? exceptionText = Exceptions.Custom(FullNameOfExecutedCode(), joined);
        return ThrowIsNotNull(exceptionText, isReallyThrowing);
    }

    /// <summary>
    /// Throws a custom exception with a stack trace from the given exception.
    /// </summary>
    /// <param name="exception">The exception to report.</param>
    /// <returns>True if the exception condition was met.</returns>
    internal static bool CustomWithStackTrace(Exception exception) { return Custom(Exceptions.TextOfExceptions(exception)); }

    /// <summary>
    /// Throws if the specified directory does not exist.
    /// </summary>
    /// <param name="path">The directory path to verify.</param>
    /// <returns>True if the directory does not exist.</returns>
    internal static bool DirectoryExists(string path)
    { return ThrowIsNotNull(Exceptions.DirectoryExists(FullNameOfExecutedCode(), path)); }

    /// <summary>
    /// Throws if the parameter value appears to be URL-encoded.
    /// </summary>
    /// <param name="parameterValue">The parameter value to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>True if the parameter is invalid.</returns>
    internal static bool InvalidParameter(string parameterValue, string parameterName)
    { return ThrowIsNotNull(Exceptions.InvalidParameter(FullNameOfExecutedCode(), parameterValue, parameterName)); }

    /// <summary>
    /// Throws if the argument value is null, empty, or whitespace.
    /// </summary>
    /// <param name="argumentName">The name of the argument.</param>
    /// <param name="argumentValue">The value of the argument.</param>
    /// <returns>True if the argument is null or empty.</returns>
    internal static bool IsNullOrEmpty(string argumentName, string argumentValue)
    { return ThrowIsNotNull(Exceptions.IsNullOrWhitespace(FullNameOfExecutedCode(), argumentName, argumentValue, true)); }

    /// <summary>
    /// Throws if the case is not implemented for the specified value.
    /// </summary>
    /// <param name="notImplementedName">The case value that is not implemented.</param>
    /// <returns>True if the exception was thrown.</returns>
    internal static bool NotImplementedCase(object notImplementedName)
    { return ThrowIsNotNull(Exceptions.NotImplementedCase, notImplementedName); }

    /// <summary>
    /// Throws a "not implemented method" exception.
    /// </summary>
    /// <returns>True if the exception was thrown.</returns>
    internal static bool NotImplementedMethod() { return ThrowIsNotNull(Exceptions.NotImplementedMethod); }

    /// <summary>
    /// Gets the full name (type.method) of the code that called into ThrowEx.
    /// </summary>
    /// <returns>The caller's full name.</returns>
    internal static string FullNameOfExecutedCode()
    {
        Tuple<string, string, string> placeOfException = Exceptions.PlaceOfException();
        string fullName = FullNameOfExecutedCode(placeOfException.Item1, placeOfException.Item2, true);
        return fullName;
    }

    /// <summary>
    /// Builds the full name from a type and method name.
    /// </summary>
    /// <param name="typeOrSource">The type, MethodBase, or string representing the source.</param>
    /// <param name="methodName">The method name.</param>
    /// <param name="isFromThrowEx">Whether the call originates from ThrowEx (increases stack depth).</param>
    /// <returns>The formatted "type.method" string.</returns>
    private static string FullNameOfExecutedCode(object typeOrSource, string methodName, bool isFromThrowEx = false)
    {
        if (methodName == null)
        {
            int depth = 2;
            if (isFromThrowEx)
            {
                depth++;
            }

            methodName = Exceptions.CallingMethod(depth);
        }
        string typeFullName;
        if (typeOrSource is Type typeInstance)
        {
            typeFullName = typeInstance.FullName ?? "Type cannot be get via type is Type type2";
        }
        else if (typeOrSource is MethodBase method)
        {
            typeFullName = method.ReflectedType?.FullName ?? "Type cannot be get via type is MethodBase method";
            methodName = method.Name;
        }
        else if (typeOrSource is string)
        {
            typeFullName = typeOrSource.ToString() ?? "Type cannot be get via type is string";
        }
        else
        {
            Type typeFromInstance = typeOrSource.GetType();
            typeFullName = typeFromInstance.FullName ?? "Type cannot be get via type.GetType()";
        }
        return string.Concat(typeFullName, ".", methodName);
    }

    /// <summary>
    /// Throws an exception if the exception text is not null.
    /// </summary>
    /// <param name="exceptionText">The exception text. If not null, an exception is thrown.</param>
    /// <param name="isReallyThrowing">Whether to actually throw the exception.</param>
    /// <returns>True if the exception text was not null.</returns>
    internal static bool ThrowIsNotNull(string? exceptionText, bool isReallyThrowing = true)
    {
        if (exceptionText != null)
        {
            Debugger.Break();
            if (isReallyThrowing)
            {
                throw new Exception(exceptionText);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Evaluates a function with caller name and an argument, and throws if the result is not null.
    /// </summary>
    /// <typeparam name="TArg">The type of the function argument.</typeparam>
    /// <param name="func">The function that generates the exception message.</param>
    /// <param name="exceptionArg">The argument to pass to the function.</param>
    /// <returns>True if the exception was thrown.</returns>
    internal static bool ThrowIsNotNull<TArg>(Func<string, TArg, string?> func, TArg exceptionArg)
    {
        string? exceptionMessage = func(FullNameOfExecutedCode(), exceptionArg);
        return ThrowIsNotNull(exceptionMessage);
    }

    /// <summary>
    /// Evaluates a function with caller name and throws if the result is not null.
    /// </summary>
    /// <param name="func">The function that generates the exception message.</param>
    /// <returns>True if the exception was thrown.</returns>
    internal static bool ThrowIsNotNull(Func<string, string?> func)
    {
        string? exceptionMessage = func(FullNameOfExecutedCode());
        return ThrowIsNotNull(exceptionMessage);
    }
}
