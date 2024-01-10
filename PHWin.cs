namespace SunamoWinStd;

public partial class PHWin
{


    #region Interop


















    #endregion

    private static string AddPathIfNotContains(UserFoldersWin local, string v, string codeExe)
    {
        if (!pathExe.ContainsKey(codeExe))
        {
            var fi = WindowsOSHelper.FileIn(local, v, codeExe);
            fi = FS.GetDirectoryName(fi);
            pathExe.Add(codeExe, fi);

            return fi;
        }
        return pathExe[codeExe];
    }

    public static Editor preferredEditor = Editor.Code;

    public static void PreferredEditor(string f)
    {
        switch (preferredEditor)
        {
            case Editor.Code:
                PHWin.Code(f);
                break;
            case Editor.Codium:
                PHWin.Codium(f);
                break;
            case Editor.CodeInsider:
                PHWin.CodeInsider(f);
                break;
            default:
                ThrowEx.NotImplementedCase(preferredEditor);
                break;
        }
    }

    public static void OpenInBrowserAutomaticallyCountOfOpened(Browsers prohlizec, string s, int waitMs = 0)
    {
        OpenInBrowser(prohlizec, s, waitMs);
    }

    /// <returns></returns>
    public static List<string> BrowsersWhichDontHaveExeInDefinedPath()
    {
        List<string> doesntExists = new List<string>();

        AddBrowsers();
        foreach (var item in path)
        {
            if (!FS.ExistsFile(item.Value))
            {
                doesntExists.Add(item.Value);
            }
        }

        return doesntExists;
    }





    public static void OpenInAllBrowsers(string uri)
    {
        OpenInAllBrowsers(CA.ToListString(uri));
    }

    public static void OpenInAllBrowsers(IList<string> uris)
    {
        AddBrowsers();
        foreach (var uri in uris)
        {
            foreach (var item in path)
            {
                if (item.Key == Browsers.Tor)
                {
                    continue;
                }
                OpenInBrowser(item.Key, uri, 50);
            }
        }
    }

    public static void OpenFolder(string folder)
    {
        Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", folder);
    }



    public static void SaveAndOpenInBrowser(Browsers prohlizec, string htmlKod)
    {
        string s = Path.GetTempFileName() + ".html";
        File.WriteAllText(s, htmlKod);
        OpenInBrowser(prohlizec, s, 50);
    }

    public static bool IsUsed(string fullPath)
    {
        return FileUtil.WhoIsLocking(fullPath).Count > 0;
    }











}
