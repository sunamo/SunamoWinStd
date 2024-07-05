namespace SunamoWinStd.Helpers;





//using cl;


public class FSWin //: IFSWin
{
    public static FSWin ci = new FSWin();

    private static void Terminate(List<Process> pr)
    {
        foreach (var item in pr)
        {
            Terminate(item);
        }
    }


    private static void Terminate(Process item)
    {
        //Thread.Sleep(10000);
        Task.Factory.StartNew(() => { item.Kill(); });
        item.WaitForExit();
    }

    public static void DeleteFileMaybeLocked(string s)
    {
        var pr = FileUtil.WhoIsLocking(s);
        Terminate(pr);
        FS.TryDeleteFile(s);
    }

    public static void DeleteFileOrFolderMaybeLocked(string p)
    {
        Console.WriteLine("DeleteFileOrFolderMaybeLocked: " + p);
        if (File.Exists(p))
        {
            DeleteFileMaybeLocked(p);
            if (File.Exists(p))
            {
                ThisApp.Error(p + " could not be deleted! Press enter to continue!");
                Console.ReadLine();
            }
            else
            {
                ThisApp.Success(p + " was deleted completely!");
            }
        }
        else if (Directory.Exists(p))
        {
            var files = Directory.GetFiles(p, "*", SearchOption.AllDirectories);

            foreach (var item in files)
            {
                //if (RandomHelper.RandomBool())
                //{
                //    continue;
                //}
                DeleteFileMaybeLocked(item);
            }
            files = Directory.GetFiles(p, "*", SearchOption.AllDirectories);
            if (files.Length == 0)
            {
                Directory.Delete(p, true);
                ThisApp.Success(p + " was deleted completely!");
            }
            else
            {
                ThisApp.Error(p + " could not be deleted completely! Press enter to continue!");
                Console.ReadLine();
            }
        }
        else
        {
            // Only warning, not exc with stacktrace cecause is using in Quadient
            ThisApp.Warning("Doesnt exists as file / folder:" + p);
            //ThrowEx.FileDoesntExists(p);
        }
    }

    public static Type type = typeof(FSWin);

    /// <summary>
    /// Nedařilo se mi s tímhle mazat git složky
    /// 
    /// řešením bylo otevřít git bash a rm -rf .git
    /// </summary>
    /// <param name="p"></param>


    /// <summary>
    /// <summary>
    /// Jednodušší bude si udělat push a celou složku smazat
    /// V Azuru poté uvidím všechny změny, to bych sice viděl i ve forku ale musel bych to přidávat
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="v"></param>
    public static void MoveFolderMaybeLocked(string arg1, string v)
    {
        FS.WithEndSlash(ref arg1);
        FS.WithEndSlash(ref v);

        var files = Directory.GetFiles(arg1, "*", SearchOption.AllDirectories);
        foreach (var item in files)
        {
            var np = item.Replace(arg1, v);
            var pr = FileUtil.WhoIsLocking(item, false);
            Terminate(pr);

            FS.CreateUpfoldersPsysicallyUnlessThere(np);
            if (File.Exists(item))
            {
                File.Move(item, np);
            }
        }

        files = Directory.GetFiles(arg1, "*", SearchOption.AllDirectories);
        if (files.Length == 0)
        {
            Directory.Delete(arg1, true);
        }
        else
        {
            ThisApp.Error("Not all files was moved! " + arg1);
            Console.ReadLine();
        }
    }
}
