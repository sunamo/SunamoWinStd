namespace SunamoWinStd;

/// <summary>
/// Represents the PH win type.
/// </summary>
public partial class PHWin
{
    private const string CodiumExe = "VSCodium.exe";
    private const string CodeExe = "Code.exe";
    private const string WebStorm64Exe = "WebStorm64.exe";
    private const string CodeInsiderExe = "Code - Insiders.exe";
    private static readonly Browsers defaultBrowser = Browsers.EdgeStable;
    /// <summary>
    /// Gets or sets the opened.
    /// </summary>
    public static int Opened { get; set; }
    // Browser executable paths indexed by browser type. Does not contain Browsers.None.
    /// <summary>
    /// Gets or sets the browser paths.
    /// </summary>
    public static Dictionary<Browsers, string> BrowserPaths { get; set; } = new();
    /// <summary>
    /// Gets or sets the executable paths.
    /// </summary>
    public static Dictionary<string, string> ExecutablePaths { get; set; } = new();
    // Milliseconds to wait after every 10 opened tabs. 0 means no wait.
    /// <summary>
    /// Gets or sets the wait if ten.
    /// </summary>
    public static int WaitIfTen { get; set; } = 0;
    private static readonly int browserCount;
    /// <summary>
    /// Gets or sets the preferred editor.
    /// </summary>
    public static Editor PreferredEditor { get; set; } = Editor.Code;
    static PHWin()
    {
        var browsers = ((Browsers[])Enum.GetValues(typeof(Browsers))).ToList();
        browserCount = browsers.Count;
    }
    private static
        async Task
        BreakIfTenIde()
    {
    }
    /// <summary>
    /// Open folder in totalcmd.
    /// </summary>
    /// <returns>The open folder in totalcmd result.</returns>
    public static void OpenFolderInTotalcmd(ILogger logger, string folder, bool isThrowingOnError = false)
    {
        PH.RunFromPath(logger, "totalcmd.exe", "/O /T " + folder, false, isThrowingOnError);
    }
    /// <summary>
    /// Web storm 64.
    /// </summary>
    /// <returns>The web storm 64 result.</returns>
    public static void WebStorm64(ILogger logger, string filePath, bool isThrowingOnError = false)
    {
        PH.RunFromPath(logger, WebStorm64Exe, filePath, false, isThrowingOnError);
    }
    /// <summary>
    /// Add browsers.
    /// </summary>
    /// <returns>The add browsers result.</returns>
    public static void AddBrowsers()
    {
        if (BrowserPaths.Count == 0)
        {
            var allBrowsers = ((Browsers[])Enum.GetValues(typeof(Browsers)));
            foreach (var item in allBrowsers)
                if (item != Browsers.None)
                    AddBrowser(item);
        }
    }
    // waitMs: 0 is not recommended as Google may show captcha for many searches.
    /// <summary>
    /// Open in browser.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="isThrowingOnError">The is throwing on error.</param>
    public static void OpenInBrowser(ILogger logger, Browsers browser, string text, int waitMs = 500,
        bool isForceAttemptingHttps = false, bool isThrowingIfNotValidUrl = false)
    {
        PathFormatDetectorService pathFormatDetector = new(logger);


        if (isForceAttemptingHttps) text = UH.AppendHttpsIfNotExists(text);
        var browserPath = BrowserPaths[browser];
        BreakIfTen();

        if (pathFormatDetector.DetectPathType(text) == null)
        {

            text = PH.NormalizeUri(text);
            if (!text.StartsWith("http"))
            {
                if (isForceAttemptingHttps)
                    text = "https://" + text;
                else
                {
                    logger.LogWarning("Skipping non-URL value: " + text);
                    return;
                }
            }
            if (!Uri.TryCreate(text, UriKind.Absolute, out var _))
            {
                logger.LogError($"Can't create uri from " + text);
                if (isThrowingIfNotValidUrl)
                {
                    ThrowEx.Custom($"Can't create uri from " + text);
                }
                return;
            }
        }

        text = "/new-tab " + text;
        Process.Start(new ProcessStartInfo(browserPath, text));
        Thread.Sleep(100);
        if (waitMs > 0) Thread.Sleep(waitMs);
    }
    internal static void BreakIfTen()
    {
        Opened++;
        if (Opened % 10 == 0)
            if (WaitIfTen != 0)
                Thread.Sleep(WaitIfTen);
    }
    // waitMs: 0 is not recommended as Google may show captcha for many searches.
    /// <summary>
    /// Open in browser.
    /// </summary>
    /// <param name="text)">The text.</param>
    /// <returns>The open in browser result.</returns>
    public static void OpenInBrowser(ILogger logger, string uri,
        int waitMs = 500)
    {
        OpenInBrowser(logger, defaultBrowser, uri, waitMs);
    }
    private static void NullIfNotExists(ref string? browserPath)
    {
        if (!File.Exists(browserPath)) browserPath = null;
    }
    /// <summary>
    /// Preferred editor open.
    /// </summary>
    /// <param name="10">The 10.</param>
    /// <returns>The preferred editor open result.</returns>
    public static void PreferredEditorOpen(ILogger logger, string filePath, bool isThrowingOnError = false)
    {
        switch (PreferredEditor)
        {
            case Editor.Code:
                Code(logger, filePath, isThrowingOnError);
                break;
            case Editor.Codium:
                Codium(logger, filePath, isThrowingOnError);
                break;
            case Editor.CodeInsider:
                CodeInsider(logger, filePath, isThrowingOnError);
                break;
            default:
                ThrowEx.NotImplementedCase(PreferredEditor);
                break;
        }
    }

