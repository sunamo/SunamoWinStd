namespace SunamoWinStd;

public class PHWin
{
    public static Type type = typeof(PHWin);

    const string CodiumExe = "VSCodium.exe";
    const string CodeExe = "Code.exe";
    const string WebStorm64Exe = "WebStorm64.exe";
    const string CodeInsiderExe = "Code - Insiders.exe";

    public static async Task Codium(string defFile)
    {
        await BreakIfTenIde();

        if (string.IsNullOrWhiteSpace(defFile))
        {
            ThrowEx.InvalidParameter(defFile, "defFile");
        }

        PH.RunFromPath(CodiumExe, defFile, false);
    }

    static
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
        await Task.Delay(1500);
    }

    public static async Task Code(string defFile)
    {
        await BreakIfTenIde();

        if (string.IsNullOrWhiteSpace(defFile))
        {
            ThrowEx.InvalidParameter(defFile, "defFile");
        }
        //ThrowEx.FileDoesntExists(defFile);

        //var v = AddPathIfNotContains( UserFoldersWin.Local, @"Programs\Microsoft VS Code", CodeExe);

        PH.RunFromPath(CodeExe, defFile, false);
    }

    public static async Task WebStorm64(string defFile)
    {
        PH.RunFromPath(WebStorm64Exe, defFile, false);
    }

    public static async Task CodeInsider(string defFile)
    {
        await BreakIfTenIde();
        PH.RunFromPath(CodeInsiderExe, defFile, false);
    }


    static Browsers defBr = Browsers.EdgeStable;
    public static int opened = 0;
    /// <summary>
    /// Not contains Other
    /// </summary>
    public static Dictionary<Browsers, string> path = new Dictionary<Browsers, string>();
    public static Dictionary<string, string> pathExe = new Dictionary<string, string>();

    public static void AddBrowsers()
    {
        if (path.Count == 0)
        {
            var all = Enum.GetValues<Browsers>();
            foreach (var item in all)
            {
                if (item != Browsers.None)
                {
                    AddBrowser(item);
                }
            }
        }
    }

    public static int waitIfTen = 0;

    /// <summary>
    /// forceAttemptHttps by dávalo větší smysl true
    /// ale protože jsem si nepoznačil proč mi to radši dá do uvozovek, budu se držet původní definice metody
    /// 
    /// </summary>
    /// <param name="prohlizec"></param>
    /// <param name="s"></param>
    /// <param name="waitMs"></param>
    /// <param name="forceAttemptHttps"></param>
    public static void OpenInBrowser(Browsers prohlizec, string s, int waitMs = Consts.waitMsOpenInBrowser, bool forceAttemptHttps = false, bool throwExIsNotValidUrl = false)
    {
        if (forceAttemptHttps)
        {
            s = UH.AppendHttpsIfNotExists(s);
        }

        string b = path[prohlizec];
        BreakIfTen();
        s = PH.NormalizeUri(s);

        if (!s.StartsWith("http"))
        {
            s = "https://" + s;
        }

        if (!Uri.TryCreate(s, new UriCreationOptions { }, out Uri _))
        {
            if (throwExIsNotValidUrl)
            {
                //ThrowEx.UriFormat(s, UH.IsUri);
            }
            return;
        }



        //if (prohlizec == Browsers.Chrome)
        //{
        s = "/new-tab " + s;
        //}



        Process.Start(new ProcessStartInfo(b, s));
        Thread.Sleep(100);

        if (waitMs > 0)
        {
            Thread.Sleep(waitMs);

        }
    }

    private static void BreakIfTen()
    {
        opened++;
        if (opened % 10 == 0)
        {
            //System.Diagnostics.Debugger.Break();
            if (waitIfTen != 0)
            {
                Thread.Sleep((int)waitIfTen);
            }
        }
    }

    /// <summary>
    /// 12-1-24 nevím proč bych neměl mít default values takže je vracím
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="throwExIsNotValidUrl"></param>
    /// <param name="waitMs"></param>
    public static void OpenInBrowser(string uri, bool throwExIsNotValidUrl = false, int waitMs = Consts.waitMsOpenInBrowser)
    {
        OpenInBrowser(defBr, uri, waitMs);
    }

    public static void AddBrowser()
    {
        AddBrowser(defBr);
    }

    static int countOfBrowsers = 0;
    static PHWin()
    {
        var brs = Enum.GetValues<Browsers>().ToList();
        countOfBrowsers = brs.Count;
        // None is deleting automatically
        //countOfBrowsers--;
    }

    public static string AddBrowser(Browsers prohlizec)
    {
        if (path.Count != countOfBrowsers)
        {
            if (path.ContainsKey(prohlizec))
            {
                return path[prohlizec];
            }

            string b = string.Empty;

            switch (prohlizec)
            {
                case Browsers.Chrome: //1
                    b = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                    NullIfNotExists(ref b);
                    break;
                case Browsers.Firefox: //2
                    b = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
                    if (!File.Exists(b))
                    {
                        b = @"C:\Program Files\Mozilla Firefox\firefox.exe";
                    }
                    NullIfNotExists(ref b);
                    break;
                case Browsers.EdgeBeta: //3

                    //C:\Users\Administrator\AppData\Local\Microsoft\WindowsApps\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\MicrosoftEdge.exe
                    b = @"C:\Program Files (x86)\Microsoft\Edge Beta\Application\msedge.exe";//WindowsOSHelper.FileIn(UserFoldersWin.Local, @"microsoft\edge beta\application", "msedge.exe");


                    break;
                case Browsers.Opera://4
                    // Opera has version also when is installing to PF, it cant be changed
                    //b = @"C:\Program Files\Opera\65.0.3467.78\opera.exe";
                    b = WindowsOSHelper.FileIn(@"C:\Program Files\Opera\", "opera.exe");
                    if (!File.Exists(b))
                    {
                        b = WindowsOSHelper.FileIn(UserFoldersWin.Local, @"Programs\Opera", "opera.exe");
                    }
                    NullIfNotExists(ref b);
                    break;
                case Browsers.Vivaldi://5
                    b = @"C:\Program Files\Vivaldi\Application\vivaldi.exe";
                    if (!File.Exists(b))
                    {
                        b = WindowsOSHelper.FileIn(UserFoldersWin.Local, "Vivaldi", "vivaldi.exe");
                    }
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
                case Browsers.Iridium: //6
                    b = @"C:\Program Files\Iridium\iridium.exe";
                    NullIfNotExists(ref b);
                    break;
                case Browsers.Falkon: //7
                    b = @"C:\Program Files\Falkon\falkon.exe";
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
                case Browsers.ChromeDev:
                    b = @"C:\Program Files\Google\Chrome Dev\Application\chrome.exe";
                    NullIfNotExists(ref b);
                    break;

                case Browsers.EdgeStable://254
                    // tady se to skutečně jmenuje MicrosoftEdge.exe
                    b = @"C:\Windows\SystemApps\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\MicrosoftEdge.exe";

                    if (!File.Exists(b))
                    {
                        //C:\Users\Administrator\AppData\Local\Microsoft\WindowsApps\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\MicrosoftEdge.exe
                        b = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";

                    }
                    NullIfNotExists(ref b);
                    break;
                default:
                    ThrowEx.NotImplementedCase(prohlizec);
                    break;
            }

            if (b == null)
            {
                b = string.Empty;
            }

            path.Add(prohlizec, b);

            return b;
        }
        return path[prohlizec];
    }



    private static void NullIfNotExists(ref string b)
    {
        if (!File.Exists(b))
        {
            b = null;
        }
    }

    /// <summary>
    /// A1 is chrome replacement
    /// </summary>
    /// <param name="array"></param>
    /// <param name="what"></param>
    public static void SearchInAll(IList array, string what)
    {

        ThrowEx.NotImplementedMethod();
        ////var br = Browsers.Chrome;
        //PHWin.AddBrowser();
        //foreach (var item in array)
        //{
        //    opened++;
        //    string uri = UriWebServices.FromChromeReplacement(item.ToString(), what);
        //    PHWin.OpenInBrowser(uri, true, 50);
        //    if (opened % 10 == 0)
        //    {
        //        System.Diagnostics.Debugger.Break();
        //    }
        //}
    }

    public static void AssignSearchInAll()
    {
        //AddBrowsers();
        //UriWebServices.AssignSearchInAll(PHWin.SearchInAll);
    }

    #region Interop


















    #endregion

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

    public static Editor preferredEditor = Editor.Code;

    public static async Task PreferredEditor(string f)
    {
        switch (preferredEditor)
        {
            case Editor.Code:
                await PHWin.Code(f);
                break;
            case Editor.Codium:
                await PHWin.Codium(f);
                break;
            case Editor.CodeInsider:
                await PHWin.CodeInsider(f);
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
            if (!File.Exists(item.Value))
            {
                doesntExists.Add(item.Value);
            }
        }

        return doesntExists;
    }





    public static void OpenInAllBrowsers(string uri)
    {
        OpenInAllBrowsers(new List<string>([uri]));
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
