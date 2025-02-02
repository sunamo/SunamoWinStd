namespace SunamoWinStd;
using System.Security.Principal;

public class WindowsOSHelper
{
    private static Type type = typeof(WindowsOSHelper);

    public static string? FileIn(UserFoldersWin local, string appName, string exe)
    {
        var folder = Path.Combine(PathOfAppDataFolder(local), appName);
        return FileIn(folder, exe);
    }

    public static string? FileIn(string folder, string exe)
    {
        if (Directory.Exists(folder))
        {
            var masc = string.Empty; //FS.MascFromExtension(exe);
            masc = exe;

            return Directory.GetFiles(folder, masc, SearchOption.AllDirectories).FirstOrDefault();
        }

        return null;
    }

    /// <summary>
    ///     All
    /// </summary>
    /// <param name="af"></param>
    public static string PathOfAppDataFolder(UserFoldersWin af)
    {
        var result = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", af.ToString());
        return result;
    }
}