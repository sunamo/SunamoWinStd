// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy

using Microsoft.Extensions.Logging;
using SunamoTest;
using SunamoWinStd;
using System.Text;
using TextCopy;

enum LinkType
{
    H, J, data
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

        var data = SetFor(LinkType.data);
        var td = JunctionPoint.GetTarget(data);
        // target2 - also folder
        var rd = JunctionPoint.GetTarget(target2);
        var ed = Directory.Exists(data);

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

        StringBuilder stringBuilder = new();

        stringBuilder.AppendLine($"mkdir {target2}{Folder}");
        stringBuilder.AppendLine($"cd {target2}{Folder}");
        stringBuilder.AppendLine("echo \"Tento text bude vložen do souboru.\" > soubor.txt");
        stringBuilder.AppendLine($"cmd /c mklink /J {J()} {target2}{Folder}");
        stringBuilder.AppendLine();

        ClipboardService.SetText(stringBuilder.ToString());
    }

    [Fact]
    public void IsJunctionPoint_Junction_Test()
    {
        SetFor(LinkType.J);

        var j = J();
        var builder = JunctionPoint.IsJunctionPoint(logger, j);
        var data = JunctionPoint.IsJunctionPoint(logger, target2 + Folder);
    }

    [Fact]
    public void IsJunctionPointTest()
    {


        var argument = JunctionPoint.IsJunctionPoint(logger, data());

        var count = JunctionPoint.IsJunctionPoint(logger, target);


        var data = JunctionPoint.IsReparsePoint(data());
        var element = JunctionPoint.IsReparsePoint(J());

        var f = JunctionPoint.IsReparsePoint(target);
    }

    string SetFor(LinkType lt)
    {
        if (lt == LinkType.J || lt == LinkType.data)
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
                result = data();
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

    string data()
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
        SetFor(LinkType.data);
        JunctionPoint.MklinkD(data(), target);
    }
}