using Microsoft.Extensions.Logging;
using SunamoTest;
using SunamoWinStd;
using System.Text;
using TextCopy;

enum LinkType
{
    H, J, D
}


public class JunctionPointTests
{
    ILogger logger = TestLogger.Instance;
    /// <summary>
    /// Nastavuje se v metodě SetFor
    /// </summary>
    string target = null;
    const string target2 = @"D:\_Test\sunamo\win\JunctionPoint\";
    string Folder => "Folder";
    string File => "File";

    [Fact]
    public void GetTargetTest()
    {
        //TestHelper.Init();

        var d = SetFor(LinkType.D);
        var td = JunctionPoint.GetTarget(d);
        // target2 - also folder
        var rd = JunctionPoint.GetTarget(target2);
        var ed = Directory.Exists(d);

        var j = SetFor(LinkType.J);
        var tj = JunctionPoint.GetTarget(j);
        //var rj = JunctionPoint.GetTarget(target);
        var ej = Directory.Exists(j);

        SetFor(LinkType.H);
        var h = SetFor(LinkType.H);
        var th = JunctionPoint.GetTarget(h);
        var rh = JunctionPoint.GetTarget(target);
        var eh = Directory.Exists(h);
    }

    public void CreateWithMklink()
    {
        var path = target2;
        while (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            path = Path.GetDirectoryName(path);
        }

        StringBuilder sb = new();

        sb.AppendLine($"mkdir {target2}{Folder}");
        sb.AppendLine($"cd {target2}{Folder}");
        sb.AppendLine("echo \"Tento text bude vložen do souboru.\" > soubor.txt");
        sb.AppendLine($"cmd /c mklink /J {J()} {target2}{Folder}");
        sb.AppendLine();

        ClipboardService.SetText(sb.ToString());
    }

    [Fact]
    public void IsJunctionPoint_Junction_Test()
    {
        SetFor(LinkType.J);

        var j = J();
        var b = JunctionPoint.IsJunctionPoint(logger, j);
        var d = JunctionPoint.IsJunctionPoint(logger, target2 + Folder);
    }

    [Fact]
    public void IsJunctionPointTest()
    {


        var a = JunctionPoint.IsJunctionPoint(logger, D());

        var c = JunctionPoint.IsJunctionPoint(logger, target);


        var d = JunctionPoint.IsReparsePoint(D());
        var e = JunctionPoint.IsReparsePoint(J());

        var f = JunctionPoint.IsReparsePoint(target);
    }

    string SetFor(LinkType lt)
    {
        if (lt == LinkType.J || lt == LinkType.D)
        {
            target = target2 + Folder;
        }
        else
        {
            target = target2 + File;
        }

        string result = null;
        switch (lt)
        {
            case LinkType.H:
                result = H();
                break;
            case LinkType.J:
                result = J();
                break;
            case LinkType.D:
                result = D();
                break;
        }

        return result;
    }

    string H()
    {
        return @"D:\_Test\sunamo\win\JunctionPoint\H_" + Folder;
    }

    string J()
    {
        return @"D:\_Test\sunamo\win\JunctionPoint\J_" + Folder;
    }

    string D()
    {
        return @"D:\_Test\sunamo\win\JunctionPoint\D_" + Folder;
    }

    [Fact]
    public void MklinkH()
    {

        SetFor(LinkType.H);
        JunctionPoint.MklinkH(H() + ".txt", target + ".txt");
    }

    [Fact]
    public void MklinkJ()
    {

        SetFor(LinkType.J);
        var j = J();
        JunctionPoint.Delete(logger, j);
        JunctionPoint.MklinkJ(j, target);
    }

    [Fact]
    public void MklinkD()
    {
        SetFor(LinkType.D);
        JunctionPoint.MklinkD(D(), target);
    }
}