namespace SunamoWinStd._sunamo.SunamoFileSystem;

internal class FS
{
    internal static bool TryDeleteFile(string item)
    {
        // TODO: To all code message logging as here

        try
        {
            // If file won't exists, wont throw any exception
            File.Delete(item);
            return true;
        }
        catch
        {
            //ThisApp.Error(sess.i18n(XlfKeys.FileCanTBeDeleted) + ": " + item);
            return false;
        }
    }
    internal static void CreateUpfoldersPsysicallyUnlessThere(string nad)
    {
        CreateFoldersPsysicallyUnlessThere(Path.GetDirectoryName(nad));
    }

    internal static void CreateFoldersPsysicallyUnlessThere(string nad)
    {
        ThrowEx.IsNullOrEmpty("nad", nad);
        //ThrowEx.IsNotWindowsPathFormat("nad", nad);


        if (Directory.Exists(nad))
        {
            return;
        }

        List<string> slozkyKVytvoreni = new List<string>
        {
            nad
        };

        while (true)
        {
            nad = Path.GetDirectoryName(nad);

            // TODO: Tady to nefunguje pro UWP/UAP apps protoze nemaji pristup k celemu disku. Zjistit co to je UWP/UAP/... a jak v nem ziskat/overit jakoukoliv slozku na disku
            if (Directory.Exists(nad))
            {
                break;
            }

            string kopia = nad;
            slozkyKVytvoreni.Add(kopia);
        }

        slozkyKVytvoreni.Reverse();
        foreach (string item in slozkyKVytvoreni)
        {
            string folder = item;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
    }
    internal static void FirstCharUpper(ref string nazevPP)
    {
        nazevPP = FirstCharUpper(nazevPP);
    }

    internal static string FirstCharUpper(string nazevPP)
    {
        if (nazevPP.Length == 1)
        {
            return nazevPP.ToUpper();
        }

        string sb = nazevPP.Substring(1);
        return nazevPP[0].ToString().ToUpper() + sb;
    }

    internal static Dictionary<string, List<string>> GetDictionaryByFileNameWithExtension(List<string> files)
    {
        Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
        foreach (var item in files)
        {
            string filename = Path.GetFileName(item);
            DictionaryHelper.AddOrCreateIfDontExists<string, string>(result, filename, item);
        }

        return result;
    }

    internal static List<string> OnlyNamesNoDirectEdit(List<string> files2)
    {
        List<string> files = new List<string>(files2.Count);
        for (int i = 0; i < files2.Count; i++)
        {
            files.Add(Path.GetFileName(files2[i]));
        }
        return files;
    }

    internal static string AddExtensionIfDontHave(string file, string ext)
    {
        // For *.* and git paths {dir}/*
        if (file[file.Length - 1] == AllChars.asterisk)
        {
            return file;
        }
        if (Path.GetExtension(file) == string.Empty)
        {
            return file + ext;
        }

        return file;
    }



    /// <summary>
    ///     Usage: Exceptions.FileWasntFoundInDirectory
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    #region WithEndSlash
    internal static string WithEndSlash(ref string v)
    {
        if (v != string.Empty)
        {
            v = v.TrimEnd(AllChars.bs) + AllChars.bs;
        }

        SH.FirstCharUpper(ref v);
        return v;
    }

    internal static string WithEndSlash(string v)
    {
        return WithEndSlash(ref v);
    }
    #endregion

    internal static string Slash(string path, bool slash)
    {
        string result = null;
        if (slash)
        {
            result = path.Replace(AllStrings.bs, AllStrings.slash); //SHReplace.ReplaceAll2(path, AllStrings.slash, AllStrings.bs);
        }
        else
        {
            result = path.Replace(AllStrings.slash, AllStrings.bs); //SHReplace.ReplaceAll2(path, AllStrings.bs, AllStrings.slash);
        }

        SH.FirstCharUpper(ref result);
        return result;
    }


}