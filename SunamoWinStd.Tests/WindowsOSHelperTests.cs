using SunamoWinStd._public.SunamoEnums.Enums;

namespace SunamoWinStd.Tests;

/// <summary>
/// Tests for WindowsOSHelper and browser lookup (moved from sunamo.Tests.wpf win.Tests).
/// </summary>
public class WindowsOSHelperTests
{
    /// <summary>
    /// Verifies the browser path lookup against a machine-specific install location.
    /// </summary>
    [Fact(Skip = "Machine-specific path; the original test expected a Seznam browser install path")]
    public void FileInTest()
    {
        var p = PHWin.AddBrowser(Browsers.Chrome);
        Assert.Equal(@"C:\Users\j\AppData\Roaming\Seznam Browser\Seznam.cz.exe", p);
    }
}
