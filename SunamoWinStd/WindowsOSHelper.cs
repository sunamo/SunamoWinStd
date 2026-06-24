namespace SunamoWinStd;

public class WindowsOSHelper
{
    public static string? FileIn(UserFoldersWin userFoldersWin, string appName, string executableName)
    {
        var folder = Path.Combine(PathOfAppDataFolder(userFoldersWin), appName);
        return FileIn(folder, executableName);
    }

    public static string? FileIn(string folder, string executableName)
    {
        if (Directory.Exists(folder))
        {
            return Directory.GetFiles(folder, executableName, SearchOption.AllDirectories).FirstOrDefault();
        }
        return null;
    }

    public static string PathOfAppDataFolder(UserFoldersWin userFoldersWin)
    {
        var result = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", userFoldersWin.ToString());
        return result;
    }
}
