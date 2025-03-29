namespace SunamoWinStd._sunamo;
public class PathFormatDetectorService(ILogger logger)
{
    public bool IsWindowsPathFormat(string argValue)
    {
        if (string.IsNullOrWhiteSpace(argValue)) return false;
        var badFormat = false;
        if (argValue.Length < 3) return badFormat;
        if (!char.IsLetter(argValue[0])) badFormat = true;
        if (char.IsLetter(argValue[1])) badFormat = true;
        if (argValue.Length > 2)
            if (argValue[1] != '\\' && argValue[2] != '\\')
                badFormat = true;
        return !badFormat;
    }

    public string? DetectPathType(string path)
    {
        if (IsWindowsPathFormat(path))
        {
            return "Windows";
        }

        if (path.Contains('\\') && !path.Contains('/'))
        {
            return "Windows";
        }
        else if (!path.Contains('\\') && path.StartsWith('/'))
        {
            return "Unix";
        }
        else if (path.Contains('\\') && path.Contains('/'))
        {
            // Obsahuje oba oddělovače - může být složitější případ (např. UNC cesty ve Windows,
            // nebo záměrná kombinace). Zde můžeme upřednostnit Windows, nebo vrátit neurčito.
            // Záleží na konkrétním případu použití.
            return "Mixed";
        }
        else
        {
            logger.LogError(path + " - Invalid path or does not contain separators");
            return null;
        }
    }

}
