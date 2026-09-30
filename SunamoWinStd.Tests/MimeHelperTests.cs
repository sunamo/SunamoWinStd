using SunamoWinStd.Helpers;

namespace SunamoWinStd.Tests;

/// <summary>
/// Tests for MimeHelper (moved from sunamo.Tests.wpf win.Tests).
/// </summary>
public class MimeHelperTests
{
    /// <summary>
    /// Verifies MIME detection of real files stored in the local test data folder.
    /// </summary>
    [Fact(Skip = "Requires local test data in D:/_Test/sunamo")]
    public void GetMimeFromFileTest()
    {
        var f = @"D:\_Test\sunamo\win\Helpers\MImeHelper\GetMimeFromFile\Real";
        //application/octet-stream>
        Assert.Equal(string.Empty, MimeHelper.GetMimeFromFile(f + ".webp"));
        // 
        Assert.Equal(string.Empty, MimeHelper.GetMimeFromFile(f + ".jpg"));
    }
}
