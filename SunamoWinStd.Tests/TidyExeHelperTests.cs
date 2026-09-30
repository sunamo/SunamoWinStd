namespace SunamoWinStd.Tests;

/// <summary>
/// Tests for TidyExeHelper (moved from sunamo.Tests.wpf win.Tests).
/// </summary>
public class TidyExeHelperTests
{
    /// <summary>
    /// Verifies that the embedded tidy configuration is written next to the executable.
    /// </summary>
    [Fact]
    public void WriteTidyConfigToExecutableLocationTest()
    {
        var tidyConfig = TidyExeHelper.WriteTidyConfigToExecutableLocation();
        Assert.True(File.Exists(tidyConfig));
        Assert.Contains("doctype: omit", File.ReadAllText(tidyConfig));
    }

    /// <summary>
    /// Formats HTML from the local test data folder with tidy.
    /// </summary>
    [Fact(Skip = "Requires local test data in D:/_Test/sunamo")]
    public async Task FormatHtmlTest()
    {
        var tidyConfig = TidyExeHelper.WriteTidyConfigToExecutableLocation();

        var content = File.ReadAllText(@"D:\_Test\sunamo\SunamoTidy\FormatHtml\1.html");

        var actual = await TidyExeHelper.FormatHtml(content, tidyConfig,
            commands => Task.FromResult(new List<List<string>>()));
    }
}
