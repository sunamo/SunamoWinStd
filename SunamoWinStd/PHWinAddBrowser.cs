// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy

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
            var builder = string.Empty;
            switch (prohlizec)
            {
                case Browsers.Chrome: //1
                    builder = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                    NullIfNotExists(ref builder);
                    break;
                case Browsers.Firefox: //2
                    builder = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
                    if (!File.Exists(builder)) builder = @"C:\Program Files\Mozilla Firefox\firefox.exe";
                    NullIfNotExists(ref builder);
                    break;
                case Browsers.EdgeBeta: //3
                    //C:\Users\Administrator\AppData\Local\Microsoft\WindowsApps\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\MicrosoftEdge.exe
                    builder = @"C:\Program Files (x86)\Microsoft\Edge Beta\Application\msedge.exe"; //WindowsOSHelper.FileIn(UserFoldersWin.Local, @"microsoft\edge beta\application", "msedge.exe");
                    break;
                case Browsers.Opera: //4
                    // Opera has version also when is installing to PF, it cant be changed
                    //b = @"C:\Program Files\Opera\65.0.3467.78\opera.exe";
                    builder = WindowsOSHelper.FileIn(@"C:\Program Files\Opera\", "opera.exe");
                    if (!File.Exists(builder))
                        builder = WindowsOSHelper.FileIn(UserFoldersWin.Local, @"Programs\Opera", "opera.exe");
                    NullIfNotExists(ref builder);
                    break;
                case Browsers.Vivaldi: //5
                    builder = @"C:\Program Files\Vivaldi\Application\vivaldi.exe";
                    if (!File.Exists(builder)) builder = WindowsOSHelper.FileIn(UserFoldersWin.Local, "Vivaldi", "vivaldi.exe");
                    NullIfNotExists(ref builder);
                    break;
                //case Browsers.InternetExplorer:
                //    builder = @"C:\Program Files (x86)\Internet Explorer\iexplore.exe";
                //    break;
                //case Browsers.Maxthon:
                //    builder = @"C:\Program Files (x86)\Maxthon5\Bin\Maxthon.exe";
                //    if (!File.Exists(builder))
                //    {
                //        builder = WindowsOSHelper.FileIn(UserFoldersWin.Local, @"Maxthon\Application", "Maxthon.exe");
                //    }
                //    NullIfNotExists(ref builder);
                //    break;
                case Browsers.Slimjet: //6
                    //b = @"C:\Program Files\Iridium\iridium.exe";
                    builder = @"C:\Program Files\Slimjet\slimjet.exe";
                    NullIfNotExists(ref builder);
                    break;
                case Browsers.WaterFox: //7
                    //b = @"C:\Program Files\Falkon\falkon.exe";
                    builder = @"C:\Program Files\Waterfox\waterfox.exe";
                    NullIfNotExists(ref builder);
                    break;
                case Browsers.OperaGX: //8
                    builder = WindowsOSHelper.FileIn(UserFoldersWin.Local, @"Programs\Opera GX", "opera.exe");
                    NullIfNotExists(ref builder);
                    break;
                //case Browsers.Seznam: //7
                //    builder = WindowsOSHelper.FileIn(UserFoldersWin.Roaming, @"Seznam Browser", "Seznam.cz.exe");
                //    NullIfNotExists(ref builder);
                //    break;
                //case Browsers.Chromium: //8
                //    builder = @"D:\paSync\_browsers\Chromium\chrome.exe";
                //    NullIfNotExists(ref builder);
                //    break;
                case Browsers.ChromeCanary: //9
                    builder = WindowsOSHelper.FileIn(UserFoldersWin.Local, @"Google\Chrome SxS", "chrome.exe");
                    NullIfNotExists(ref builder);
                    break;
                case Browsers.Tor: //10
                    builder = @"D:\Desktop\Tor Browser\Browser\firefox.exe";
                    NullIfNotExists(ref builder);
                    break;
                case Browsers.Bravebrowser: //11
                    builder = @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe";
                    NullIfNotExists(ref builder);
                    break;
                //case Browsers.PaleMoon:
                //    builder = @"C:\Program Files\Pale Moon\palemoon.exe";
                //    NullIfNotExists(ref builder);
                //    break;
                case Browsers.ChromeBeta: //12
                    builder = @"C:\Program Files\Google\Chrome Beta\Application\chrome.exe";
                    NullIfNotExists(ref builder);
                    break;
                case Browsers.EdgeDev: //13
                    //C:\Users\Administrator\AppData\Local\Microsoft\WindowsApps\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\MicrosoftEdge.exe
                    builder = @"C:\Program Files (x86)\Microsoft\Edge Dev\Application\msedge.exe";
                    NullIfNotExists(ref builder);
                    break;
                case Browsers.EdgeCanary: //14
                    builder = WindowsOSHelper.FileIn(UserFoldersWin.Local, @"microsoft\edge sxs\application", "msedge.exe");
                    NullIfNotExists(ref builder);
                    break;
                //15
                case Browsers.ChromeDev:
                    builder = @"C:\Program Files\Google\Chrome Dev\Application\chrome.exe";
                    NullIfNotExists(ref builder);
                    break;
                case Browsers.Min:
                    builder = @"C:\Users\r\AppData\Local\min\min.exe";
                    NullIfNotExists(ref builder);
                    break;
                case Browsers.Basilisk:
                    builder = @"C:\Program Files\Basilisk\basilisk.exe";
                    NullIfNotExists(ref builder);
                    break;
                case Browsers.NawerWhale:
                    builder = @"C:\Program Files\Naver\Naver Whale\Application\whale.exe";
                    NullIfNotExists(ref builder);
                    break;
                case Browsers.KMeleon:
                    builder = @"D:\paSync\_browsers\KM-Goanna\k-meleon.exe";
                    NullIfNotExists(ref builder);
                    break;
                case Browsers.PaleMoon:
                    builder = @"C:\Program Files\Pale Moon\palemoon.exe";
                    NullIfNotExists(ref builder);
                    break;
                case Browsers.LibreWolf:
                    builder = @"C:\Program Files\LibreWolf\librewolf.exe";
                    NullIfNotExists(ref builder);
                    break;
                case Browsers.EdgeStable: //254
                    // tady se to skutečně jmenuje MicrosoftEdge.exe
                    builder = @"C:\Windows\SystemApps\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\MicrosoftEdge.exe";
                    if (!File.Exists(builder))
                        //C:\Users\Administrator\AppData\Local\Microsoft\WindowsApps\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\MicrosoftEdge.exe
                        builder = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
                    NullIfNotExists(ref builder);
                    break;
                default:
                    ThrowEx.NotImplementedCase(prohlizec);
                    break;
            }
            if (builder == null) builder = string.Empty;
            path.Add(prohlizec, builder);
            return builder;
        }
        return path[prohlizec];
    }
}