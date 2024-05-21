namespace SunamoWinStd;


/// <summary>
/// Jsou dělané přesně podle Request.Browser.Browser
/// </summary>
public enum Browsers : byte
{
    // Nic zde nikdy nesmíš měnit, můžeš maximálně přidávat nové prohlížeče
    //Other = 0,
    //Chrome = 1,
    //Firefox = 2,
    //Maxthon = 3,
    ////InternetExplorer = 3,
    //Opera = 4,
    //Edge = 5,
    //Vivaldi = 6,
    //ChromeCanary = 7,
    //Seznam = 8
    Chrome = 1,
    Firefox = 2,
    EdgeBeta = 3,
    Opera = 4,
    Vivaldi = 5,
    //Maxthon = 6,
    Iridium = 6, // Later
    Falkon = 7, //Seznam = 7,
    OperaGX = 8, //Chromium = 8,
    ChromeCanary = 9,
    Tor = 10,
    Bravebrowser = 11,
    ChromeBeta = 12,
    EdgeDev = 13,
    EdgeCanary = 14,
    ChromeDev = 15,
    EdgeStable = 254,
    None = 255
}