namespace SunamoWinStd;

/// <summary>
/// Helper class for Windows OS specific operations like finding files in user app data folders.
/// </summary>
public class WindowsOSHelper
{
    /// <summary>
    /// Finds a file in a user app data subfolder.
    /// </summary>
    /// <param name="userFoldersWin">The user folder type (Local, Roaming, LocalLow).</param>
    /// <param name="appName">Application subfolder name.</param>
    /// <param name="executableName">Executable file name to find.</param>
    /// <returns>Full path of the found file, or null.</returns>
    public static string? FileIn(UserFoldersWin userFoldersWin, string appName, string executableName)
    {
        var folder = Path.Combine(PathOfAppDataFolder(userFoldersWin), appName);
        return FileIn(folder, executableName);
    }
    /// <summary>
    /// Finds a file recursively in the specified folder.
    /// </summary>
    /// <param name="folder">Folder to search in.</param>
    /// <param name="executableName">Executable file name to find.</param>
    /// <returns>Full path of the found file, or null.</returns>
    public static string? FileIn(string folder, string executableName)
    {
        if (Directory.Exists(folder))
        {
            var searchPattern = executableName;
            return Directory.GetFiles(folder, searchPattern, SearchOption.AllDirectories).FirstOrDefault();
        }
        return null;
    }
    /// <summary>
    ///     Gets the path of the specified AppData subfolder.
    /// </summary>
    /// <param name="userFoldersWin">The user folder type.</param>
    /// <returns>Full path to the AppData subfolder.</returns>
    public static string PathOfAppDataFolder(UserFoldersWin userFoldersWin)
    {
        var result = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", userFoldersWin.ToString());
        return result;
    }
}
