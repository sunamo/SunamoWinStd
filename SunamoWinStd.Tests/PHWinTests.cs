// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy

using Microsoft.Extensions.Logging;
using SunamoTest;
using System.Text;

namespace SunamoWinStd.Tests;
public class PHWinTests
{
    ILogger logger = TestLogger.Instance;

    public void CodeTest()
    {
        var path = CreateTestFile();
        PHWin.Code(logger, path, true);
    }

    public void CodeWithLineTest()
    {
        var path = CreateTestFile();
        PHWin.Code(logger, path, true, 150);
    }

    private string CreateTestFile()
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"a.txt");
        if (!File.Exists(path))
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < 200; i++)
            {
                stringBuilder.AppendLine(i.ToString());
            }

            File.WriteAllText(path, stringBuilder.ToString());
        }

        return path;
    }

    public void CodeInsiderTest()
    {
        var path = CreateTestFile();
        PHWin.CodeInsider(logger, path, true);
    }

    public void CodiumTest()
    {
        var path = CreateTestFile();
        PHWin.Codium(logger, path, true);
    }

    public void OpenInBrowserTest()
    {
        PHWin.AddBrowser();
        PHWin.OpenInBrowser(logger, @"D:\OneDrive\sunamo\SeznamkaCz\Output\EveryProcessedAd.html");
    }
}