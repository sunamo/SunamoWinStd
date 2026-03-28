using Microsoft.Extensions.Logging;
using SunamoTest;
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
}
