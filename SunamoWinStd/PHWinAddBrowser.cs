namespace SunamoWinStd;

partial class PHWin
{
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
}