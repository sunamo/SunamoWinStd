namespace SunamoWinStd._sunamo;

internal class PathFormatDetectorService(ILogger logger)
{
    internal bool IsWindowsPathFormat(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        var isBadFormat = false;
        if (path.Length < 3) return isBadFormat;
        if (!char.IsLetter(path[0])) isBadFormat = true;
        if (char.IsLetter(path[1])) isBadFormat = true;
        if (path.Length > 2)
            if (path[1] != '\\' && path[2] != '\\')
                isBadFormat = true;
        return !isBadFormat;
    }

    /// <summary>
    /// Returns true if Windows path, false if Unix path, null if undetermined.
    /// </summary>
    /// <param name="path">The path to detect format for.</param>
    /// <param name="isLoggingIfNotUnixOrWindowsPath">Whether to log an error if the path format cannot be determined.</param>
    /// <returns>True for Windows, false for Unix, null for undetermined.</returns>
    internal bool? DetectPathType(string path, bool isLoggingIfNotUnixOrWindowsPath = false)
    {
        if (IsWindowsPathFormat(path))
        {
            return true;
        }

        if (path.Contains('\\') && !path.Contains('/'))
        {
            return true;
        }
        else if (!path.Contains('\\') && path.StartsWith('/'))
        {
            return false;
        }
        else if (path.Contains('\\') && path.Contains('/'))
        {
            if (isLoggingIfNotUnixOrWindowsPath)
            {
                logger.LogError(path + " - Contains both \\ and /");
            }

            return null;
        }
        else
        {
            if (isLoggingIfNotUnixOrWindowsPath)
            {
                logger.LogError(path + " - Invalid path or does not contain separators");
            }

            return null;
        }
    }

}
