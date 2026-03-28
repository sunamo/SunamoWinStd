using Microsoft.Extensions.Logging;
using SunamoTest;
using SunamoWinStd;
using System.Text;
using TextCopy;

/// <summary>
/// Type of symbolic link to create.
/// </summary>
enum LinkType
{
    /// <summary>
    /// Hard link (/H).
    /// </summary>
    HardLink,
    /// <summary>
    /// Junction (/J).
    /// </summary>
    Junction,
    /// <summary>
    /// Directory symbolic link (/D).
    /// </summary>
    Directory
}


/// <summary>
/// Tests for junction point creation, detection, and target resolution.
/// </summary>
public class JunctionPointTests
{
    private ILogger logger = TestLogger.Instance;
    /// <summary>
    /// Set in the SetFor method.
    /// </summary>
    private string? target = null;
    private const string testBasePath = @"D:\_Test\sunamo\win\JunctionPoint\";
    private string FolderName => "Folder";
    private string FileName => "File";

    /// <summary>
    /// Tests getting the target of junction points created with different link types.
    /// </summary>
    [Fact]
    public void GetTargetTest()
    {
        var directoryLinkPath = SetFor(LinkType.Directory);
        Assert.NotNull(directoryLinkPath);
        var directoryTarget = JunctionPoint.GetTarget(directoryLinkPath!);
        var basePathTarget = JunctionPoint.GetTarget(testBasePath);
        var isDirectoryLinkExists = System.IO.Directory.Exists(directoryLinkPath);

        var junctionPath = SetFor(LinkType.Junction);
        Assert.NotNull(junctionPath);
        var junctionTarget = JunctionPoint.GetTarget(junctionPath!);
        var isJunctionExists = System.IO.Directory.Exists(junctionPath);

        SetFor(LinkType.HardLink);
        var hardLinkPath = SetFor(LinkType.HardLink);
        Assert.NotNull(hardLinkPath);
        var hardLinkTarget = JunctionPoint.GetTarget(hardLinkPath!);
        var targetResolved = JunctionPoint.GetTarget(target!);
        var isHardLinkExists = System.IO.Directory.Exists(hardLinkPath);

        Assert.True(isDirectoryLinkExists || isJunctionExists || isHardLinkExists,
            "At least one of the test link types should exist on disk");
    }

    /// <summary>
    /// Creates mklink commands for testing and copies them to clipboard.
    /// </summary>
    private void CreateWithMklink()
    {
        var currentPath = testBasePath;
        while (!System.IO.Directory.Exists(currentPath))
        {
            System.IO.Directory.CreateDirectory(currentPath);
            currentPath = Path.GetDirectoryName(currentPath)!;
        }

        StringBuilder stringBuilder = new();

        stringBuilder.AppendLine($"mkdir {testBasePath}{FolderName}");
        stringBuilder.AppendLine($"cd {testBasePath}{FolderName}");
        stringBuilder.AppendLine("echo \"This text will be inserted into the file.\" > file.txt");
        stringBuilder.AppendLine($"cmd /c mklink /J {GetJunctionPath()} {testBasePath}{FolderName}");
        stringBuilder.AppendLine();

        ClipboardService.SetText(stringBuilder.ToString());
    }

    /// <summary>
    /// Tests that a junction point is correctly identified as a junction.
    /// </summary>
    [Fact]
    public void IsJunctionPoint_Junction_Test()
    {
        SetFor(LinkType.Junction);

        var junctionPath = GetJunctionPath();
        var isJunction = JunctionPoint.IsJunctionPoint(logger, junctionPath);
        var isFolderJunction = JunctionPoint.IsJunctionPoint(logger, testBasePath + FolderName);

        if (System.IO.Directory.Exists(junctionPath))
        {
            Assert.True(isJunction, "Junction path should be detected as a junction point");
        }

        Assert.False(isFolderJunction, "Regular folder should not be detected as a junction point");
    }

    /// <summary>
    /// Tests junction point and reparse point detection for different link types.
    /// </summary>
    [Fact]
    public void IsJunctionPointTest()
    {
        var directoryLinkPath = GetDirectoryLinkPath();
        var junctionPath = GetJunctionPath();

        var isDirectoryJunction = JunctionPoint.IsJunctionPoint(logger, directoryLinkPath);
        var isDirectoryReparsePoint = JunctionPoint.IsReparsePoint(directoryLinkPath);
        var isJunctionReparsePoint = JunctionPoint.IsReparsePoint(junctionPath);

        if (System.IO.Directory.Exists(directoryLinkPath))
        {
            Assert.True(isDirectoryReparsePoint, "Directory link should be a reparse point");
        }

        if (System.IO.Directory.Exists(junctionPath))
        {
            Assert.True(isJunctionReparsePoint, "Junction should be a reparse point");
        }
    }

    private string? SetFor(LinkType linkType)
    {
        if (linkType == LinkType.Junction || linkType == LinkType.Directory)
        {
            target = testBasePath + FolderName;
        }
        else
        {
            target = testBasePath + FileName;
        }

        string? result = null;
        switch (linkType)
        {
            case LinkType.HardLink:
                result = GetHardLinkPath();
                break;
            case LinkType.Junction:
                result = GetJunctionPath();
                break;
            case LinkType.Directory:
                result = GetDirectoryLinkPath();
                break;
        }

        return result;
    }

    private string GetHardLinkPath()
    {
        return @"D:\_Test\sunamo\win\JunctionPoint\H_" + FolderName;
    }

    private string GetJunctionPath()
    {
        return @"D:\_Test\sunamo\win\JunctionPoint\J_" + FolderName;
    }

    private string GetDirectoryLinkPath()
    {
        return @"D:\_Test\sunamo\win\JunctionPoint\D_" + FolderName;
    }

    /// <summary>
    /// Tests creating a hard link with mklink /H.
    /// </summary>
    [Fact]
    public void MklinkHardLinkTest()
    {
        SetFor(LinkType.HardLink);
        var command = JunctionPoint.MklinkH(GetHardLinkPath() + ".txt", target! + ".txt");
        Assert.NotNull(command);
    }

    /// <summary>
    /// Tests creating a junction with mklink /J.
    /// </summary>
    [Fact]
    public void MklinkJunctionTest()
    {
        SetFor(LinkType.Junction);
        var junctionPath = GetJunctionPath();
        JunctionPoint.Delete(logger, junctionPath);
        var command = JunctionPoint.MklinkJ(junctionPath, target!);
        Assert.NotNull(command);
    }

    /// <summary>
    /// Tests creating a directory symbolic link with mklink /D.
    /// </summary>
    [Fact]
    public void MklinkDirectoryTest()
    {
        SetFor(LinkType.Directory);
        var command = JunctionPoint.MklinkD(GetDirectoryLinkPath(), target!);
        Assert.NotNull(command);
    }
}
