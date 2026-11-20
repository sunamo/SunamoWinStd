namespace SunamoWinStd;

/// <summary>
/// Represents the windows OS helper type.
/// </summary>
public class WindowsOSHelper
{
    /// <summary>
    /// File in.
    /// </summary>
    /// <param name="userFoldersWin">The user folders win.</param>
    /// <param name="appName">The app name.</param>
    /// <param name="executableName">The executable name.</param>
    /// <returns>The file in result.</returns>
    public static string? FileIn(UserFoldersWin userFoldersWin, string appName, string executableName)
    {
        var folder = Path.Combine(PathOfAppDataFolder(userFoldersWin), appName);
        return FileIn(folder, executableName);
    }

    /// <summary>
    /// File in.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <param name="executableName">The executable name.</param>
    /// <returns>The file in result.</returns>
    public static string? FileIn(string folder, string executableName)
    {
        if (Directory.Exists(folder))
        {
            return Directory.GetFiles(folder, executableName, SearchOption.AllDirectories).FirstOrDefault();
        }
        return null;
    }

    /// <summary>
    /// Path of app data folder.
    /// </summary>
    /// <param name="userFoldersWin">The user folders win.</param>
    /// <returns>The path of app data folder result.</returns>
    public static string PathOfAppDataFolder(UserFoldersWin userFoldersWin)
    {
        var result = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", userFoldersWin.ToString());
        return result;
    }
}
