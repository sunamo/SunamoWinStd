namespace SunamoWinStd.Tests;

/// <summary>
/// Tests of the shell-execute start info; nothing is started.
/// </summary>
public class PHShellExecuteTests
{
    /// <summary>
    /// The start info opens the value itself through the shell, not through cmd.exe.
    /// </summary>
    [Fact]
    public void CreateShellExecuteStartInfo_UsesShellExecuteWithoutCmd()
    {
        var uri = "https://www.spritmonitor.de/en/search.html?searchterm=Octavia&x=1 2";

        var startInfo = PH.CreateShellExecuteStartInfo(uri);

        Assert.True(startInfo.UseShellExecute);
        Assert.Equal(uri, startInfo.FileName);
        Assert.Equal(string.Empty, startInfo.Arguments);
    }

    /// <summary>
    /// StartAllUri with no values starts nothing and does not call the converter.
    /// </summary>
    [Fact]
    public void StartAllUri_EmptyList_DoesNothing()
    {
        var calls = 0;

        PH.StartAllUri(new List<string>(), value => { calls++; return value; });

        Assert.Equal(0, calls);
    }
}
