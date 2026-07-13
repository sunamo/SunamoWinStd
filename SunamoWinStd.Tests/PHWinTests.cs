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

    // Invariant "Pokud je empty, exe neexistuje na disku! A vice versa!" - kazda detekovana
    // cesta musi byt bud prazdna, nebo ukazovat na skutecne existujici .exe.
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

    // Kdyz je prohlizec nainstalovany kdekoliv ve standardnich korenech, detekce ho musi najit.
    // Regrese: K-Meleon se driv hledal jen na neexistujici D:\paSync ceste, Comodo jen v Program Files
    // (ne x86), Comet nemel detekci vubec.
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
            return; // na tomto stroji neni nainstalovany - nic netestujeme

        var path = PHWin.AddBrowser(browser);
        Assert.False(string.IsNullOrEmpty(path), $"{browser} je na disku, ale detekce vratila prazdnou cestu");
        Assert.True(File.Exists(path), $"Detekovana {browser} cesta neexistuje: {path}");
    }
}
