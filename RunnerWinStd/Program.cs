// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy


namespace RunnerWinStd;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SunamoDevCode.SunamoCSharp;
using SunamoGetFiles;
using SunamoWinStd;
using SunamoWinStd.Tests;
using System.Threading.Tasks;

internal partial class Program
{
    static ILogger logger = NullLogger.Instance;

    

    static void Main()
    {
        //PHWin.ExecutableOfAllBrowsers();
        PHWinTests t = new PHWinTests();
        //t.OpenFolderInTotalcmdTest();
        //t.OpenInBrowserTest();

        //t.CodeTest();
        //OpenAllTsTsxVueFilesInVscodeInFolderWorker_ToDelete([@"C:\Proj_Net\_ZaPo\portal-ui\src\shared\"], true, false);

        //t.CodeTest();
        //t.CodiumTest();
        //t.CodeInsiderTest();

        PHWin.Codium(logger, @"D:\Downloads\At ziji duchove.srt");

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