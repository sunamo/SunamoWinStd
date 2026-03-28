namespace SunamoWinStd;

/// <summary>
/// Windows-specific process helper for browser management, editor launching, and file operations.
/// </summary>
public partial class PHWin
{
    private const string CodiumExe = "VSCodium.exe";
    private const string CodeExe = "Code.exe";
    private const string WebStorm64Exe = "WebStorm64.exe";
    private const string CodeInsiderExe = "Code - Insiders.exe";
    private static readonly Browsers defaultBrowser = Browsers.EdgeStable;
    /// <summary>
    /// Number of browser tabs opened in current session.
    /// </summary>
    public static int Opened { get; set; }
    /// <summary>
    ///     Browser executable paths indexed by browser type. Does not contain Browsers.None.
    /// </summary>
    public static Dictionary<Browsers, string> BrowserPaths { get; set; } = new();
    /// <summary>
    /// Executable paths indexed by executable name.
    /// </summary>
    public static Dictionary<string, string> ExecutablePaths { get; set; } = new();
    /// <summary>
    /// Milliseconds to wait after every 10 opened tabs. 0 means no wait.
    /// </summary>
    public static int WaitIfTen { get; set; } = 0;
    private static readonly int browserCount;
    /// <summary>
    /// Preferred code editor to use for opening files.
    /// </summary>
    public static Editor PreferredEditor { get; set; } = Editor.Code;
    static PHWin()
    {
        var browsers = Enum.GetValues<Browsers>().ToList();
        browserCount = browsers.Count;
    }
    private static
#if ASYNC
        async Task
#else
    void
#endif
        BreakIfTenIde()
    {
    }
    /// <summary>
    /// Opens a folder in Total Commander.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="folder">Folder path to open.</param>
    /// <param name="isThrowingOnError">Whether to throw on error.</param>
    public static void OpenFolderInTotalcmd(ILogger logger, string folder, bool isThrowingOnError = false)
    {
        PH.RunFromPath(logger, "totalcmd.exe", "/O /T " + folder, false, isThrowingOnError);
    }
    /// <summary>
    /// Opens a file in WebStorm 64-bit.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="filePath">File path to open.</param>
    /// <param name="isThrowingOnError">Whether to throw on error.</param>
    public static void WebStorm64(ILogger logger, string filePath, bool isThrowingOnError = false)
    {
        PH.RunFromPath(logger, WebStorm64Exe, filePath, false, isThrowingOnError);
    }
    /// <summary>
    /// Initializes browser executable paths for all known browsers.
    /// </summary>
    public static void AddBrowsers()
    {
        if (BrowserPaths.Count == 0)
        {
            var allBrowsers = Enum.GetValues<Browsers>();
            foreach (var item in allBrowsers)
                if (item != Browsers.None)
                    AddBrowser(item);
        }
    }
    /// <summary>
    ///     Opens a URL in the specified browser.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="browser">Browser to use.</param>
    /// <param name="text">URL or path to open.</param>
    /// <param name="waitMs">Milliseconds to wait after opening. 0 is not recommended as Google may show captcha for many searches.</param>
    /// <param name="isForceAttemptingHttps">Whether to force HTTPS prefix.</param>
    /// <param name="isThrowingIfNotValidUrl">Whether to throw if the URL is invalid.</param>
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
            if (!text.StartsWith("http")) text = "https://" + text;
            if (!Uri.TryCreate(text, new UriCreationOptions(), out var _))
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
    /// <summary>
    ///     Opens a URL in the default browser.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="uri">URL to open.</param>
    /// <param name="waitMs">Milliseconds to wait after opening. 0 is not recommended as Google may show captcha for many searches.</param>
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
    /// Opens a file in the preferred editor.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="filePath">File path to open.</param>
    /// <param name="isThrowingOnError">Whether to throw on error.</param>
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

    /// <summary>
    /// Opens a URL in the specified browser and tracks open count.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="browser">Browser to use.</param>
    /// <param name="text">URL or path to open.</param>
    /// <param name="waitMs">Milliseconds to wait after opening. 0 is not recommended as Google may show captcha for many searches.</param>
    public static void OpenInBrowserAutomaticallyCountOfOpened(ILogger logger, Browsers browser, string text, int waitMs = 500)
    {
        OpenInBrowser(logger, browser, text, waitMs);
    }

    /// <summary>
    /// Returns list of browser executable paths that do not exist on disk.
    /// </summary>
    /// <returns>List of missing browser executable paths.</returns>
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
    /// Prints executable paths of all known browsers to console.
    /// </summary>
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
    /// Opens a single URI in all known browsers.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="uri">URI to open.</param>
    public static void OpenInAllBrowsers(ILogger logger, string uri)
    {
        OpenInAllBrowsers(logger, new List<string>([uri]));
    }
    /// <summary>
    /// Opens multiple URIs in all known browsers.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="uris">List of URIs to open.</param>
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
    /// Opens a folder in Windows Explorer.
    /// </summary>
    /// <param name="folder">Folder path to open.</param>
    public static void OpenFolder(string folder)
    {
        Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", folder);
    }
    /// <summary>
    /// Saves HTML content to a temp file and opens it in the specified browser.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="browser">Browser to use.</param>
    /// <param name="htmlContent">HTML content to save and open.</param>
    public static void SaveAndOpenInBrowser(ILogger logger, Browsers browser, string htmlContent)
    {
        var tempFilePath = Path.GetTempFileName() + ".html";
        File.WriteAllText(tempFilePath, htmlContent);
        OpenInBrowser(logger, browser, tempFilePath, 50);
    }
    /// <summary>
    /// Checks if a file is currently in use by any process.
    /// </summary>
    /// <param name="fullPath">Full path to the file.</param>
    /// <returns>True if the file is locked by a process.</returns>
    public static bool IsUsed(string fullPath)
    {
        return FileUtil.WhoIsLocking(fullPath).Count > 0;
    }
}
