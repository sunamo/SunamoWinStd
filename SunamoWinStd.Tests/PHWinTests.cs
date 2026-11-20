using Microsoft.Extensions.Logging;
using SunamoTest;
using SunamoWinStd._public.SunamoEnums.Enums;
using System.Text;

namespace SunamoWinStd.Tests;

/// <summary>
/// Tests for PHWin browser and editor opening functionality.
/// </summary>
public class PHWinTests
{
    private ILogger logger = TestLogger.Instance;

    /// <summary>
    /// Tests opening a file in VS Code.
    /// </summary>
    [Fact]
    public void CodeTest()
    {
        var filePath = CreateTestFile();
        PHWin.Code(logger, filePath, true);
    }

    /// <summary>
    /// Tests opening a file in VS Code at a specific line.
    /// </summary>
    [Fact]
    public void CodeWithLineTest()
    {
        var filePath = CreateTestFile();
        PHWin.Code(logger, filePath, true, 150);
    }

    private string CreateTestFile()
    {
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"a.txt");
        if (!File.Exists(filePath))
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < 200; i++)
            {
                stringBuilder.AppendLine(i.ToString());
            }

            File.WriteAllText(filePath, stringBuilder.ToString());
        }

        return filePath;
    }

    /// <summary>
    /// Tests opening a file in VS Code Insiders.
    /// </summary>
    [Fact]
    public void CodeInsiderTest()
    {
        var filePath = CreateTestFile();
        PHWin.CodeInsider(logger, filePath, true);
    }

    /// <summary>
    /// Tests opening a file in VSCodium.
    /// </summary>
    [Fact]
    public void CodiumTest()
    {
        var filePath = CreateTestFile();
        PHWin.Codium(logger, filePath, true);
    }

    /// <summary>
    /// Tests opening a URL in the default browser.
    /// </summary>
    [Fact]
    public void OpenInBrowserTest()
    {
        PHWin.AddBrowser();
        var testFilePath = Path.Combine(Path.GetTempPath(), "TestBrowserOpen.html");
        File.WriteAllText(testFilePath, "<html><body><h1>Test</h1></body></html>");
        PHWin.OpenInBrowser(logger, testFilePath);
    }

    // Invariant "If empty, the exe does not exist on disk! And vice versa!" - each detected
    // the path must be either empty or point to a really existing .exe.
    [Fact]
    public void EveryBrowserPathIsEmptyOrExists()
    {
        PHWin.AddBrowsers();
        foreach (var kvp in PHWin.BrowserPaths)
        {
            if (!string.IsNullOrEmpty(kvp.Value))
                Assert.True(File.Exists(kvp.Value),
                    $"{kvp.Key} ma neprazdnou cestu, ktera ale na disku neexistuje: {kvp.Value}");
        }
    }

    // When a browser is installed anywhere in the standard roots, detection must find it.
    // Regression: K-Meleon used to be searched only in the nonexistent path D:\paSync, Comodo only in Program Files
    // (not x86), Comet had no detection at all.
    [Theory]
    [InlineData(Browsers.KMeleon, "K-Meleon", "k-meleon.exe")]
    [InlineData(Browsers.Comodo, @"Comodo\Dragon", "dragon.exe")]
    [InlineData(Browsers.Comet, @"Perplexity\Comet", "comet.exe")]
    public void InstalledBrowserIsDetected(Browsers browser, string appFolder, string executableName)
    {
        var roots = new[]
        {
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        };
        bool installed = roots.Where(r => !string.IsNullOrEmpty(r))
            .Select(r => Path.Combine(r, appFolder))
            .Any(f => Directory.Exists(f) &&
                      Directory.EnumerateFiles(f, executableName, SearchOption.AllDirectories).Any());
        if (!installed)
            return; // not installed on this machine - we test nothing

        var path = PHWin.AddBrowser(browser);
        Assert.False(string.IsNullOrEmpty(path), $"{browser} je na disku, ale detekce vratila prazdnou cestu");
        Assert.True(File.Exists(path), $"Detekovana {browser} cesta neexistuje: {path}");
    }
}
