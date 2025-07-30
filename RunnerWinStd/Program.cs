
namespace RunnerWinStd;
using SunamoWinStd.Tests;

internal class Program
{
    static void Main()
    {
        //PHWin.ExecutableOfAllBrowsers();
        PHWinTests t = new PHWinTests();
        //t.OpenFolderInTotalcmdTest();
        t.OpenInBrowserTest();

        //t.CodeTest();
        //t.CodiumTest();
        //t.CodeInsiderTest();

        //t.CodiumTest();

        ////t.CodeWithLineTest();
        ///
        //JunctionPointTests t = new();
        //t.IsJunctionPoint_Junction_Test();
        //t.CreateWithMklink();

        //Console.WriteLine("Finished");
        Console.ReadLine();
    }
}