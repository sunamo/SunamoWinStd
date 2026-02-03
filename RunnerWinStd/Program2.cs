using SunamoDevCode.SunamoCSharp;
using SunamoGetFiles;
using SunamoWinStd;
using System;
using System.Collections.Generic;
using System.Text;

namespace RunnerWinStd
{
    internal partial class Program
    {
        private static async Task OpenAllTsTsxVueFilesInVscodeInFolderWorker_ToDelete(List<string> list, bool rec, bool insider)
        {
            var files = list.SelectMany(dir => FSGetFiles.GetFilesEveryFolder(logger, dir, "*.ts;*.tsx;*.vue", SearchOption.AllDirectories, new SunamoGetFiles._public.SunamoArgs.GetFilesEveryFolderArgs() { ExcludeGeneratedCodeFolders = true }));

            foreach (var item in files)
            {
                var count = await File.ReadAllTextAsync(item);
                count = CSharpHelper.RemoveComments(count);

                if (!count.Contains("\"@siesta"))
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
    }
}
