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

    public static void AddBrowser()
    {
        AddBrowser(defBr);
    }

    public static string AddBrowser(Browsers prohlizec)
    {
        if (path.Count != countOfBrowsers)
        {
            if (path.ContainsKey(prohlizec)) return path[prohlizec];

            var b = string.Empty;

            switch (prohlizec)
            {
                case Browsers.Chrome: //1
                    b = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                    NullIfNotExists(ref b);
                    break;
                case Browsers.Firefox: //2
                    b = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
                    if (!File.Exists(b)) b = @"C:\Program Files\Mozilla Firefox\firefox.exe";
                    NullIfNotExists(ref b);
                    break;
                case Browsers.EdgeBeta: //3

                    //C:\Users\Administrator\AppData\Local\Microsoft\WindowsApps\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\MicrosoftEdge.exe
                    b = @"C:\Program Files (x86)\Microsoft\Edge Beta\Application\msedge.exe"; //WindowsOSHelper.FileIn(UserFoldersWin.Local, @"microsoft\edge beta\application", "msedge.exe");


                    break;
                case Browsers.Opera: //4
                    // Opera has version also when is installing to PF, it cant be changed
                    //b = @"C:\Program Files\Opera\65.0.3467.78\opera.exe";
                    b = WindowsOSHelper.FileIn(@"C:\Program Files\Opera\", "opera.exe");
                    if (!File.Exists(b))
                        b = WindowsOSHelper.FileIn(UserFoldersWin.Local, @"Programs\Opera", "opera.exe");
                    NullIfNotExists(ref b);
                    break;
                case Browsers.Vivaldi: //5
                    b = @"C:\Program Files\Vivaldi\Application\vivaldi.exe";
                    if (!File.Exists(b)) b = WindowsOSHelper.FileIn(UserFoldersWin.Local, "Vivaldi", "vivaldi.exe");
                    NullIfNotExists(ref b);
                    break;
                //case Browsers.InternetExplorer:
                //    b = @"C:\Program Files (x86)\Internet Explorer\iexplore.exe";
                //    break;
                //case Browsers.Maxthon:
                //    b = @"C:\Program Files (x86)\Maxthon5\Bin\Maxthon.exe";
                //    if (!File.Exists(b))
                //    {
                //        b = WindowsOSHelper.FileIn(UserFoldersWin.Local, @"Maxthon\Application", "Maxthon.exe");
                //    }
                //    NullIfNotExists(ref b);
                //    break;
                case Browsers.Slimjet: //6
                    //b = @"C:\Program Files\Iridium\iridium.exe";
                    b = @"C:\Program Files\Slimjet\slimjet.exe";
                    NullIfNotExists(ref b);
                    break;
                case Browsers.WaterFox: //7
                    //b = @"C:\Program Files\Falkon\falkon.exe";
                    b = @"C:\Program Files\Waterfox\waterfox.exe";
                    NullIfNotExists(ref b);
                    break;
                case Browsers.OperaGX: //8
                    b = WindowsOSHelper.FileIn(UserFoldersWin.Local, @"Programs\Opera GX", "opera.exe");
                    NullIfNotExists(ref b);
                    break;
                //case Browsers.Seznam: //7
                //    b = WindowsOSHelper.FileIn(UserFoldersWin.Roaming, @"Seznam Browser", "Seznam.cz.exe");
                //    NullIfNotExists(ref b);
                //    break;
                //case Browsers.Chromium: //8
                //    b = @"D:\paSync\_browsers\Chromium\chrome.exe";
                //    NullIfNotExists(ref b);
                //    break;
                case Browsers.ChromeCanary: //9
                    b = WindowsOSHelper.FileIn(UserFoldersWin.Local, @"Google\Chrome SxS", "chrome.exe");
                    NullIfNotExists(ref b);
                    break;
                case Browsers.Tor: //10
                    b = @"D:\Desktop\Tor Browser\Browser\firefox.exe";
                    NullIfNotExists(ref b);
                    break;
                case Browsers.Bravebrowser: //11
                    b = @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe";
                    NullIfNotExists(ref b);
                    break;
                //case Browsers.PaleMoon:
                //    b = @"C:\Program Files\Pale Moon\palemoon.exe";
                //    NullIfNotExists(ref b);
                //    break;
                case Browsers.ChromeBeta: //12
                    b = @"C:\Program Files\Google\Chrome Beta\Application\chrome.exe";
                    NullIfNotExists(ref b);
                    break;
                case Browsers.EdgeDev: //13

                    //C:\Users\Administrator\AppData\Local\Microsoft\WindowsApps\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\MicrosoftEdge.exe
                    b = @"C:\Program Files (x86)\Microsoft\Edge Dev\Application\msedge.exe";
                    NullIfNotExists(ref b);
                    break;

                case Browsers.EdgeCanary: //14
                    b = WindowsOSHelper.FileIn(UserFoldersWin.Local, @"microsoft\edge sxs\application", "msedge.exe");
                    NullIfNotExists(ref b);

                    break;
                //15
                case Browsers.ChromeDev:
                    b = @"C:\Program Files\Google\Chrome Dev\Application\chrome.exe";
                    NullIfNotExists(ref b);
                    break;

                case Browsers.Min:
                    b = @"C:\Users\r\AppData\Local\min\min.exe";
                    NullIfNotExists(ref b);
                    break;



                case Browsers.Basilisk:
                    b = @"C:\Program Files\Basilisk\basilisk.exe";
                    NullIfNotExists(ref b);
                    break;
                case Browsers.NawerWhale:
                    b = @"C:\Program Files\Naver\Naver Whale\Application\whale.exe";
                    NullIfNotExists(ref b);
                    break;
                case Browsers.KMeleon:
                    b = @"D:\paSync\_browsers\KM-Goanna\k-meleon.exe";
                    NullIfNotExists(ref b);
                    break;
                case Browsers.PaleMoon:
                    b = @"C:\Program Files\Pale Moon\palemoon.exe";
                    NullIfNotExists(ref b);
                    break;

                case Browsers.LibreWolf:
                    b = @"C:\Program Files\LibreWolf\librewolf.exe";
                    NullIfNotExists(ref b);
                    break;


                case Browsers.EdgeStable: //254
                    // tady se to skutečně jmenuje MicrosoftEdge.exe
                    b = @"C:\Windows\SystemApps\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\MicrosoftEdge.exe";

                    if (!File.Exists(b))
                        //C:\Users\Administrator\AppData\Local\Microsoft\WindowsApps\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\MicrosoftEdge.exe
                        b = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
                    NullIfNotExists(ref b);
                    break;

                default:
                    ThrowEx.NotImplementedCase(prohlizec);
                    break;
            }

            if (b == null) b = string.Empty;

            path.Add(prohlizec, b);

            return b;
        }

        return path[prohlizec];
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