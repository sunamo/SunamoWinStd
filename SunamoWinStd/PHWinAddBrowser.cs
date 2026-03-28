namespace SunamoWinStd;

partial class PHWin
{
    /// <summary>
    /// Adds the default browser to the browser paths dictionary.
    /// </summary>
    public static void AddBrowser()
    {
        AddBrowser(defaultBrowser);
    }
    /// <summary>
    /// Adds the specified browser executable path to the browser paths dictionary.
    /// </summary>
    /// <param name="browser">The browser type to add.</param>
    /// <returns>The executable path of the browser, or empty string if not found.</returns>
    public static string AddBrowser(Browsers browser)
    {
        if (BrowserPaths.Count != browserCount)
        {
            if (BrowserPaths.ContainsKey(browser)) return BrowserPaths[browser];
            var browserPath = string.Empty;
            switch (browser)
            {
                case Browsers.Chrome:
                    browserPath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.Firefox:
                    browserPath = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
                    if (!File.Exists(browserPath)) browserPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.EdgeBeta:
                    browserPath = @"C:\Program Files (x86)\Microsoft\Edge Beta\Application\msedge.exe";
                    break;
                case Browsers.Opera:
                    browserPath = WindowsOSHelper.FileIn(@"C:\Program Files\Opera\", "opera.exe");
                    if (!File.Exists(browserPath))
                        browserPath = WindowsOSHelper.FileIn(UserFoldersWin.Local, @"Programs\Opera", "opera.exe");
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.Vivaldi:
                    browserPath = @"C:\Program Files\Vivaldi\Application\vivaldi.exe";
                    if (!File.Exists(browserPath)) browserPath = WindowsOSHelper.FileIn(UserFoldersWin.Local, "Vivaldi", "vivaldi.exe");
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.Slimjet:
                    browserPath = @"C:\Program Files\Slimjet\slimjet.exe";
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.WaterFox:
                    browserPath = @"C:\Program Files\Waterfox\waterfox.exe";
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.OperaGX:
                    browserPath = WindowsOSHelper.FileIn(UserFoldersWin.Local, @"Programs\Opera GX", "opera.exe");
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.ChromeCanary:
                    browserPath = WindowsOSHelper.FileIn(UserFoldersWin.Local, @"Google\Chrome SxS", "chrome.exe");
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.Tor:
                    browserPath = @"D:\Desktop\Tor Browser\Browser\firefox.exe";
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.Bravebrowser:
                    browserPath = @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe";
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.ChromeBeta:
                    browserPath = @"C:\Program Files\Google\Chrome Beta\Application\chrome.exe";
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.EdgeDev:
                    browserPath = @"C:\Program Files (x86)\Microsoft\Edge Dev\Application\msedge.exe";
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.EdgeCanary:
                    browserPath = WindowsOSHelper.FileIn(UserFoldersWin.Local, @"microsoft\edge sxs\application", "msedge.exe");
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.ChromeDev:
                    browserPath = @"C:\Program Files\Google\Chrome Dev\Application\chrome.exe";
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.Min:
                    browserPath = @"C:\Users\r\AppData\Local\min\min.exe";
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.Basilisk:
                    browserPath = @"C:\Program Files\Basilisk\basilisk.exe";
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.NawerWhale:
                    browserPath = @"C:\Program Files\Naver\Naver Whale\Application\whale.exe";
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.KMeleon:
                    browserPath = @"D:\paSync\_browsers\KM-Goanna\k-meleon.exe";
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.PaleMoon:
                    browserPath = @"C:\Program Files\Pale Moon\palemoon.exe";
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.LibreWolf:
                    browserPath = @"C:\Program Files\LibreWolf\librewolf.exe";
                    NullIfNotExists(ref browserPath);
                    break;
                case Browsers.EdgeStable:
                    browserPath = @"C:\Windows\SystemApps\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\MicrosoftEdge.exe";
                    if (!File.Exists(browserPath))
                        browserPath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
                    NullIfNotExists(ref browserPath);
                    break;
                default:
                    ThrowEx.NotImplementedCase(browser);
                    break;
            }
            if (browserPath == null) browserPath = string.Empty;
            BrowserPaths.Add(browser, browserPath);
            return browserPath;
        }
        return BrowserPaths[browser];
    }
}
