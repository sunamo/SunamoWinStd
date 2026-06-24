namespace SunamoWinStd._sunamo.SunamoFileSystem;

internal class FS
{
    internal static bool TryDeleteFile(string filePath)
    {
        try
        {
            File.Delete(filePath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("File cannot be deleted: " + filePath + " " + ex.Message);
            return false;
        }
    }

    internal static void CreateUpfoldersPhysicallyUnlessThere(string path)
    {
        CreateFoldersPhysicallyUnlessThere(Path.GetDirectoryName(path));
    }

    internal static void CreateFoldersPhysicallyUnlessThere(string? path)
    {
        ThrowEx.IsNullOrEmpty("path", path!);

        if (Directory.Exists(path))
        {
            return;
        }

        var foldersToCreate = new List<string>
        {
            path!
        };

        while (true)
        {
            path = Path.GetDirectoryName(path);

            if (Directory.Exists(path))
            {
                break;
            }

            foldersToCreate.Add(path!);
        }

        foldersToCreate.Reverse();
        foreach (string item in foldersToCreate)
        {
            if (!Directory.Exists(item))
            {
                Directory.CreateDirectory(item);
            }
        }
    }

    // Ensures the path ends with a backslash and has an uppercase first character.
    // Usage: Exceptions.FileWasntFoundInDirectory
    internal static string WithEndSlash(ref string path)
    {
        if (path != string.Empty)
        {
            path = path.TrimEnd('\\') + '\\';
        }

        SH.FirstCharUpper(ref path);
        return path;
    }
}
