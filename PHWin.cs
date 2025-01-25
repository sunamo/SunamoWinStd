namespace SunamoWinStd;

using Microsoft.Extensions.Logging;

public partial class PHWin
{
    private const string CodiumExe = "VSCodium.exe";
    private const string CodeExe = "Code.exe";
    private const string WebStorm64Exe = "WebStorm64.exe";
    private const string CodeInsiderExe = "Code - Insiders.exe";
    public static Type type = typeof(PHWin);


    private static readonly Browsers defBr = Browsers.EdgeStable;
    public static int opened;

    /// <summary>
    ///     Not contains Other
    /// </summary>
    public static Dictionary<Browsers, string> path = new();

    public static Dictionary<string, string> pathExe = new();

    public static int waitIfTen = 0;

    private static readonly int countOfBrowsers;

    public static Editor preferredEditor = Editor.Code;

    static PHWin()
    {
        var brs = Enum.GetValues<Browsers>().ToList();
        countOfBrowsers = brs.Count;
        // None is deleting automatically
        //countOfBrowsers--;
    }



    private static
#if ASYNC
        async Task
#else
    void
#endif
        BreakIfTenIde()
    {
        // Tady to není potřeba, otevřít textový soubor není tak náročné jako web stránku a
        //BreakIfTen();
        // ale často mám chybu "Another instance of Code is running but not responding".
        // Dobré je nezavírat taby v ide, pak to jde všechno rychleji

        // bylo tu Thread.Sleep, ale v asyncu to havarovalo bez důvodu. ThreadHelper.Sleep to samé, ačkoliv používá Task.Delay
        //#if ASYNC
        //        await
        //#endif
        //ThreadHelper.Sleep(1500);
        //await Task.Delay(1500);
    }




    public static void OpenFolderInTotalcmd(ILogger logger, string folder, bool throwExWhenError = false)
    {
        PH.RunFromPath(logger, "totalcmd.exe", "/O /T " + folder, false, throwExWhenError);
    }

    public static void WebStorm64(ILogger logger, string defFile, bool throwExWhenError = false)
    {
        PH.RunFromPath(logger, WebStorm64Exe, defFile, false, throwExWhenError);
    }



    public static void AddBrowsers()
    {
        if (path.Count == 0)
        {
            var all = Enum.GetValues<Browsers>();
            foreach (var item in all)
                if (item != Browsers.None)
                    AddBrowser(item);
        }
    }

    /// <summary>
    ///     forceAttemptHttps by dávalo větší smysl true
    ///     ale protože jsem si nepoznačil proč mi to radši dá do uvozovek, budu se držet původní definice metody
    /// </summary>
    /// <param name="prohlizec"></param>
    /// <param name="s"></param>
    /// <param name="waitMs"></param>
    /// <param name="forceAttemptHttps"></param>
    public static void OpenInBrowser(ILogger logger, Browsers prohlizec, string s, int waitMs = 0,
        bool forceAttemptHttps = false, bool throwExIsNotValidUrl = false)
    {
        if (forceAttemptHttps) s = UH.AppendHttpsIfNotExists(s);

        var b = path[prohlizec];
        BreakIfTen();
        s = PH.NormalizeUri(s);

        if (!s.StartsWith("http")) s = "https://" + s;

        if (!Uri.TryCreate(s, new UriCreationOptions(), out var _))
        {
            logger.LogError($"Can't create uri from " + s);
            if (throwExIsNotValidUrl)
            {
                ThrowEx.Custom($"Can't create uri from " + s);
            }

            return;
        }


        //if (prohlizec == Browsers.Chrome)
        //{
        s = "/new-tab " + s;
        //}


        Process.Start(new ProcessStartInfo(b, s));
        Thread.Sleep(100);

        if (waitMs > 0) Thread.Sleep(waitMs);
    }

    internal static void BreakIfTen()
    {
        opened++;
        if (opened % 10 == 0)
            //System.Diagnostics.Debugger.Break();
            if (waitIfTen != 0)
                Thread.Sleep(waitIfTen);
    }

    /// <summary>
    ///     12-1-24 nevím proč bych neměl mít default values takže je vracím
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="throwExIsNotValidUrl"></param>
    /// <param name="waitMs"></param>
    public static void OpenInBrowser(ILogger logger, string uri, bool throwExIsNotValidUrl = false,
        int waitMs = 0)
    {
        OpenInBrowser(logger, defBr, uri, waitMs);
    }




    private static void NullIfNotExists(ref string b)
    {
        if (!File.Exists(b)) b = null;
    }



    [Obsolete("Toto již není třeba, vše musí být jen v jednom nugetu")]
    public static void AssignSearchInAll()
    {
        //AddBrowsers();
        //UriWebServices.AssignSearchInAll(PHWin.SearchInAll);
    }

    private static string AddPathIfNotContains(UserFoldersWin local, string v, string codeExe)
    {
        if (!pathExe.ContainsKey(codeExe))
        {
            var fi = WindowsOSHelper.FileIn(local, v, codeExe);
            fi = Path.GetDirectoryName(fi);
            pathExe.Add(codeExe, fi);

            return fi;
        }

        return pathExe[codeExe];
    }

    public static void PreferredEditor(ILogger logger, string f, bool throwExWhenError = false)
    {
        switch (preferredEditor)
        {
            case Editor.Code:
                Code(logger, f, throwExWhenError);
                break;
            case Editor.Codium:
                Codium(logger, f, throwExWhenError);
                break;
            case Editor.CodeInsider:
                CodeInsider(logger, f, throwExWhenError);
                break;
            default:
                ThrowEx.NotImplementedCase(preferredEditor);
                break;
        }
    }

    public static void OpenInBrowserAutomaticallyCountOfOpened(ILogger logger, Browsers prohlizec, string s, int waitMs = 0)
    {
        OpenInBrowser(logger, prohlizec, s, waitMs);
    }

    /// <returns></returns>
    public static List<string> BrowsersWhichDontHaveExeInDefinedPath()
    {
        var doesntExists = new List<string>();

        AddBrowsers();
        foreach (var item in path)
            if (!File.Exists(item.Value))
                doesntExists.Add(item.Value);

        return doesntExists;
    }

    public static void ExecutableOfAllBrowsers()
    {
        PHWin.AddBrowsers();
        Console.WriteLine("Pokud je empty, exe neexistuje na disku! A vice versa!");
        foreach (var item in PHWin.path)
        {
            Console.WriteLine(item.Key + ": " + SH.NullToStringOrDefault(item.Value));
        }
    }

    public static void OpenInAllBrowsers(ILogger logger, string uri)
    {
        OpenInAllBrowsers(logger, new List<string>([uri]));
    }

    public static void OpenInAllBrowsers(ILogger logger, IList<string> uris)
    {
        AddBrowsers();
        foreach (var uri in uris)
            foreach (var item in path)
            {
                if (item.Key == Browsers.Tor) continue;
                OpenInBrowser(logger, item.Key, uri, 50);
            }
    }

    public static void OpenFolder(string folder)
    {
        Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", folder);
    }


    public static void SaveAndOpenInBrowser(ILogger logger, Browsers prohlizec, string htmlKod)
    {
        var s = Path.GetTempFileName() + ".html";
        File.WriteAllText(s, htmlKod);
        OpenInBrowser(logger, prohlizec, s, 50);
    }

    public static bool IsUsed(string fullPath)
    {
        return FileUtil.WhoIsLocking(fullPath).Count > 0;
    }

    #region Interop

    #endregion
}