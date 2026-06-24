namespace SunamoWinStd._sunamo.SunamoExceptions;

internal partial class ThrowEx
{
    internal static bool Custom(Exception exception, bool isReallyThrowing = true)
        => Custom(Exceptions.TextOfExceptions(exception), isReallyThrowing);

    internal static bool Custom(string message, bool isReallyThrowing = true, string secondMessage = "")
    {
        string joined = string.Join(" ", message, secondMessage);
        string? exceptionText = Exceptions.Custom(FullNameOfExecutedCode(), joined);
        return ThrowIsNotNull(exceptionText, isReallyThrowing);
    }

    internal static bool CustomWithStackTrace(Exception exception) => Custom(Exceptions.TextOfExceptions(exception));

    internal static bool DirectoryExists(string path)
        => ThrowIsNotNull(Exceptions.DirectoryExists(FullNameOfExecutedCode(), path));

    internal static bool InvalidParameter(string parameterValue, string parameterName)
        => ThrowIsNotNull(Exceptions.InvalidParameter(FullNameOfExecutedCode(), parameterValue, parameterName));

    internal static bool IsNullOrEmpty(string argumentName, string argumentValue)
        => ThrowIsNotNull(Exceptions.IsNullOrWhitespace(FullNameOfExecutedCode(), argumentName, argumentValue, true));

    internal static bool NotImplementedCase(object notImplementedName)
        => ThrowIsNotNull(Exceptions.NotImplementedCase, notImplementedName);

    internal static bool NotImplementedMethod() => ThrowIsNotNull(Exceptions.NotImplementedMethod);

    internal static string FullNameOfExecutedCode()
    {
        var placeOfException = Exceptions.PlaceOfException();
        string fullName = FullNameOfExecutedCode(placeOfException.Item1, placeOfException.Item2, true);
        return fullName;
    }

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
            var typeFromInstance = typeOrSource.GetType();
            typeFullName = typeFromInstance.FullName ?? "Type cannot be get via type.GetType()";
        }
        return string.Concat(typeFullName, ".", methodName);
    }

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

    internal static bool ThrowIsNotNull<TArg>(Func<string, TArg, string?> func, TArg exceptionArg)
    {
        string? exceptionMessage = func(FullNameOfExecutedCode(), exceptionArg);
        return ThrowIsNotNull(exceptionMessage);
    }

    internal static bool ThrowIsNotNull(Func<string, string?> func)
    {
        string? exceptionMessage = func(FullNameOfExecutedCode());
        return ThrowIsNotNull(exceptionMessage);
    }
}
