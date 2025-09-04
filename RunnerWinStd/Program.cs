
namespace RunnerWinStd;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SunamoDevCode.SunamoCSharp;
using SunamoGetFiles;
using SunamoWinStd;
using SunamoWinStd.Tests;
using System.Threading.Tasks;

internal class Program
{
    static ILogger logger = NullLogger.Instance;

    private static async Task OpenAllTsTsxVueFilesInVscodeInFolderWorker_ToDelete(List<string> l, bool rec, bool insider)
    {
        var files = l.SelectMany(dir => FSGetFiles.GetFilesEveryFolder(logger, dir, "*.ts;*.tsx;*.vue", SearchOption.AllDirectories, new SunamoGetFiles._public.SunamoArgs.GetFilesEveryFolderArgs() { ExcludeGeneratedCodeFolders = true }));

        foreach (var item in files)
        {
            var c = await File.ReadAllTextAsync(item);
            c = CSharpHelper.RemoveComments(c);

            if (!c.Contains("\"@siesta"))
            {
                continue;
            }

            if (insider)
            {
                PHWin.CodeInsider(logger, item);
            }
            else
            {
                PHWin.Code(logger, item);
            }
            Thread.Sleep(500);
        }
    }

    static void Main()
    {
        //PHWin.ExecutableOfAllBrowsers();
        PHWinTests t = new PHWinTests();
        //t.OpenFolderInTotalcmdTest();
        //t.OpenInBrowserTest();

        //t.CodeTest();
        OpenAllTsTsxVueFilesInVscodeInFolderWorker_ToDelete([@"C:\Proj_Net\_ZaPo\portal-ui\src\shared\"], true, false);

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