    // waitMs: 0 is not recommended as Google may show captcha for many searches.
    /// <summary>
    /// Initializes a new instance of the open in browser automatically count of opened class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="isThrowingOnError">The is throwing on error.</param>
    public static void OpenInBrowserAutomaticallyCountOfOpened(ILogger logger, Browsers browser, string text, int waitMs = 500)
    {
        OpenInBrowser(logger, browser, text, waitMs);
    }

    /// <summary>
    /// Browsers which dont have exe in defined path.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="isThrowingOnError">The is throwing on error.</param>
    /// <returns>The browsers which dont have exe in defined path result.</returns>
    public static List<string> BrowsersWhichDontHaveExeInDefinedPath()
    {
        var missingPaths = new List<string>();
        AddBrowsers();
        foreach (var item in BrowserPaths)
            if (!File.Exists(item.Value))
                missingPaths.Add(item.Value);
        return missingPaths;
    }
    /// <summary>
    /// Executable of all browsers.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="browser">The browser.</param>
    /// <param name="text">The text.</param>
    /// <param name="waitMs">The wait ms.</param>
    /// <returns>The executable of all browsers result.</returns>
    public static void ExecutableOfAllBrowsers()
    {
        PHWin.AddBrowsers();
        Console.WriteLine("If empty, the exe does not exist on disk! And vice versa!");
        foreach (var item in PHWin.BrowserPaths)
        {
            Console.WriteLine(item.Key + ": " + SH.NullToStringOrDefault(item.Value));
        }
    }
    /// <summary>
    /// Initializes a new instance of the open in all browsers class.
    /// </summary>
    public static void OpenInAllBrowsers(ILogger logger, string uri)
    {
        OpenInAllBrowsers(logger, new List<string>([uri]));
    }
    /// <summary>
    /// Open in all browsers.
    /// </summary>
    /// <returns>The open in all browsers result.</returns>
    public static void OpenInAllBrowsers(ILogger logger, IList<string> uris)
    {
        AddBrowsers();
        foreach (var uri in uris)
            foreach (var item in BrowserPaths)
            {
                if (item.Key == Browsers.Tor) continue;
                OpenInBrowser(logger, item.Key, uri, 50);
            }
    }
    /// <summary>
    /// Open folder.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="uri">The uri.</param>
    /// <returns>The open folder result.</returns>
    public static void OpenFolder(string folder)
    {
        Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", folder);
    }
    /// <summary>
    /// Save and open in browser.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="uris">The uris.</param>
    /// <returns>The save and open in browser result.</returns>
    public static void SaveAndOpenInBrowser(ILogger logger, Browsers browser, string htmlContent)
    {
        var tempFilePath = Path.GetTempFileName() + ".html";
        File.WriteAllText(tempFilePath, htmlContent);
        OpenInBrowser(logger, browser, tempFilePath, 50);
    }
    /// <summary>
    /// Is used.
    /// </summary>
    /// <param name="item.Key">The item key.</param>
    /// <returns>The is used result.</returns>
    public static bool IsUsed(string fullPath)
    {
        return FileUtil.WhoIsLocking(fullPath).Count > 0;
    }
}
